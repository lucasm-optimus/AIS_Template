using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Tilray.Integrations.Core.Domain.Aggregates.Sales
{
    public class SalesOrder : Entity
    {
        private SalesOrder() { LineItems = new(); }

        [JsonProperty("division")]
        public string Division { get; private set; }

        [JsonProperty("customerReference")]
        public string CustomerReference { get; private set; }

        [JsonProperty("eCommerceOrderID")]
        public string ECommerceOrderID { get; private set; }

        [JsonProperty("customer")]
        public string Customer { get; private set; }

        [JsonProperty("updateOrderIfExists")]
        public bool UpdateOrderIfExists { get; private set; }

        [JsonProperty("orderDate")]
        public DateTime OrderDate { get; private set; }

        [JsonProperty("orderReceivedDate")]
        public DateTime OrderReceivedDate { get; private set; }

        [JsonProperty("orderType")]
        public string OrderType { get; private set; }

        [JsonProperty("shippingCarrier")]
        public string ShippingCarrier { get; private set; }

        [JsonProperty("shippingMethod")]
        public string ShippingMethod { get; private set; }

        [JsonProperty("shipDate")]
        public DateTime ShipDate { get; private set; }

        [JsonProperty("taxExempt")]
        public bool TaxExempt { get; private set; }

        [JsonProperty("notes")]
        public string Notes { get; private set; }

        [JsonProperty("customerPO")]
        public string CustomerPO { get; private set; }

        [JsonProperty("currencyIsoCode")]
        public string CurrencyIsoCode { get; private set; }

        [JsonProperty("ccOrder")]
        public string CCOrder { get; private set; }

        [JsonProperty("shipTo")]
        public string ShipTo { get; private set; }

        [JsonProperty("cardCode")]
        public string CardCode { get; private set; }

        [JsonProperty("ccPrepayment")]
        public CCPrepayment CCPrepayment { get; private set; }

        [JsonProperty("standardPrepayment")]
        public StandardPrepayment StandardPrepayment { get; private set; }

        [JsonProperty("customerAddresses")]
        public CustomerAddresses CustomerAddresses { get; private set; }

        [JsonProperty("lineItems")]
        public List<LineItem> LineItems { get; private set; }

        public static SalesOrder Create(EcomSalesOrder payload, OrderDefaultsSettings orderDefaults, string customerAddressReference)
        {
            if (payload.StoreName == Constants.Ecom.SalesOrder.StoreName_AphriaMed)
            {
                return CreateForAphria(payload, orderDefaults, customerAddressReference);
            }
            else if (payload.StoreName == Constants.Ecom.SalesOrder.StoreName_SweetWater)
            {
                return CreateForSweetWater(payload, orderDefaults, customerAddressReference);
            }
            else
            {
                return null;
            }
        }

        private static SalesOrder CreateForSweetWater(EcomSalesOrder payload, OrderDefaultsSettings orderDefaults, string customerAddressReference)
        {
            var salesOrder = new SalesOrder();
            salesOrder.SetDivision(orderDefaults.SweetWater.Division);
            salesOrder.SetCustomerReference(payload.ECommOrderNo + orderDefaults.SweetWater.OrderReferenceSuffix);
            salesOrder.SetECommerceOrderID(payload.ECommOrderID);
            salesOrder.SetCustomer(payload.CustomerAccountNumber);
            salesOrder.SetUpdateOrderIfExists(false);
            salesOrder.SetOrderDate(payload.OrderedOn);
            salesOrder.SetOrderType(orderDefaults.Medical.OrderType);
            salesOrder.SetShippingCarrier(payload.ShippingCarrier);
            salesOrder.SetShippingMethod(payload.ShippingMethod);
            salesOrder.SetNotes(payload.Notes);
            salesOrder.SetShipDate(string.IsNullOrWhiteSpace(payload.ShippingWeek) ? DateTime.UtcNow.Date : Convert.ToDateTime(payload.ShippingWeek));//check for value: ShippingWeek
            salesOrder.SetTaxExempt(true);
            salesOrder.SetCustomerPO(payload.CustomerPO != null ? payload.CustomerPO : null);
            salesOrder.SetCurrencyIsoCode("USD");
            salesOrder.SetCCOrder(payload.ECommOrderID);
            salesOrder.SetShipTo(customerAddressReference);

            foreach (var item in payload.OrderLines)
            {
                var lineItem = LineItem.Create();
                lineItem.SetItemNumber(string.IsNullOrWhiteSpace(item.ObeerSku) ? item.ObeerSku : item.Product);
                lineItem.SetQuantity(item.Quantity);
                lineItem.SetUnitPrice(item.UnitPrice);
                lineItem.SetFirm(false);
                lineItem.SetLocation(item.FulFillLoc);
                lineItem.SetId(item.Id);
                salesOrder.AddLineItem(lineItem);
            }

            if (payload.Taxes.Any())
            {
                foreach (var tax in payload.Taxes)
                {
                    var lineItem = LineItem.Create();
                    lineItem.SetItemNumber(GetTaxCode(orderDefaults, payload.ShipToState, tax));
                    lineItem.SetQuantity(1);
                    lineItem.SetUnitPrice(tax.Amount);
                    lineItem.SetAmountCoveredByInsurance(tax.CoveredByInsurance ? tax.Amount : null);
                    lineItem.SetFirm(true);
                    salesOrder.AddLineItem(lineItem);
                }
            }

            return salesOrder;
        }

        private static SalesOrder CreateForAphria(EcomSalesOrder payload, OrderDefaultsSettings orderDefaults, string customerAddressReference)
        {
            var salesOrder = new SalesOrder();
            salesOrder.SetDivision(orderDefaults.Medical.Division);
            salesOrder.SetCustomerReference(payload.ECommOrderNo + orderDefaults.Medical.OrderReferenceSuffix);
            salesOrder.SetECommerceOrderID(payload.ECommOrderID);
            salesOrder.SetCustomer(payload.CustomerAccountNumber);
            salesOrder.SetUpdateOrderIfExists(false);
            salesOrder.SetOrderDate(payload.OrderedOn);
            salesOrder.SetOrderType(orderDefaults.Medical.OrderType);
            salesOrder.SetShippingCarrier(payload.ShippingCarrier == "Canada Post" ? "CANPOST" : payload.ShippingCarrier);
            salesOrder.SetShippingMethod(payload.ShippingMethod == "Expedited Parcel" ? "ExpeditedParcel" : payload.ShippingMethod);
            salesOrder.SetNotes(payload.Notes);
            salesOrder.SetCCOrder(payload.ECommOrderID);

            salesOrder.SetCCPrepayment(CCPrepayment.Create(payload.AmountPaidByCustomer, payload.PrepaymentTransactionID, orderDefaults.Medical.PaymentGateway));

            salesOrder.SetStandardPrepayment(StandardPrepayment.Create(payload.AmountPaidByBillTo, payload.CustomerAccountNumber));

            var address = Address.Create(AddressReference.Create(customerAddressReference));
            var customerAddresses = CustomerAddresses.Create();
            customerAddresses.SetAcknowledgement(address);
            customerAddresses.SetShipTo(address);
            customerAddresses.SetInstallation(address);
            customerAddresses.SetBillTo(address);
            salesOrder.SetCustomerAddresses(customerAddresses);

            foreach (var item in payload.OrderLines)
            {
                var lineItem = LineItem.Create();
                lineItem.SetItemNumber(item.Product);
                lineItem.SetQuantity(item.Quantity);
                lineItem.SetUnitPrice(item.UnitPrice);
                lineItem.SetRequiredLotToPick(item.Lot ?? string.Empty);
                lineItem.SetAmountCoveredByInsurance(payload.ShippingCoveredByInsurance ? payload.ShippingCost : null);
                lineItem.SetGramsCoveredByInsurance(item.GramsCoveredByInsurance > 0 ? item.GramsCoveredByInsurance : null);
                lineItem.SetFirm(true);
                salesOrder.AddLineItem(lineItem);

                if (payload.ShippingCost != 0)
                {
                    lineItem = LineItem.Create();
                    lineItem.SetItemNumber(item.Product);
                    lineItem.SetQuantity(1);
                    lineItem.SetUnitPrice(item.UnitPrice);
                    lineItem.SetRequiredLotToPick(item.Lot ?? string.Empty);
                    lineItem.SetAmountCoveredByInsurance(payload.ShippingCoveredByInsurance ? payload.ShippingCost : null);
                    lineItem.SetGramsCoveredByInsurance(null);
                    lineItem.SetFirm(true);
                    salesOrder.AddLineItem(lineItem);
                }
                if (payload.DiscountAmount != 0)
                {
                    lineItem = LineItem.Create();
                    lineItem.SetItemNumber(payload.PatientType == "Veteran" ? orderDefaults.Medical.Items.DiscountVeteran : orderDefaults.Medical.Items.DiscountCivilian);
                    lineItem.SetQuantity(1);
                    lineItem.SetUnitPrice(-payload.DiscountAmount);
                    lineItem.SetRequiredLotToPick(item.Lot ?? string.Empty);
                    lineItem.SetAmountCoveredByInsurance(payload.AmountPaidByBillTo != null && payload.AmountPaidByCustomer == 0 ? -payload.DiscountAmount : null);
                    lineItem.SetGramsCoveredByInsurance(null);
                    lineItem.SetFirm(true);
                    salesOrder.AddLineItem(lineItem);
                }
            }

            if (payload.Taxes.Any())
            {
                foreach (var tax in payload.Taxes)
                {
                    var lineItem = LineItem.Create();
                    lineItem.SetItemNumber(GetTaxCode(orderDefaults, payload.ShipToState, tax));
                    lineItem.SetQuantity(1);
                    lineItem.SetUnitPrice(tax.Amount);
                    lineItem.SetAmountCoveredByInsurance(tax.CoveredByInsurance ? tax.Amount : null);
                    lineItem.SetFirm(true);
                    salesOrder.AddLineItem(lineItem);
                }
            }

            return salesOrder;
        }


        public void SetDivision(string division) => Division = division;
        public void SetCustomerReference(string customerReference) => CustomerReference = customerReference;
        public void SetECommerceOrderID(string eCommerceOrderID) => ECommerceOrderID = eCommerceOrderID;
        public void SetCustomer(string customer) => Customer = customer;
        public void SetUpdateOrderIfExists(bool updateOrderIfExists) => UpdateOrderIfExists = updateOrderIfExists;
        public void SetOrderDate(DateTime orderDate) => OrderDate = orderDate;
        public void SetOrderReceivedDate(DateTime orderReceivedDate) => OrderReceivedDate = orderReceivedDate;
        public void SetOrderType(string orderType) => OrderType = orderType;
        public void SetShippingCarrier(string shippingCarrier) => ShippingCarrier = shippingCarrier;
        public void SetShippingMethod(string shippingMethod) => ShippingMethod = shippingMethod;
        public void SetShipDate(DateTime shipDate) => ShipDate = shipDate;
        public void SetTaxExempt(bool taxExempt) => TaxExempt = taxExempt;
        public void SetNotes(string notes) => Notes = notes;
        public void SetCustomerPO(string customerPO) => CustomerPO = customerPO;
        public void SetCurrencyIsoCode(string currencyIsoCode) => CurrencyIsoCode = currencyIsoCode;
        public void SetCCOrder(string ccOrder) => CCOrder = ccOrder;
        public void SetShipTo(string shipTo) => ShipTo = shipTo;
        public void SetCardCode(string cardCode) => CardCode = cardCode;
        public void SetCCPrepayment(CCPrepayment ccPrepayment) => CCPrepayment = ccPrepayment;
        public void SetStandardPrepayment(StandardPrepayment standardPrepayment) => StandardPrepayment = standardPrepayment;
        public void SetCustomerAddresses(CustomerAddresses customerAddresses) => CustomerAddresses = customerAddresses;
        public void AddLineItem(LineItem lineItem) => LineItems.Add(lineItem);

        private static string? GetTaxCode(OrderDefaultsSettings orderDefaults, string shipToState, Tax tax)
        {
            List<string> provinces = ["BC", "MB", "SK"];
            if (tax.TaxType != "PST")
            {
                return typeof(MedicalTaxCodesDefaults).GetProperties().FirstOrDefault(p => p.Name.Contains(tax.TaxType)).GetValue(orderDefaults.Medical.TaxCodes).ToString();
            }
            else if (tax.TaxType == "PST" && provinces.Contains(shipToState))
            {
                return $"PST{shipToState}" switch
                {
                    "PSTBC" => orderDefaults.Medical.TaxCodes.PSTBC,
                    "PSTMB" => orderDefaults.Medical.TaxCodes.PSTMB,
                    "PSTSK" => orderDefaults.Medical.TaxCodes.PSTSK,
                    _ => null,
                };
            }
            else
            {
                return null;
            }
        }
    }

    public class CCPrepayment
    {
        public double AmountPrepaidByCC { get; private set; }
        public string PrepaidCCTransactionID { get; private set; }
        public string CCPaymentGateway { get; private set; }

        private CCPrepayment() { }

        private CCPrepayment(double amountPrepaidByCC, string prepaidCCTransactionID, string ccPaymentGateway)
        {
            AmountPrepaidByCC = amountPrepaidByCC;
            PrepaidCCTransactionID = prepaidCCTransactionID;
            CCPaymentGateway = ccPaymentGateway;
        }

        public static CCPrepayment Create(double amountPrepaidByCC, string prepaidCCTransactionID, string ccPaymentGateway)
        {
            return new CCPrepayment(amountPrepaidByCC, prepaidCCTransactionID, ccPaymentGateway);
        }

        public void SetAmountPrepaidByCC(double amountPrepaidByCC) => AmountPrepaidByCC = amountPrepaidByCC;
        public void SetPrepaidCCTransactionID(string prepaidCCTransactionID) => PrepaidCCTransactionID = prepaidCCTransactionID;
        public void SetCCPaymentGateway(string ccPaymentGateway) => CCPaymentGateway = ccPaymentGateway;
    }

    public class StandardPrepayment
    {
        public double AmountPaid { get; private set; }
        public string PrepaymentCustomer { get; private set; }

        private StandardPrepayment() { }

        private StandardPrepayment(double amountPaid, string prepaymentCustomer)
        {
            AmountPaid = amountPaid;
            PrepaymentCustomer = prepaymentCustomer;
        }

        public static StandardPrepayment Create(double amountPaid, string prepaymentCustomer)
        {
            return new StandardPrepayment(amountPaid, prepaymentCustomer);
        }

        public void SetAmountPaid(double amountPaid) => AmountPaid = amountPaid;
        public void SetPrepaymentCustomer(string prepaymentCustomer) => PrepaymentCustomer = prepaymentCustomer;
    }

    public class CustomerAddresses
    {
        public Address Acknowledgement { get; private set; }
        public Address ShipTo { get; private set; }
        public Address Installation { get; private set; }
        public Address BillTo { get; private set; }

        private CustomerAddresses() { }

        public static CustomerAddresses Create()
        {
            return new CustomerAddresses();
        }

        public void SetAcknowledgement(Address acknowledgement) => Acknowledgement = acknowledgement;
        public void SetShipTo(Address shipTo) => ShipTo = shipTo;
        public void SetInstallation(Address installation) => Installation = installation;
        public void SetBillTo(Address billTo) => BillTo = billTo;
    }

    public class Address
    {
        public AddressReference AddressReference { get; private set; }

        private Address() { }

        private Address(AddressReference addressReference)
        {
            AddressReference = addressReference;
        }

        public static Address Create(AddressReference addressReference)
        {
            return new Address(addressReference);
        }

        public void SetAddressReference(AddressReference addressReference) => AddressReference = addressReference;
    }

    public class AddressReference
    {
        public string Reference { get; private set; }

        private AddressReference() { }

        private AddressReference(string reference)
        {
            Reference = reference;
        }

        public static AddressReference Create(string reference)
        {
            return new AddressReference(reference);
        }

        public void SetReference(string reference) => Reference = reference;
    }

    public class LineItem
    {
        public string ItemNumber { get; private set; }
        public double Quantity { get; private set; }
        public double? UnitPrice { get; private set; }
        public string RequiredLotToPick { get; private set; }
        public double? AmountCoveredByInsurance { get; private set; }
        public double? GramsCoveredByInsurance { get; private set; }
        public bool? Firm { get; private set; }
        public string Location { get; private set; }
        public string Id { get; private set; }

        private LineItem() { }

        public static LineItem Create()
        {
            return new LineItem();
        }

        public void SetItemNumber(string itemNumber) => ItemNumber = itemNumber;
        public void SetQuantity(double quantity) => Quantity = quantity;
        public void SetUnitPrice(double unitPrice) => UnitPrice = unitPrice;
        public void SetRequiredLotToPick(string requiredLotToPick) => RequiredLotToPick = requiredLotToPick;
        public void SetAmountCoveredByInsurance(double? amountCoveredByInsurance) => AmountCoveredByInsurance = amountCoveredByInsurance;
        public void SetGramsCoveredByInsurance(double? gramsCoveredByInsurance) => GramsCoveredByInsurance = gramsCoveredByInsurance;
        public void SetFirm(bool? firm) => Firm = firm;
        public void SetLocation(string location) => Location = location;
        public void SetId(string id) => Id = id;
    }
}