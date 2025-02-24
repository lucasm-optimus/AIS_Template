namespace Tilray.Integrations.Core.Domain.Aggregates.Invoices
{
    public class GLAccountsSettings
    {
        public GLAccountA1 A1 { get; set; }
        public GLAccountSWB SWB { get; set; }
    }

    public class GLAccountA1
    {
        public string AP { get; set; }
        public string Freight { get; set; }
        public string GST { get; set; }
        public string QST { get; set; }
    }

    public class GLAccountSWB
    {
        public string Freight { get; set; }
        public string Tax { get; set; }
    }
}
