using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
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

namespace BeanTraderClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        // TODO : Might be nice to get these from DI. For now, they can just be statics
        public static BeanTraderService BeanTrader { get; set; }
        public static BeanTraderCallback BeanTraderCallbackHandler { get; set; } = new BeanTraderCallback();

        public MainWindow()
        {
            InitializeComponent();

            MainFrame.Navigate(new WelcomePage());
        }
    }
}
