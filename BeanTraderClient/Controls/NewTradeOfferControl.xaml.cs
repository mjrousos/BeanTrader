using BeanTraderClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BeanTraderClient.Controls
{
    /// <summary>
    /// Interaction logic for NewTradeOfferControl.xaml
    /// </summary>
    public partial class NewTradeOfferControl : UserControl
    {
        NewTradeOfferViewModel Model => DataContext as NewTradeOfferViewModel;

        public NewTradeOfferControl()
        {
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) => Model.CancelTradeOffer();
        private void CreateButton_Click(object sender, RoutedEventArgs e) => Model.CreateTradeOffer();
    }
}
