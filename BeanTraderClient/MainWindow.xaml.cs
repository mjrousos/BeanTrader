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
        private static BeanTraderServiceClient beanTraderClient;
        private static object clientSyncObject = new object();

        // TODO : Might be nice to get these from DI. For now, they can just be statics
        public static string CurrentUsername { get; set; }
        public static BeanTraderServiceClient BeanTrader
        {
            get
            {
                // TODO : This is not the best solution. It would be better to move all beanTraderClient interaction into a dedicated BeanTraderService
                // class and include an 'on reconnect' event. Then, have that class wrap calls in try/catches and use a timer to check for heartbeat every ~1 second
                // On failure, it would re-connect and trigger the 'on reconnected' event (to allow the app to retrieve trade offers, etc.)
                if (beanTraderClient == null || beanTraderClient.State == CommunicationState.Closed || beanTraderClient.State == CommunicationState.Faulted)
                {
                    lock (clientSyncObject)
                    {
                        if (beanTraderClient == null || beanTraderClient.State == CommunicationState.Closed || beanTraderClient.State == CommunicationState.Faulted)
                        {
                            beanTraderClient = new BeanTraderServiceClient(new InstanceContext(BeanTraderCallbackHandler));
                            beanTraderClient.Open();

                            // Set user name ("login")
                            if (!string.IsNullOrEmpty(CurrentUsername))
                            {
                                beanTraderClient.Login(CurrentUsername);
                            }
                        }
                    }
                }

                return beanTraderClient;
            }
        }

        public static BeanTraderCallback BeanTraderCallbackHandler { get; set; } = new BeanTraderCallback();

        public MainWindow()
        {
            InitializeComponent();

            MainFrame.Navigate(new WelcomePage());
        }
    }
}
