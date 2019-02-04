using BeanTrader.Models;
using BeanTraderClient.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeanTraderClient.ViewModels
{
    public class TradingViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Trader trader;

        public Trader CurrentTrader
        {
            get { return trader; }
            set
            {
                trader = value;
                OnPropertyChanged(nameof(UserName));
                OnPropertyChanged(nameof(Inventory));
                OnPropertyChanged(nameof(WelcomeMessage));
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
