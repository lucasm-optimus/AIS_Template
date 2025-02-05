using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tilray.Integrations.Core.Domain.Aggregates.Sales
{
    public class RstkSalesOrderLineItem
    {
        private RstkSalesOrderLineItem() { }

        public string rstk__soapi_mode__c { get; private set; }
        public string rstk__soapi_sohdr__c { get; private set; }
        public ExternalReferenceId rstk__soapi_soprod__r { get; private set; }
        public double rstk__soapi_qtyorder__c { get; private set; }
        public double? rstk__soapi_price__c { get; private set; }
        public bool? rstk__soapi_firm__c { get; private set; }
        public bool rstk__soapi_taxexempt__c { get; private set; }
        public string required_Lot_To_Pick__c { get; private set; }
        public bool rstk__soapi_updatecustfields__c { get; private set; }
        public bool? rstk__soapi_async__c { get; private set; }
        public string rstk__soapi_upgroup__c { get; private set; }
        public double? amount_Covered_By_Insurance__c { get; private set; }
        public double? grams_Covered_By_Insurance__c { get; private set; }
        public ExternalReferenceId rstk__soapi_shipsite__r { get; private set; }
        public ExternalReferenceId rstk__soapi_shiplocid__r { get; private set; }
        public string rstk__soapi_shiplocnum__c { get; private set; }
        public string currencyIsoCode { get; private set; }

        public static RstkSalesOrderLineItem Create()
        {
            return new RstkSalesOrderLineItem();
        }

        public void SetRstk__soapi_mode__c(string value) => rstk__soapi_mode__c = value;
        public void SetRstk__soapi_sohdr__c(string value) => rstk__soapi_sohdr__c = value;
        public void SetRstk__soapi_soprod__r(ExternalReferenceId value) => rstk__soapi_soprod__r = value;
        public void SetRstk__soapi_qtyorder__c(double value) => rstk__soapi_qtyorder__c = value;
        public void SetRstk__soapi_price__c(double? value) => rstk__soapi_price__c = value;
        public void SetRstk__soapi_firm__c(bool? value) => rstk__soapi_firm__c = value;
        public void SetRstk__soapi_taxexempt__c(bool value) => rstk__soapi_taxexempt__c = value;
        public void SetRequired_Lot_To_Pick__c(string value) => required_Lot_To_Pick__c = value;
        public void SetRstk__soapi_updatecustfields__c(bool value) => rstk__soapi_updatecustfields__c = value;
        public void SetRstk__soapi_async__c(bool? value) => rstk__soapi_async__c = value;
        public void SetRstk__soapi_upgroup__c(string value) => rstk__soapi_upgroup__c = value;
        public void SetAmount_Covered_By_Insurance__c(double? value) => amount_Covered_By_Insurance__c = value;
        public void SetGrams_Covered_By_Insurance__c(double? value) => grams_Covered_By_Insurance__c = value;
        public void SetRstk__soapi_shipsite__r(ExternalReferenceId value) => rstk__soapi_shipsite__r = value;
        public void SetRstk__soapi_shiplocid__r(ExternalReferenceId value) => rstk__soapi_shiplocid__r = value;
        public void SetRstk__soapi_shiplocnum__c(string value) => rstk__soapi_shiplocnum__c = value;
        public void SetCurrencyIsoCode(string value) => currencyIsoCode = value;
    }
}
