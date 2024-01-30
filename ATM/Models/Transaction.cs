using System;
using System.Collections.Generic;

namespace ATM.Models
{
    public partial class Transaction
    {
        public int IdTransaction { get; set; }
        public int UserId { get; set; }
        public DateTime Date { get; set; }
        public byte Type { get; set; }
        public decimal Amount { get; set; }

        public virtual TransactionType TypeNavigation { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
