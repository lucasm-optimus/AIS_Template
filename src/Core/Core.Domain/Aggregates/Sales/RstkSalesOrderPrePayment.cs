using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tilray.Integrations.Core.Domain.Aggregates.Sales
{
    public class RstkPrePayment
    {
        private RstkPrePayment() { }

        public ExternalReferenceId rstk__soppy_div__r { get; private set; }
        public string rstk__soppy_type__c { get; private set; }
        public ExternalReferenceId rstk__soppy_order__r { get; private set; }
        public ExternalReferenceId rstk__soppy_custno__r { get; private set; }
        public ExternalReferenceId rstk__soppy_addrseq__r { get; private set; }
        public double rstk__soppy_amount__c { get; private set; }
        public string rstk__soppy_appmethod__c { get; private set; }
        public ExternalReferenceId rstk__soppy_sohdrcust__r { get; private set; }
        public ExternalReferenceId rstk__soppy_ppyacct__r { get; private set; }
        public bool rstk__soppy_cctxn__c { get; private set; }

        public static RstkPrePayment Create()
        {
            return new RstkPrePayment();
        }

        public void SetRstk__soppy_div__r(ExternalReferenceId value) => rstk__soppy_div__r = value;
        public void SetRstk__soppy_type__c(string value) => rstk__soppy_type__c = value;
        public void SetRstk__soppy_order__r(ExternalReferenceId value) => rstk__soppy_order__r = value;
        public void SetRstk__soppy_custno__r(ExternalReferenceId value) => rstk__soppy_custno__r = value;
        public void SetRstk__soppy_addrseq__r(ExternalReferenceId value) => rstk__soppy_addrseq__r = value;
        public void SetRstk__soppy_amount__c(double value) => rstk__soppy_amount__c = value;
        public void SetRstk__soppy_appmethod__c(string value) => rstk__soppy_appmethod__c = value;
        public void SetRstk__soppy_sohdrcust__r(ExternalReferenceId value) => rstk__soppy_sohdrcust__r = value;
        public void SetRstk__soppy_ppyacct__r(ExternalReferenceId value) => rstk__soppy_ppyacct__r = value;
        public void SetRstk__soppy_cctxn__c(bool value) => rstk__soppy_cctxn__c = value;
    }
}
