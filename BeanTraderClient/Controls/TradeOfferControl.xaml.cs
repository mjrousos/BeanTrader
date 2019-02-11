using BeanTrader.Models;
using BeanTraderClient.Resources;
using BeanTraderClient.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for TradeOfferControl.xaml
    /// </summary>
    public partial class TradeOfferControl : UserControl
    {
        public static readonly DependencyProperty TradeOfferProperty =
            DependencyProperty.Register("TradeOffer", typeof(TradeOffer), typeof(TradeOfferControl), new PropertyMetadata(new PropertyChangedCallback(UpdateDictionaries)));

        public static readonly DependencyProperty TradingModelProperty =
            DependencyProperty.Register("TradingModel", typeof(TradingViewModel), typeof(TradeOfferControl));

        private static void UpdateDictionaries(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Update these on TradeOffer so that the BeanDictionaries are available for
            // data binding without having to construct them each time they're needed
            if (e.NewValue is TradeOffer newTradeOffer && d is TradeOfferControl control)
            {
                control.Offering = new BeanDictionary(newTradeOffer.Offering);
                control.Asking = new BeanDictionary(newTradeOffer.Asking);
            }
        }

        public TradeOffer TradeOffer
        {
            get => (TradeOffer)GetValue(TradeOfferProperty);
            set => SetValue(TradeOfferProperty, value);
        }

        // Access parent trading model (if any) to allow making trades, etc.
        public TradingViewModel TradingModel
        {
            get => (TradingViewModel)GetValue(TradingModelProperty);
            set => SetValue(TradingModelProperty, value);
        }
        
        public BeanDictionary Offering { get; private set; }
        public BeanDictionary Asking { get; private set; }
        public string CompleteTradeDescription =>
            TradeOffer?.SellerId == TradingModel?.CurrentTrader.Id ?
            StringResources.CancelTradeDescription :
            StringResources.AcceptTradeDescription;

        public TradeOfferControl()
        {
            InitializeComponent();

            // Set LayoutRoot's data context so that data binding within this
            // control uses this object as the context but data binding by 
            // this control's users won't have their context's changed.
            LayoutRoot.DataContext = this;
        }

        private async void CompleteTradeButton_Click(object sender, RoutedEventArgs e)
        {
            CompleteTradeButton.IsEnabled = false;
            if (!await TradingModel?.CompleteTrade(TradeOffer))
            {
                CompleteTradeButton.IsEnabled = true;
            }            
        }
    }
}
