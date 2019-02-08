using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeanTrader.Models
{
    public partial class TradeOffer
    {
        // TODO
        public string SellerName => SellerId.ToString();

        public override string ToString() =>
            $"{BeansToString(Offering)} => {BeansToString(Asking)}";

        private object BeansToString(Dictionary<Beans, uint> beans) => string.Join(", ", beans.Select(b => $"{b.Value} {b.Key}"));
    }
}
