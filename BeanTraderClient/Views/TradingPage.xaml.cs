using BeanTraderClient.ViewModels;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace BeanTraderClient.Views
{
    /// <summary>
    /// Interaction logic for TradingPage.xaml
    /// </summary>
    public partial class TradingPage : Page
    {
        public TradingViewModel Model { get; }

        public TradingPage(TradingViewModel viewModel)
        {
            InitializeComponent();
            Model = viewModel;
            this.DataContext = this.Model;
        }

        private void Load(object sender, RoutedEventArgs e)
        {
            Model.Load();

            // Make sure that this page's model is
            // cleaned up if the app closes
            Application.Current.MainWindow.Closing += Unload;
        }

        private void Unload(object sender, EventArgs e)
        {
            Application.Current.MainWindow.Closing -= Unload;
            Model.Unload();
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
