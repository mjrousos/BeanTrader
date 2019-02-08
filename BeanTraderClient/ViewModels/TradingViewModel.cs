using BeanTrader.Models;
using BeanTraderClient.Controls;
using BeanTraderClient.Resources;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace BeanTraderClient.ViewModels
{
    public class TradingViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Trader trader;
        private string statusText;
        private Brush statusBrush;
        private IList<TradeOffer> tradeOffers;
        private readonly IDialogCoordinator dialogCoordinator;
        private readonly Timer statusClearTimer;

        public TradingViewModel(IDialogCoordinator dialogCoordinator)
        {
            this.dialogCoordinator = dialogCoordinator;
            statusClearTimer = new Timer(ClearStatus);
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

        public string StatusText
        {
            get => statusText;
            set
            {
                if (statusText != value)
                {
                    statusText = value;
                    OnPropertyChanged(nameof(StatusText));
                }
            }
        }

        public Brush StatusBrush
        {
            get => statusBrush;
            set
            {
                if (statusBrush != value)
                {
                    statusBrush = value;
                    OnPropertyChanged(nameof(StatusBrush));
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

            var newTradeOfferViewModel = new NewTradeOfferViewModel(() => dialogCoordinator.HideMetroDialogAsync(this, newTradeDialog));

            newTradeDialog.Content = new NewTradeOfferControl
            {
                DataContext = newTradeOfferViewModel
            };

            await dialogCoordinator.ShowMetroDialogAsync(this, newTradeDialog);
        }
        private void SetStatus(string message) => SetStatus(message, Application.Current.FindResource("IdealForegroundColorBrush") as Brush);

        private void SetStatus(string message, Brush brush)
        {
            StatusText = message;
            StatusBrush = brush;
            ResetStatusClearTimer();
        }

        private void ResetStatusClearTimer(int dueTime = 5000)
        {
            statusClearTimer.Change(dueTime, Timeout.Infinite);
        }

        private void ClearStatus(object _)
        {
            StatusText = string.Empty;
        }

    }
}
