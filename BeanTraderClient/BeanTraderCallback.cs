using BeanTrader;
using BeanTrader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeanTraderClient
{
    public class BeanTraderCallback : BeanTraderServiceCallback
    {
        public void AddNewTradeOffer(TradeOffer offer)
        {
            throw new NotImplementedException();
        }

        public void RemoveTradeOffer(Guid offerId)
        {
            throw new NotImplementedException();
        }

        public void TradeAccepted(Guid offerId, Guid buyerId, string buyerName)
        {
            throw new NotImplementedException();
        }
    }
}
