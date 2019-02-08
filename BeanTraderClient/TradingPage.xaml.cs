using BeanTrader.Models;
using BeanTraderClient.ViewModels;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
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
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            MainWindow.BeanTrader.StopListening();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // As an example, do work asynchronously with Delegate.BeginInvoke to demonstrate
            // how such calls can be ported to .NET Core.
            Func<Trader> userInfoRetriever = MainWindow.BeanTrader.GetCurrentTraderInfo;
            userInfoRetriever.BeginInvoke(result =>
            {
                Model.CurrentTrader = userInfoRetriever.EndInvoke(result);
            }, null);
        }

        private async void NewTradeButton_Click(object sender, RoutedEventArgs e)
        {
            await Model.CreateNewTradeOffer();
        }
    }
}
