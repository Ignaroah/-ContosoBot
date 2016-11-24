using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BotTest.Infrastructure.Models
{
    public class UserEntity:Entity
    {
        public string Name { get; set; }
        public ICollection<AccountEntity> Accounts { get; set; }

        public ICollection<PayeeEntity> Payees { get; set; }
        public string providerId { get; set; }
        public string Phone { get;set;}
        public string Email { get; set; }
        public string Address { get; set; }
    }
}