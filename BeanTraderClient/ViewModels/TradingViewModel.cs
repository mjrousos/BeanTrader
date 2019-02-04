using BeanTrader.Models;
using BeanTraderClient.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BeanTraderClient.ViewModels
{
    public class TradingViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Trader trader;
        private IList<TradeOffer> tradeOffers;

        public Trader CurrentTrader
        {
            get => trader;
            set
            {
                trader = value;
                OnPropertyChanged(nameof(UserName));
                OnPropertyChanged(nameof(Inventory));
                OnPropertyChanged(nameof(WelcomeMessage));
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
    }
}
