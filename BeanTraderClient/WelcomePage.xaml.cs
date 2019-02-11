using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for Welcome.xaml
    /// </summary>
    public partial class WelcomePage : Page
    {
        public WelcomePage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // TODO : Store/retrieve name from registry
            NameTextBox.Text = string.Empty;
            NameTextBox.Focus();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            // This would be simpler with Task.Run, but I want to
            // use APIs that would have been more common in older WPF apps
            var worker = new BackgroundWorker();
            worker.DoWork += Login;
            worker.RunWorkerCompleted += CompleteLogin;
            worker.RunWorkerAsync(NameTextBox.Text);
        }

        private void Login(object sender, DoWorkEventArgs e)
        {
            MainWindow.BeanTrader = new BeanTraderServiceClient(new InstanceContext(MainWindow.BeanTraderCallbackHandler));
            MainWindow.BeanTrader.Login(e.Argument as string);
        }

        private void CompleteLogin(object sender, RunWorkerCompletedEventArgs e)
        {
            NavigationService.Navigate(new TradingPage());
        }

    }
}
