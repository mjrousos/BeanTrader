using BeanTrader.Models;
using System;
using System.Threading.Tasks;

namespace BeanTraderClient.ViewModels
{
    public class NewTradeOfferViewModel
    {
        private readonly Func<Task> closeDialogFunc;

        public BeanDictionary BeansOffered { get; set; } = new BeanDictionary();
        public BeanDictionary BeansAsked { get; set; } = new BeanDictionary();
        public event Func<TradeOffer, Task> CreateTradeHandler;

        public bool? NewOfferAdded = null;

        public NewTradeOfferViewModel(Func<Task> closeDialogFunc)
        {
            this.closeDialogFunc = closeDialogFunc;
        }

        public async Task CreateTradeOfferAsync()
        {
            await closeDialogFunc();

            await CreateTradeHandler?.Invoke(new TradeOffer
            {
                Asking = BeansAsked,
                Offering = BeansOffered
            });
        }

        public async Task CancelTradeOfferAsync()
        {
            await closeDialogFunc();
        }
    }
}
