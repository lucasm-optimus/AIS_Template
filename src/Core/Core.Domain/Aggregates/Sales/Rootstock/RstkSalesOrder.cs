using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tilray.Integrations.Core.Domain.Aggregates.Sales.Rootstock
{
    public class RstkSalesOrder
    {
        private RstkSalesOrder() { }

        public string rstk__soapi_mode__c { get; private set; }
        public bool rstk__soapi_addorupdate__c { get; private set; }
        public string rstk__soapi_custref__c { get; private set; }
        public string rstk__soapi_salesdiv__c { get; private set; }
        public bool rstk__soapi_updatecustfields__c { get; private set; }
        public string currencyIsoCode { get; private set; }
        public ExternalReferenceId rstk__soapi_otype__r { get; private set; }
        public string rstk__soapi_orderdate__c { get; private set; }
        public string allocation_Sent_Date__c { get; private set; }
        public string ship_Date__c { get; private set; }
        public string expected_Delivery_Date__c { get; private set; }
        public string order_Received_Date__c { get; private set; }
        public string rstk__soapi_custpo__c { get; private set; }
        public ExternalReferenceId rstk__soapi_socust__r { get; private set; }
        public ExternalReferenceId rstk__soapi_ackaddr__r { get; private set; }
        public ExternalReferenceId rstk__soapi_shiptoaddr__r { get; private set; }
        public ExternalReferenceId rstk__soapi_instaddr__r { get; private set; }
        public ExternalReferenceId rstk__soapi_billtoaddr__r { get; private set; }
        public ExternalReferenceId rstk__soapi_carrier__r { get; private set; }
        public ExternalReferenceId rstk__soapi_shipvia__r { get; private set; }
        public bool? rstk__soapi_taxexempt__c { get; private set; }
        public string rstk__soapi_intcomment__c { get; private set; }
        public bool? rstk__soapi_async__c { get; private set; }
        public string rstk__soapi_upgroup__c { get; private set; }
        public string cc_Order__c { get; private set; }
        public ExternalReferenceId rstk__soapi_soprod__r { get; private set; }
        public double rstk__soapi_qtyorder__c { get; private set; }
        public double? rstk__soapi_price__c { get; private set; }
        public bool? rstk__soapi_firm__c { get; private set; }
        public double? amount_Covered_By_Insurance__c { get; private set; }
        public double? grams_Covered_By_Insurance__c { get; private set; }
        public string required_Lot_To_Pick__c { get; private set; }
        public ExternalReferenceId rstk__soapi_shipsite__r { get; private set; }
        public ExternalReferenceId rstk__soapi_shiplocid__r { get; private set; }
        public string rstk__soapi_shiplocnum__c { get; private set; }
        public string external_Order_Reference__c { get; private set; }

        public static RstkSalesOrder Create()
        {
            return new RstkSalesOrder();
        }

        public void SetRstk__soapi_mode__c(string value) => rstk__soapi_mode__c = value;
        public void SetRstk__soapi_addorupdate__c(bool value) => rstk__soapi_addorupdate__c = value;
        public void SetRstk__soapi_custref__c(string value) => rstk__soapi_custref__c = value;
        public void SetRstk__soapi_salesdiv__c(string value) => rstk__soapi_salesdiv__c = value;
        public void SetRstk__soapi_updatecustfields__c(bool value) => rstk__soapi_updatecustfields__c = value;
        public void SetCurrencyIsoCode(string value) => currencyIsoCode = value;
        public void SetRstk__soapi_otype__r(ExternalReferenceId value) => rstk__soapi_otype__r = value;
        public void SetRstk__soapi_orderdate__c(string value) => rstk__soapi_orderdate__c = value;
        public void SetAllocation_Sent_Date__c(string value) => allocation_Sent_Date__c = value;
        public void SetShip_Date__c(string value) => ship_Date__c = value;
        public void SetExpected_Delivery_Date__c(string value) => expected_Delivery_Date__c = value;
        public void SetOrder_Received_Date__c(string value) => order_Received_Date__c = value;
        public void SetRstk__soapi_custpo__c(string value) => rstk__soapi_custpo__c = value;
        public void SetRstk__soapi_socust__r(ExternalReferenceId value) => rstk__soapi_socust__r = value;
        public void SetRstk__soapi_ackaddr__r(ExternalReferenceId value) => rstk__soapi_ackaddr__r = value;
        public void SetRstk__soapi_shiptoaddr__r(ExternalReferenceId value) => rstk__soapi_shiptoaddr__r = value;
        public void SetRstk__soapi_instaddr__r(ExternalReferenceId value) => rstk__soapi_instaddr__r = value;
        public void SetRstk__soapi_billtoaddr__r(ExternalReferenceId value) => rstk__soapi_billtoaddr__r = value;
        public void SetRstk__soapi_carrier__r(ExternalReferenceId value) => rstk__soapi_carrier__r = value;
        public void SetRstk__soapi_shipvia__r(ExternalReferenceId value) => rstk__soapi_shipvia__r = value;
        public void SetRstk__soapi_taxexempt__c(bool? value) => rstk__soapi_taxexempt__c = value;
        public void SetRstk__soapi_intcomment__c(string value) => rstk__soapi_intcomment__c = value;
        public void SetRstk__soapi_async__c(bool? value) => rstk__soapi_async__c = value;
        public void SetRstk__soapi_upgroup__c(string value) => rstk__soapi_upgroup__c = value;
        public void SetCC_Order__c(string value) => cc_Order__c = value;
        public void SetRstk__soapi_soprod__r(ExternalReferenceId value) => rstk__soapi_soprod__r = value;
        public void SetRstk__soapi_qtyorder__c(double value) => rstk__soapi_qtyorder__c = value;
        public void SetRstk__soapi_price__c(double? value) => rstk__soapi_price__c = value;
        public void SetRstk__soapi_firm__c(bool? value) => rstk__soapi_firm__c = value;
        public void SetAmount_Covered_By_Insurance__c(double? value) => amount_Covered_By_Insurance__c = value;
        public void SetGrams_Covered_By_Insurance__c(double? value) => grams_Covered_By_Insurance__c = value;
        public void SetRequired_Lot_To_Pick__c(string value) => required_Lot_To_Pick__c = value;
        public void SetRstk__soapi_shipsite__r(ExternalReferenceId value) => rstk__soapi_shipsite__r = value;
        public void SetRstk__soapi_shiplocid__r(ExternalReferenceId value) => rstk__soapi_shiplocid__r = value;
        public void SetRstk__soapi_shiplocnum__c(string value) => rstk__soapi_shiplocnum__c = value;
        public void SetExternal_Order_Reference__c(string value) => external_Order_Reference__c = value;

        public static string? GetCreatedRowId(dynamic payload)
        {
            return payload["recordId"];
        }
    }

}
