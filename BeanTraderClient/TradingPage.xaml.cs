using BeanTraderClient.ViewModels;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.ComponentModel;
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

            // Make sure that this page's model is cleaned up if
            // the page is navigated away from or if the app closes
            this.Unloaded += Unload;
            Application.Current.MainWindow.Closing += Unload;
        }

        private void Unload(object sender, EventArgs e)
        {
            Application.Current.MainWindow.Closing -= Unload;
            Model?.Dispose();
            Model = null;
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
