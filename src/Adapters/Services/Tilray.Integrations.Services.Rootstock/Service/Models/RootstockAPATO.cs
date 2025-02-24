namespace Tilray.Integrations.Services.Rootstock.Service.Models
{
    public class RootstockAPATO
    {
        public string? GLAccount { get; private set; }
        public string? DocumentNumber { get; private set; }
        public string? TransactionDate { get; private set; }
        public string? Description { get; private set; }
        public double? Quantity { get; private set; }
        public double? LineTotal { get; private set; }
        public double? DistributionTotal { get; private set; }
        public double? TransactionTotal { get; private set; }
        public string? TransactionType { get; private set; }
        public string? PayType { get; private set; }
        public string? Status { get; private set; }
        public string? CompanyNumber { get; private set; }
        public string? Vendor { get; private set; }
        public string? TransactionSet { get; private set; }
        public string? LineSet { get; private set; }
        public string? ErrorMessage { get; private set; }

        private RootstockAPATO() { }

        public static RootstockAPATO Create(APATOLineItem item, string glAccountId, string companyId, string vendorId)
        {
            return new RootstockAPATO
            {
                GLAccount = glAccountId,
                DocumentNumber = item.DocumentNumber,
                TransactionDate = item.TransactionDate,
                Description = item.Description.Length > 100 ? item.Description.Replace("\"", "inch").Substring(0, 100) : item.Description.Replace("\"", "inch"),
                Quantity = item.Quantity,
                LineTotal = item.LineTotal,
                DistributionTotal = item.LineTotal,
                TransactionTotal = item.TransactionTotal,
                TransactionType = item.TransactionType,
                PayType = item.PayType,
                Status = item.Status,
                CompanyNumber = companyId,
                Vendor = vendorId,
                TransactionSet = item.TransactionSet.Length > 20 ? item.TransactionSet.Substring(0, 20) : item.TransactionSet,
                LineSet = item.LineSet
            };
        }

        public void UpdateErrorMessage(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }
    }
}
