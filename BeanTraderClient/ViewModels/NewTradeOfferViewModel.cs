using BeanTrader.Models;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Threading.Tasks;

namespace BeanTraderClient.ViewModels
{
    public class NewTradeOfferViewModel
    {
        private readonly Func<Task> closeDialogFunc;
        public delegate Task CreateTradeDelegate(TradeOffer tradeOffer);

        public BeanDictionary BeansOffered { get; set; } = new BeanDictionary();
        public BeanDictionary BeansAsked { get; set; } = new BeanDictionary();
        public event CreateTradeDelegate CreateTradeHandler;

        public bool? NewOfferAdded = null;

        public NewTradeOfferViewModel(Func<Task> closeDialogFunc)
        {
            this.closeDialogFunc = closeDialogFunc;
        }

        public async Task CreateTradeOffer()
        {
            await closeDialogFunc();

            await CreateTradeHandler?.Invoke(new TradeOffer
            {
                Asking = BeansAsked,
                Offering = BeansOffered
            });
        }

        public async Task CancelTradeOffer()
        {
            await closeDialogFunc();
        }
    }
}
