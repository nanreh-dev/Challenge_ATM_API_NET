using System;
using System.Collections.Generic;

namespace ATM.Models
{
    public partial class TransactionType
    {
        public TransactionType()
        {
            Transactions = new HashSet<Transaction>();
        }

        public byte IdType { get; set; }
        public string Description { get; set; } = null!;

        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
