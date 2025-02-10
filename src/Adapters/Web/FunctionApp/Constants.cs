using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tilray.Integrations.Functions
{
    public static class Constants
    {
        public static class ServiceBus
        {
            public static class Topics
            {
                public const string EcomSalesOrderReceived = "EcomSalesOrderReceived";
            }

            public static class Subscriptions
            {
                public const string CreateSalesOrderInRootStock = "EcomSalesOrderReceived_CreateSalesOrderInRootStock";
            }
        }
    }
}
