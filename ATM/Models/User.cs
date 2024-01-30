using System;
using System.Collections.Generic;

namespace ATM.Models
{
    public partial class User
    {
        public User()
        {
            Transactions = new HashSet<Transaction>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string CardNumber { get; set; } = null!;
        public string Pin { get; set; } = null!;
        public bool Enabled { get; set; }
        public byte? WrongPin { get; set; }

        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
