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

        // Select text in the bean count text boxes so that users can easily replace the initial '0'
        private void BeanTextBox_GotFocus(object sender, EventArgs e)
        {
            if (sender is TextBox textBox)
            {
                textBox.SelectAll();
            }
        }

        private void IgnoreIfUnfocused(object sender, MouseEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (!textBox.IsKeyboardFocusWithin)
                {
                    textBox.Focus();
                    e.Handled = true;
                }
            }
        }
    }
}
