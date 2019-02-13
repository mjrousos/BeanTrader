using BeanTrader.Models;
using BeanTraderClient.Controls;
using BeanTraderClient.Resources;
using BeanTraderClient.Views;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        // Initialized by ListenForTradeOffers, this field caches trader names (indexed by ID)
        private ConcurrentDictionary<Guid, string> traderNames;

        public TradingViewModel(IDialogCoordinator dialogCoordinator)
        {
            this.dialogCoordinator = dialogCoordinator;
            statusClearTimer = new Timer(ClearStatus);
        }

        public void Load()
        {
            // Get initial trader info and trade offers
            UpdateTraderInfo();
            ListenForTradeOffers();

            // Register for service callbacks
            MainWindow.BeanTraderCallbackHandler.AddNewTradeOfferHandler += AddTradeOffer;
            MainWindow.BeanTraderCallbackHandler.RemoveTradeOfferHandler += RemoveTraderOffer;
            MainWindow.BeanTraderCallbackHandler.TradeAcceptedHandler += TradeAccepted;
        }

        public void Unload()
        {
            // Stop listening
            Logout();

            // Unregister for service callbacks
            MainWindow.BeanTraderCallbackHandler.AddNewTradeOfferHandler -= AddTradeOffer;
            MainWindow.BeanTraderCallbackHandler.RemoveTradeOfferHandler -= RemoveTraderOffer;
            MainWindow.BeanTraderCallbackHandler.TradeAcceptedHandler -= TradeAccepted;

            // Clear model data
            CurrentTrader = null;
            TradeOffers = null;
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

        public string GetTraderName(Guid sellerId)
        {
            if (!traderNames.TryGetValue(sellerId, out string sellerName))
            {
                var names = MainWindow.BeanTrader.GetTraderNames(new Guid[] { sellerId });

                sellerName = names.ContainsKey(sellerId) ?
                    traderNames.AddOrUpdate(sellerId, names[sellerId], (g, s) => names[sellerId]) :
                    null;
            }

            return sellerName;
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
                    var tradeOffers = offersTask.Result;
                    var sellerIds = tradeOffers.Select(t => t.SellerId);
                    traderNames = new ConcurrentDictionary<Guid, string>(MainWindow.BeanTrader.GetTraderNames(sellerIds.ToArray()));
                    TradeOffers = new ObservableCollection<TradeOffer>(tradeOffers);
                });
        }

        private void Logout()
        {
            MainWindow.BeanTrader.StopListening();
            MainWindow.BeanTrader.Logout();
        }

        private void RemoveTraderOffer(Guid offerId)
        {
            var offer = tradeOffers.SingleOrDefault(o => o.Id == offerId);
            if (offer != null)
            {
                Application.Current.Dispatcher.Invoke(() =>TradeOffers.Remove(offer));
            }
        }

        private void AddTradeOffer(TradeOffer offer)
        {
            if (!tradeOffers.Any(o => o.Id == offer.Id))
            {
                Application.Current.Dispatcher.Invoke(() => TradeOffers.Add(offer));
            }
        }

        private void TradeAccepted(TradeOffer offer, Guid buyerId)
        {
            if (offer.SellerId == CurrentTrader.Id)
            {
                Task.Run(() =>
                {
                    SetStatus($"Trade ({offer}) accepted by {GetTraderName(buyerId) ?? buyerId.ToString()}");
                    UpdateTraderInfo();
                });
            }
        }

        public async Task<bool> CompleteTrade(TradeOffer tradeOffer)
        {
            var ownTrade = tradeOffer.SellerId == CurrentTrader.Id;
            var success = ownTrade ?
                await MainWindow.BeanTrader?.CancelTradeOfferAsync(tradeOffer.Id) :
                await MainWindow.BeanTrader?.AcceptTradeAsync(tradeOffer.Id);

            if (success)
            {
                SetStatus($"{(ownTrade ? "Canceled" : "Accepted")} trade ({tradeOffer})");

                // Remove the trade from the list and update trader inventory
                //RemoveTraderOffer(tradeOffer.Id);
                UpdateTraderInfo();
            }
            else
            {
                SetStatus($"Unable to {(ownTrade ? "cancel" : "accept")} trade ({tradeOffer}).", Application.Current.FindResource("ErrorBrush") as Brush);
            }

            return success;
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
                SetStatus("ERROR: Trade offer could not be created. Do you have enough beans?", Application.Current.FindResource("ErrorBrush") as Brush);
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
