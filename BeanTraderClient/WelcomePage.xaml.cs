using System.ComponentModel;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Win32;

namespace BeanTraderClient
{
    /// <summary>
    /// Interaction logic for Welcome.xaml
    /// </summary>
    public partial class WelcomePage : Page
    {
        private const string BeanTraderClientRegistryKey = @"Software\BeanTraderClient";
        private const string PreferredNameRegistryValueName = "PreferredUsername";

        public WelcomePage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            NameTextBox.Text = GetPreferredUserName();
            NameTextBox.Focus();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            var userName = NameTextBox.Text;
            SetPreferredUserName(userName);

            StartButton.IsEnabled = false;

            // This would be simpler with Task.Run, but I want to
            // use APIs that would have been more common in older WPF apps
            var worker = new BackgroundWorker();
            worker.DoWork += Login;
            worker.RunWorkerCompleted += CompleteLogin;
            worker.RunWorkerAsync(userName);
        }

        private void Login(object sender, DoWorkEventArgs e)
        {
            MainWindow.BeanTrader = new BeanTraderServiceClient(new InstanceContext(MainWindow.BeanTraderCallbackHandler));
            MainWindow.BeanTrader.Login(e.Argument as string);
        }

        private void CompleteLogin(object sender, RunWorkerCompletedEventArgs e)
        {
            StartButton.IsEnabled = true;
            NavigationService.Navigate(new TradingPage());
        }

        private string GetPreferredUserName()
        {
            var regKey = Registry.CurrentUser.CreateSubKey(BeanTraderClientRegistryKey);
            return regKey.GetValue(PreferredNameRegistryValueName)?.ToString() ?? string.Empty;
        }

        private void SetPreferredUserName(string userName)
        {
            var regKey = Registry.CurrentUser.CreateSubKey(BeanTraderClientRegistryKey);
            regKey.SetValue(PreferredNameRegistryValueName, userName);
        }
    }
}
