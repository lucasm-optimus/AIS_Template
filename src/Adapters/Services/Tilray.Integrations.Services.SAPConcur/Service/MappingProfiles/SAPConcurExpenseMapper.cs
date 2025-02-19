using AutoMapper;
using Tilray.Integrations.Core.Common.Extensions;
using Tilray.Integrations.Core.Domain.Aggregates.Expenses;

namespace Tilray.Integrations.Services.SAPConcur.Service.MappingProfiles;

public class SAPConcurExpenseMapper : Profile
{
    public SAPConcurExpenseMapper()
    {
        CreateMap<string[], Expense>()
            .ForMember(dest => dest.LineType, opt => opt.MapFrom(src => src.ElementAtOrDefault(0) ?? ""))
            .ForMember(dest => dest.BatchID, opt => opt.MapFrom(src => src.ElementAtOrDefault(1) ?? ""))
            .ForMember(dest => dest.BatchDate, opt => opt.MapFrom(src => Helpers.ParseDate(src.ElementAtOrDefault(2))))
            .ForMember(dest => dest.EmployeeLastName, opt => opt.MapFrom(src => (src.ElementAtOrDefault(5) ?? "").Replace(",", "|")))
            .ForMember(dest => dest.EmployeeFirstName, opt => opt.MapFrom(src => (src.ElementAtOrDefault(6) ?? "").Replace(",", "|")))
            .ForMember(dest => dest.EmployeeGroupID, opt => opt.MapFrom(src => (src.ElementAtOrDefault(8) ?? "").Replace(",", "|")))
            .ForMember(dest => dest.ReportID, opt => opt.MapFrom(src => src.ElementAtOrDefault(18) ?? ""))
            .ForMember(dest => dest.ReportDescription, opt => opt.MapFrom(src => (src.ElementAtOrDefault(26) ?? "").Replace(",", "|")))
            .ForMember(dest => dest.ReportTotalApprovedID, opt => opt.MapFrom(src => src.ElementAtOrDefault(31) ?? ""))
            .ForMember(dest => dest.ReportEntryTransactionDate, opt => opt.MapFrom(src => Helpers.ParseDate(src.ElementAtOrDefault(63))))
            .ForMember(dest => dest.ReportEntryCurrencyAlphaCode, opt => opt.MapFrom(src => src.ElementAtOrDefault(64) ?? ""))
            .ForMember(dest => dest.ReportEntryDescription, opt => opt.MapFrom(src => (src.ElementAtOrDefault(68) ?? "").Replace(",", "|")))
            .ForMember(dest => dest.ReportEntryVendorDescription, opt => opt.MapFrom(src => (src.ElementAtOrDefault(70) ?? "").Replace(",", "|")))
            .ForMember(dest => dest.CompanyCode, opt => opt.MapFrom(src => "'" + (src.ElementAtOrDefault(82) ?? "")))
            .ForMember(dest => dest.ERP, opt => opt.MapFrom(src => "'" + (src.ElementAtOrDefault(83) ?? "")))
            .ForMember(dest => dest.Department, opt => opt.MapFrom(src => (src.ElementAtOrDefault(84) ?? "").Replace(",", "|")))
            .ForMember(dest => dest.ExpenseCode, opt => opt.MapFrom(src => src.ElementAtOrDefault(85) ?? ""))
            .ForMember(dest => dest.ManAttention, opt => opt.MapFrom(src => src.ElementAtOrDefault(86) ?? ""))
            .ForMember(dest => dest.Region, opt => opt.MapFrom(src => src.ElementAtOrDefault(87) ?? ""))
            .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.ElementAtOrDefault(88) ?? ""))
            .ForMember(dest => dest.Brand, opt => opt.MapFrom(src => src.ElementAtOrDefault(100) ?? ""))
            .ForMember(dest => dest.Facility, opt => opt.MapFrom(src => src.ElementAtOrDefault(101) ?? ""))
            .ForMember(dest => dest.PaymentTypeCode, opt => opt.MapFrom(src => (src.ElementAtOrDefault(125) ?? "").Replace(",", "|")))
            .ForMember(dest => dest.PaymentCode, opt => opt.MapFrom(src => (src.ElementAtOrDefault(126) ?? "").Replace(",", "|")))
            .ForMember(dest => dest.ReportEntryLocationCountryCode, opt => opt.MapFrom(src => src.ElementAtOrDefault(157) ?? ""))
            .ForMember(dest => dest.ReportEntryLocationCountrySubCode, opt => opt.MapFrom(src => src.ElementAtOrDefault(158) ?? ""))
            .ForMember(dest => dest.JournalPayerPaymentTypeName, opt => opt.MapFrom(src => src.ElementAtOrDefault(162) ?? ""))
            .ForMember(dest => dest.JournalDebitOrCredit, opt => opt.MapFrom(src => src.ElementAtOrDefault(167) ?? ""))
            .ForMember(dest => dest.JournalAmount, opt => opt.MapFrom(src => Helpers.ParseDecimal(src.ElementAtOrDefault(168))))
            .ForMember(dest => dest.JournalNetOfTotalAdjustedReclaimTax, opt => opt.MapFrom(src => Helpers.ParseDecimal(src.ElementAtOrDefault(17))))
            .ForMember(dest => dest.CreditCardTransactionAmount, opt => opt.MapFrom(src => Helpers.ParseDecimal(src.ElementAtOrDefault(136))))
            .ForMember(dest => dest.CreditCardTransactionPostedAmount, opt => opt.MapFrom(src => Helpers.ParseDecimal(src.ElementAtOrDefault(139))))
            .ForMember(dest => dest.TaxAuthorityName, opt => opt.MapFrom(src => (src.ElementAtOrDefault(224) ?? "").Replace(",", "|")))
            .ForMember(dest => dest.TaxAuthorityLabel, opt => opt.MapFrom(src => (src.ElementAtOrDefault(225) ?? "").Replace(",", "|")))
            .ForMember(dest => dest.ReportEntryTaxTransactionAmount, opt => opt.MapFrom(src => Helpers.ParseDecimal(src.ElementAtOrDefault(226))))
            .ForMember(dest => dest.ReportEntryTaxReclaimTransactionAmount, opt => opt.MapFrom(src => Helpers.ParseDecimal(src.ElementAtOrDefault(229))));
    }
}
