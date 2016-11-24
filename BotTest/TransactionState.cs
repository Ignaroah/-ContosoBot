using BotTest.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BotTest
{
    public class TransactionState
    {
        public bool senderBool { get; set; }
        public bool recieverBool { get; set; }
        public bool amountBool { get; set; }
        public bool descriptionBool { get; set; }

        public bool finalisedBool { get; set; }

        public int RecieverAccountId { get; set; }
        public int SenderAccountId { get; set; }
        public string DescriptionInfo { get; set; }
        public double Amount { get; set; }

    }
}