using MahApps.Metro.Controls;
using Microsoft.Azure.KeyVault;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Threading.Tasks;

namespace BeanTraderClient.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private static BeanTraderServiceClient beanTraderClient;
        private static readonly object clientSyncObject = new object();

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
                            beanTraderClient.ClientCredentials.ClientCertificate.Certificate = GetCertificate();
                            beanTraderClient.ClientCredentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.None;
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

        private static X509Certificate2 GetCertificate()
        {
            var keyVaultClient = new KeyVaultClient(GetAzureAccessToken);
            var cert = keyVaultClient.GetSecretAsync(ConfigurationManager.AppSettings["CertificateSecretIdentifier"]).Result.Value;
            return new X509Certificate2(Convert.FromBase64String(cert));
        }

        private static async Task<string> GetAzureAccessToken(string authority, string resource, string scope)
        {
            var appCredentials = new ClientCredential(ConfigurationManager.AppSettings["AzureAppId"], ConfigurationManager.AppSettings["AzureAppPassword"]);
            var context = new AuthenticationContext(authority, TokenCache.DefaultShared);

            var result = await context.AcquireTokenAsync(resource, appCredentials);

            return result.AccessToken;
        }

        public static BeanTraderCallback BeanTraderCallbackHandler { get; set; } = new BeanTraderCallback();

        public MainWindow(WelcomePage welcomePage)
        {
            InitializeComponent();

            MainFrame.Navigate(welcomePage);
        }
    }
}
