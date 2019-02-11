using System.Collections.Generic;
using System.Linq;

namespace BeanTrader.Models
{
    public partial class TradeOffer
    {
        // TODO
        public string SellerName => SellerId.ToString();

        public override string ToString() =>
            $"{BeansToString(Offering)} => {BeansToString(Asking)}";

        private object BeansToString(Dictionary<Beans, uint> beans) => string.Join(", ", beans.Select(b => $"{b.Value} {b.Key.ToString()[0]}"));
    }
}
