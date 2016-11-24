using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BotTest.Infrastructure.Models
{
    public class TransactionEntity:Entity
    {
        public string Description { get; set; }
        public int AccountId { get; set; }
        public AccountEntity OwnerAccount { get; set; }
        public int TransactionAccountId { get; set; }
        public DateTime Date { get; set; }
        public bool Deposit { get; set; }
        public double Amount { get; set; }

        

    }
}