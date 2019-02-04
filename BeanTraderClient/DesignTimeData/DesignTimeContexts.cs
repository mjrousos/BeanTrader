using BeanTrader.Models;
using BeanTraderClient.ViewModels;
using System;

namespace BeanTraderClient.DesignTimeData
{
    public static class DesignTimeContexts
    {
        public static TradingViewModel DesignTimeTradingViewModel =>
            new TradingViewModel
            {
                CurrentTrader = new Trader
                {
                    Name = "Test User",
                    Id = Guid.Empty,
                    Inventory = new[] { 100, 50, 10, 1 }
                }
            };
    }
}
