using BeanTrader.Models;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace BeanTrader
{
    [ServiceContract(Name = "BeanTraderService", CallbackContract = typeof(IBeanTraderCallback), SessionMode = SessionMode.Required)]
    public interface IBeanTrader
    {
        [OperationContract]
        IEnumerable<TradeOffer> ListenForTradeOffers();

        [OperationContract]
        Trader GetCurrentTraderInfo();

        [OperationContract(IsOneWay = true)]
        void SetTraderName(string name);

        [OperationContract]
        string GetTraderName(Guid traderId);

        [OperationContract]
        bool AcceptTrade(Guid offerId);

        [OperationContract]
        Guid OfferTrade(TradeOffer offer);

        [OperationContract]
        bool CancelTradeOffer(Guid offerId);

        [OperationContract(IsOneWay = true)]
        void StopListening();
    }
}
