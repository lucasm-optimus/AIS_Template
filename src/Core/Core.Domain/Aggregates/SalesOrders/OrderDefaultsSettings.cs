using System;
using System.Collections.Generic;

namespace Tilray.Integrations.Core.Domain.Aggregates.Sales
{
    public class OrderDefaultsSettings
    {
        public MedicalOrderDefaults Medical { get; set; }
        public SweetWaterOrderDefaults SweetWater { get; set; }
    }

    public class MedicalOrderDefaults
    {
        public string Division { get; set; }
        public string OrderReferenceSuffix { get; set; }
        public string OrderType { get; set; }
        public string PaymentGateway { get; set; }
        public MedicalCustomerDefaults Customer { get; set; }
        public MedicalItemsDefaults Items { get; set; }
        public MedicalTaxCodesDefaults TaxCodes { get; set; }
    }

    public class MedicalCustomerDefaults
    {
        public string AccountingDimension1Civilian { get; set; }
        public string AccountingDimension1Veteran { get; set; }
        public string AccountingDimension2Suffix { get; set; }
        public string CustomerClass { get; set; }
        public string PaymentTerms { get; set; }
        public string AddressReferenceSuffix { get; set; }
    }

    public class MedicalItemsDefaults
    {
        public string Shipping { get; set; }
        public string DiscountCivilian { get; set; }
        public string DiscountVeteran { get; set; }
    }

    public class MedicalTaxCodesDefaults
    {
        public string GST { get; set; }
        public string HST { get; set; }
        public string QST { get; set; }
        public string PSTBC { get; set; }
        public string PSTMB { get; set; }
        public string PSTSK { get; set; }
    }

    public class SweetWaterOrderDefaults
    {
        public string Division { get; set; }
        public string OrderReferenceSuffix { get; set; }
        public string OrderType { get; set; }
        public SweetWaterCustomerDefaults Customer { get; set; }
        public SweetWaterItemsDefaults Items { get; set; }
    }

    public class A1
    {
        public string Division { get; set; }
    }

    public class SweetWaterCustomerDefaults
    {
        public string AccountingDimension1 { get; set; }
        public string AccountingDimension2Suffix { get; set; }
        public string CustomerClass { get; set; }
        public string PaymentTerms { get; set; }
        public string AddressReferenceSuffix { get; set; }
    }

    public class SweetWaterItemsDefaults
    {
        public string Shipping { get; set; }
        public string Discount { get; set; }
    }
}
