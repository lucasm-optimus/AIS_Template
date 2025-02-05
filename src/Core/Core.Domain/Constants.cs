using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tilray.Integrations.Core.Domain
{
    public static class Constants
    {
        public static class Ecom
        {
            public static class SalesOrder
            {
                public const string StoreName_AphriaMed = "AphriaMed";
                public const string StoreName_SweetWater = "SweetWater";
            }
        }

        public static class Rootstock
        {
            public static class TableNames
            {
                public const string Customer = "rstk__socust__c";
                public const string CustomerAddress = "rstk__socaddr__c";
                public const string SalesOrder =  "rstk__soapi__c";
                public const string Prepayment = "rstk__soppy__c";
            }
        }
    }
}
