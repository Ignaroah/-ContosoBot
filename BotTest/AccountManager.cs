using BotTest.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BotTest
{
    public class AccountManager
    {
        private ApplicationDbContext _context { get; set; }
        public AccountManager()
        {
            _context = new ApplicationDbContext();
        }

        public async Task<UserEntity> CheckValidation(string identfier)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.providerId == identfier);
        }

        public async Task<AccountEntity> CheckAccount(UserEntity user, string accountName)
        {
            return await _context.Accounts.Where(x =>x.UserId == user.Id).FirstOrDefaultAsync(x => x.Name == accountName);
        }

        public async Task<AccountEntity> CheckValidAccount(string accountName)
        {
            return await _context.Accounts.FirstOrDefaultAsync(x => x.Name == accountName);
        }
        public async Task<AccountEntity> CheckValidAccount(int accountId)
        {
            return await _context.Accounts.FirstOrDefaultAsync(x => x.Id == accountId);
        }


        public async Task CreateUser(string name, string identifier)
        {
            var newUser = new UserEntity { Name = name, providerId = identifier };
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

        }

        public AccountEntity CreateAccount(UserEntity user, string type)
        {
            bool validValue = false;
            string accountNumber = "43-5632-";
            string accountType = "-00";
            string newAcc = "";
            //savings is '-01'; checking is "-01'
            try
            {
                if (type == "checking")
                {
                    accountType = "-01";
                }

                while (!validValue)
                {
                    Random r = new Random();
                    int rInt = r.Next(0, 9999999);
                    string body = rInt.ToString("0000000");
                    newAcc = accountNumber + body + accountType;
                    var shouldBeNull = _context.Accounts.FirstOrDefault(x => x.Name == newAcc);
                    if (shouldBeNull == null)
                    {
                        validValue = true;
                    }
                }
                var account = new AccountEntity
                {
                    Name = newAcc,
                    Type = type,
                    Balance = 0.0,
                    User = user,
                    UserId = user.Id
                };
                _context.Accounts.Add(account);
                _context.SaveChangesAsync();
                return account;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<PayeeEntity> CreatePayee(UserEntity user, string name, string accountNumber)
        {
            var newPayee = new PayeeEntity
            {
                Name = name,
                AccountNumber = accountNumber,
                User = user,
                UserId = user.Id
            };
            _context.Payees.Add(newPayee);
            await _context.SaveChangesAsync();
            return newPayee;
        }

        public async Task<PayeeEntity> RemovePayee(UserEntity user, string name)
        {
            var payeeToRemove = await _context.Payees.FirstOrDefaultAsync(x => x.Name.ToLower() == name.ToLower());
            if (payeeToRemove != null)
            {
                _context.Payees.Remove(payeeToRemove);
                await _context.SaveChangesAsync();
            }
            return payeeToRemove;
        }

        public async Task<PayeeEntity> CheckValidPayee(UserEntity user, string payeeName)
        {
            return await _context.Payees.Where(x => x.UserId == user.Id).FirstOrDefaultAsync(x => x.Name.ToLower() == payeeName.ToLower());
        }

        public async Task<TransactionEntity> ProcessTransaction(TransactionState transaction)
        {

            var sender = _context.Accounts.Where(x => x.Id == transaction.SenderAccountId).FirstOrDefault();
            var receiver = _context.Accounts.Where(x => x.Id == transaction.RecieverAccountId).FirstOrDefault(); 

            sender.Balance = sender.Balance - transaction.Amount;
            receiver.Balance = receiver.Balance + transaction.Amount;
            
            var senderTransaction =  new TransactionEntity
            {
                Description = transaction.DescriptionInfo,
                AccountId = sender.Id,
                OwnerAccount = sender,
                TransactionAccountId = receiver.Id,
                Date = DateTime.Now,
                Deposit = true,
                Amount = transaction.Amount,
            };
            var recieverTransaction = new TransactionEntity
            {
                Description = transaction.DescriptionInfo,
                AccountId = receiver.Id,
                OwnerAccount = receiver,
                TransactionAccountId = sender.Id,
                Date = DateTime.Now,
                Deposit = false,
                Amount = transaction.Amount,
            };
            _context.Transactions.Add(senderTransaction);
            _context.Transactions.Add(recieverTransaction);
            await _context.SaveChangesAsync();
            return senderTransaction;

        }

        public async Task<ICollection<TransactionEntity>> ViewTransactions(UserEntity user, int range)
        {
            return await _context.Transactions.Where(x => x.OwnerAccount.UserId == user.Id)
                .OrderByDescending(t => t.Date)
                .Take(range)
                .ToListAsync();
            

            }
        public async Task<ICollection<AccountEntity>> ViewAccounts(UserEntity user)
        {
            return await _context.Accounts.Where(x => x.UserId == user.Id).ToListAsync();
        }
        public async Task<ICollection<PayeeEntity>> ViewPayee(UserEntity user)
        {
            return await _context.Payees.Where(x => x.UserId == user.Id).ToListAsync();
        }

        //public async Task<string> MakeTransaction(UserEntity user, string targetAccount)
        //{
        //    //check if both accounts exist
        //}
    }
}