using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BotTest.Infrastructure.Models
{
    public class ApplicationDbContext:DbContext
    {
        public DbSet<AccountEntity> Accounts { get; set; }
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<PayeeEntity> Payees { get; set; }
        public DbSet<TransactionEntity> Transactions { get; set; }

        public ApplicationDbContext():base("DefaultConnection")
        {

        }
    }
}