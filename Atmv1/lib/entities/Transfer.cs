using System;
using System.Collections.Generic;
using System.Text;

namespace Atmv1.lib.entities
{
    public class Transfer
    {
        public decimal TransferAmount { get; set; }
        public long RecipientBankAccountNumber { get; set; }
        public string RecipientBankAccountName { get; set; }
    }
}
