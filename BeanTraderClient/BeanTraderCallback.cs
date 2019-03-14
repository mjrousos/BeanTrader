using BeanTrader.Service;
using System;

namespace BeanTraderClient
{
    public class BeanTraderCallback : BeanTraderServiceCallback
    {
        public event Action<TradeOffer> AddNewTradeOfferHandler;
        public event Action<Guid> RemoveTradeOfferHandler;
        public event Action<TradeOffer, Guid> TradeAcceptedHandler;

        public void AddNewTradeOffer(TradeOffer offer) => AddNewTradeOfferHandler?.Invoke(offer);
        public void RemoveTradeOffer(Guid offerId) => RemoveTradeOfferHandler?.Invoke(offerId);
        public void TradeAccepted(TradeOffer offer, Guid buyerId) => TradeAcceptedHandler?.Invoke(offer, buyerId);
    }
}
