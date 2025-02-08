using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tilray.Integrations.Core.Domain.Aggregates.Sales
{
    public class StandardPrepayment : Entity
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
    }
}
