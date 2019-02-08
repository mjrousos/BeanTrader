using BeanTrader.Models;
using BeanTraderClient.Controls;
using BeanTraderClient.Resources;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace BeanTraderClient.ViewModels
{
    public class TradingViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Trader trader;
        private IList<TradeOffer> tradeOffers;
        private IDialogCoordinator dialogCoordinator;

        public TradingViewModel(IDialogCoordinator dialogCoordinator)
        {
            this.dialogCoordinator = dialogCoordinator;
        }

        public Trader CurrentTrader
        {
            get => trader;
            set
            {
                if (trader != value)
                {
                    trader = value;
                    OnPropertyChanged(nameof(UserName));
                    OnPropertyChanged(nameof(Inventory));
                    OnPropertyChanged(nameof(WelcomeMessage));
                }
            }
        }

        public IList<TradeOffer> TradeOffers
        {
            get => tradeOffers;
            set
            {
                tradeOffers = value;
                OnPropertyChanged(nameof(TradeOffers));
            }
        }

        public void RemoveTraderOffer(Guid offerId)
        {
            var offer = tradeOffers.SingleOrDefault(o => o.Id == offerId);
            if (offer != null)
            {
                tradeOffers.Remove(offer);
                OnPropertyChanged(nameof(TradeOffers));
            }
        }

        public void AddTradeOffer(TradeOffer offer)
        {
            if (!tradeOffers.Any(o => o.Id == offer.Id))
            {
                tradeOffers.Add(offer);
                OnPropertyChanged(nameof(TradeOffers));
            }
        }

        public string UserName => CurrentTrader?.Name;
        public int[] Inventory => CurrentTrader?.Inventory ?? new int[0];

        public string WelcomeMessage =>
            string.IsNullOrEmpty(UserName) ?
            StringResources.DefaultGreeting :
            string.Format(StringResources.Greeting, UserName);

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async Task CreateNewTradeOffer()
        {
            var newTradeDialog = new CustomDialog
            {
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Background = Application.Current.FindResource("WindowBackgroundBrush") as Brush,
                Style = Application.Current.FindResource("DefaultControlStyle") as Style
            };

            var newTradeOfferViewModel = new NewTradeOfferViewModel(dialogCoordinator, () => dialogCoordinator.HideMetroDialogAsync(this, newTradeDialog));

            newTradeDialog.Content = new NewTradeOfferControl
            {
                DataContext = newTradeOfferViewModel
            };

            await dialogCoordinator.ShowMetroDialogAsync(this, newTradeDialog);
        }
    }
}
