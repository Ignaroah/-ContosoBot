using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BotTest.Infrastructure.Models
{
    public class PayeeEntity:Entity
    {
        public string Name { get; set; }
        public string AccountNumber { get; set; }
        public int UserId { get; set; }
        public UserEntity User { get; set; }
    }
}