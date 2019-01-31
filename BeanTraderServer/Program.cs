using Serilog;
using System;
using System.ServiceModel;

namespace BeanTraderServer
{
    class Program
    {
        static void Main(string[] args)
        {
            ConfigureLogging();

            using (var host = new ServiceHost(typeof(BeanTrader)))
            {
                host.Open();
                Log.Information("Bean Trader Service listening");
                WaitForExitSignal();
                Log.Information("Shutting down...");
                host.Close();
            }
        }

        private static void WaitForExitSignal()
        {
            Console.WriteLine("Press enter to exit");
            Console.ReadLine();
        }

        private static void ConfigureLogging()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            Log.Information("Logging initialized");
        }
    }
}
