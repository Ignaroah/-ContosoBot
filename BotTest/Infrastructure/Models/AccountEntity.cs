using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BotTest.Infrastructure.Models
{
    public class AccountEntity:Entity
    {
        
        public string Name { get; set; }
        public string Type { get; set; }
        public double Balance { get; set; }
        public ICollection<TransactionEntity> Transaction { get; set; }
        public int UserId { get; set; }
        public UserEntity User { get; set; }
    }
}