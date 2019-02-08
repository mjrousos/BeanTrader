using BeanTraderClient.ViewModels;
using MahApps.Metro.Controls.Dialogs;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace BeanTraderClient
{
    /// <summary>
    /// Interaction logic for TradingPage.xaml
    /// </summary>
    public partial class TradingPage : Page
    {
        public TradingViewModel Model { get; set; } = new TradingViewModel(DialogCoordinator.Instance);

        public TradingPage()
        {
            InitializeComponent();
            this.DataContext = this.Model;
            this.Unloaded += (sender, args) => Model.Dispose();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
        
        private async void NewTradeButton_Click(object sender, RoutedEventArgs e)
        {
            await Model.ShowNewTradeOfferDialog();
        }
    }
}
