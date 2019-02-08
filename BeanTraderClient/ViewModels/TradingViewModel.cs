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
    public class TradingViewModel : INotifyPropertyChanged, IDisposable
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

            // Get initial trader info and trade offers
            UpdateTraderInfo();
            ListenForTradeOffers();

            // Register for service callbacks
            MainWindow.BeanTraderCallbackHandler.AddNewTradeOfferHandler += AddTradeOffer;
            MainWindow.BeanTraderCallbackHandler.RemoveTradeOfferHandler += RemoveTraderOffer;
            MainWindow.BeanTraderCallbackHandler.TradeAcceptedHandler += TradeAccepted;
        }

        public void Dispose()
        {
            // Stop listening
            StopListeningForTrades();

            // Unregister for service callbacks
            MainWindow.BeanTraderCallbackHandler.AddNewTradeOfferHandler -= AddTradeOffer;
            MainWindow.BeanTraderCallbackHandler.RemoveTradeOfferHandler -= RemoveTraderOffer;
            MainWindow.BeanTraderCallbackHandler.TradeAcceptedHandler -= TradeAccepted;
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

        public string UserName => CurrentTrader?.Name;
        public int[] Inventory => CurrentTrader?.Inventory ?? new int[4];

        public string WelcomeMessage =>
            string.IsNullOrEmpty(UserName) ?
            StringResources.DefaultGreeting :
            string.Format(StringResources.Greeting, UserName);

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private void UpdateTraderInfo()
        {
            // As an example, do work asynchronously with Delegate.BeginInvoke to demonstrate
            // how such calls can be ported to .NET Core.
            Func<Trader> userInfoRetriever = MainWindow.BeanTrader.GetCurrentTraderInfo;
            userInfoRetriever.BeginInvoke(result =>
            {
                CurrentTrader = userInfoRetriever.EndInvoke(result);
            }, null);
        }

        private void ListenForTradeOffers()
        {
            // Different async pattern just for demonstration's sake
            MainWindow.BeanTrader.ListenForTradeOffersAsync()
                .ContinueWith(offersTask =>
                {
                    TradeOffers = offersTask.Result;
                });
        }

        private void StopListeningForTrades()
        {
            MainWindow.BeanTrader.StopListening();
        }

        private void RemoveTraderOffer(Guid offerId)
        {
            var offer = tradeOffers.SingleOrDefault(o => o.Id == offerId);
            if (offer != null)
            {
                tradeOffers.Remove(offer);
                OnPropertyChanged(nameof(TradeOffers));
            }
        }

        private void AddTradeOffer(TradeOffer offer)
        {
            if (!tradeOffers.Any(o => o.Id == offer.Id))
            {
                tradeOffers.Add(offer);
                OnPropertyChanged(nameof(TradeOffers));
            }
        }

        private void TradeAccepted(TradeOffer offer, Guid buyerId)
        {
            if (offer.SellerId == CurrentTrader.Id)
            {
                // TODO - Get buyer name
                SetStatus($"Trade ({offer}) accepted by {buyerId}");
            }
        }

        public async Task ShowNewTradeOfferDialog()
        {
            var newTradeDialog = new CustomDialog
            {
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Background = Application.Current.FindResource("WindowBackgroundBrush") as Brush,
                Style = Application.Current.FindResource("DefaultControlStyle") as Style
            };

            var newTradeOfferViewModel = new NewTradeOfferViewModel(() => dialogCoordinator.HideMetroDialogAsync(this, newTradeDialog));
            newTradeOfferViewModel.CreateTradeHandler += CreateTradeOfferAsync;

            newTradeDialog.Content = new NewTradeOfferControl
            {
                DataContext = newTradeOfferViewModel
            };

            await dialogCoordinator.ShowMetroDialogAsync(this, newTradeDialog);
        }

        private async Task CreateTradeOfferAsync(TradeOffer tradeOffer)
        {
            if (await MainWindow.BeanTrader.OfferTradeAsync(tradeOffer) != Guid.Empty)
            {
                SetStatus("New trade offer created");
            }
            else
            {
                SetStatus("ERROR - Trade offer could not be created. Do you have sufficient beans?", Application.Current.FindResource("ErrorBrush") as Brush);
            }

            UpdateTraderInfo();
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
