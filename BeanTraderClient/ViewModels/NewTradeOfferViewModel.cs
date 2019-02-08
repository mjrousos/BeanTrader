using BeanTrader.Models;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Threading.Tasks;

namespace BeanTraderClient.ViewModels
{
    public class NewTradeOfferViewModel
    {
        private readonly Func<Task> closeDialogFunc;

        public BeanDictionary BeansOffered { get; set; } = new BeanDictionary();
        public BeanDictionary BeansAsked { get; set; } = new BeanDictionary();

        public bool? NewOfferAdded = null;

        public NewTradeOfferViewModel(IDialogCoordinator dialogCoordinator, Func<Task> closeDialogFunc)
        {
            this.dialogCoordinator = dialogCoordinator;
            this.closeDialogFunc = closeDialogFunc;
        }

        public async Task CreateTradeOffer()
        {
            await closeDialogFunc();
        }

        public async Task CancelTradeOffer()
        {
            await closeDialogFunc();
        }
    }
}
