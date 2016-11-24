using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using BotTest.Infrastructure.Models;
using System.Security.Claims;
using System.Collections.Generic;

namespace BotTest
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        private ApplicationDbContext _context { get; set; }
        public MessagesController()
        {
            _context = new ApplicationDbContext();
        }
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                HttpClient client = new HttpClient();
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                StateClient stateClient = activity.GetStateClient();
                BotData userData = await stateClient.BotState.GetUserDataAsync(activity.ChannelId, activity.From.Id);

                //Activity thereply = activity.CreateReply(activity.From.Name);
                //await connector.Conversations.ReplyToActivityAsync(thereply);
                try
                {


                    var account = new AccountEntity { Name = activity.Text };
                    var replyString = "";
                    var userMessage = activity.Text;
                    var accountManager = new AccountManager();
                    var authUser = await accountManager.CheckValidation(activity.From.Id);
                    //var authUser = await accountManager.CheckValidation("109906983001699");
                    //created data

                    //var transaction = new TransactionState();
                    //userData.SetProperty<TransactionState>("TransactionAsk", transaction);
                    var transaction = userData.GetProperty<TransactionState>("TransactionAsk");
                    if (transaction == null)
                    {
                        transaction = new TransactionState();
                    }
                    if (userMessage.ToLower().Contains("cancel"))
                    {
                        transaction = new TransactionState();
                        replyString = "I have cancelled your transaction, did you need anything else?";
                        userData.SetProperty<TransactionState>("TransactionAsk", transaction);
                        await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
                    }



                    if (authUser != null)
                    {
                        if (userMessage.ToLower().Contains("view"))
                        {

                            //Activity replyToConversation = activity.CreateReply("");
                            //replyToConversation.Recipient = activity.From;
                            //replyToConversation.Type = "message";
                            //replyToConversation.Attachments = new List<Attachment>();
                            //////////////////////

                            //List<CardAction> receiptList = new List<CardAction>();
                            //// List<ReceiptItem> receiptList = new List<ReceiptItem>();


                            //////////////////////////

                            if (userMessage.ToLower().Contains("transactions"))
                            {
                                replyString = "Here are your transactions.";
                                var collection = await accountManager.ViewTransactions(authUser, 5);
                                foreach (TransactionEntity userTransaction in collection)
                                {
                                    
                                    string deposit = "";
                                    if (userTransaction.Deposit) {
                                        deposit = "deposited";
                                    }
                                    else
                                    {
                                        deposit = "withdrawn";
                                    }

                                    replyString += $"\n\n {userTransaction.Date}: ${userTransaction.Amount} was {deposit} \n\n {userTransaction.Description} \n - - - \n";
                                    //CardAction lineItem = new CardAction()
                                    //{
                                    //    Title = $"{userTransaction.Date}: ${userTransaction.Amount} was {deposit} \n\n {userTransaction.Description} ",
                                    //};
                                    //receiptList.Add(lineItem);

                                }

                            }
                            if (userMessage.ToLower().Contains("accounts"))
                            {
                                replyString = "Here are your accounts.";
                                var collection = await accountManager.ViewAccounts(authUser);
                                foreach (AccountEntity userAccount in collection)
                                {
                                    replyString += $"\n\n {userAccount.Name} - Balance: {userAccount.Balance}\n - - - \n";
                                    //ReceiptItem lineItem = new ReceiptItem()
                                    //{
                                    //    Title = $"{userAccount.Name} "
                                    //};
                                    ////receiptList.Add(lineItem);
                                }

                            }
                            if (userMessage.ToLower().Contains("payees"))
                            {
                                replyString = "Here are your payees.";
                                var collection = await accountManager.ViewPayee(authUser);
                                foreach (PayeeEntity userAccount in collection)
                                {
                                    replyString += $"\n\n {userAccount.Name}\n - - - \n";
                                    //ReceiptItem lineItem = new ReceiptItem()
                                    //{
                                    //    Title = $"{userAccount.Name} "
                                    //};
                                    ////receiptList.Add(lineItem);
                                }
                            }

                            //HeroCard plCard = new HeroCard()
                            //{
                            //    Title = "Transactions",
                            //    Buttons = receiptList
                            //};
                            //replyString = "";
                            //Attachment plAttatchment = plCard.ToAttachment();
                            //replyToConversation.Attachments.Add(plAttatchment);

                            //await connector.Conversations.SendToConversationAsync(replyToConversation);

                        }
                        else if (userData.GetProperty<bool>("currencyAsk")) {
                            var inputs = userMessage.Split();
                            if (inputs.Length == 4)
                            {
                                var currencyToConvert = Convert.ToDouble(inputs[0]);
                                string x = await client.GetStringAsync(new Uri("http://api.fixer.io/latest?base="+ inputs[1]));


                                RootObject rootObject = JsonConvert.DeserializeObject<RootObject>(x);
                                string secondCurr = inputs[3];
                                double finalrate = 1;
                                if (secondCurr.ToLower() == "aud")
                                {
                                    finalrate = rootObject.rates.AUD;
                                }else if (secondCurr.ToLower() == "bgn")
                                {
                                    finalrate = rootObject.rates.BGN;
                                }
                                else if (secondCurr.ToLower() == "brl")
                                { finalrate = rootObject.rates.BRL;

                                }
                                else if (secondCurr.ToLower() == "cad")
                                { finalrate = rootObject.rates.CAD;

                                }
                                else if (secondCurr.ToLower() == "chf")
                                { finalrate = rootObject.rates.CHF;

                                }
                                else if (secondCurr.ToLower() == "cny")
                                { finalrate = rootObject.rates.CNY;

                                }
                                else if (secondCurr.ToLower() == "czk")
                                { finalrate = rootObject.rates.CZK;

                                }
                                else if (secondCurr.ToLower() == "dkk")
                                { finalrate = rootObject.rates.DKK;

                                }
                                else if (secondCurr.ToLower() == "gbp")
                                { finalrate = rootObject.rates.GBP;

                                }
                                else if (secondCurr.ToLower() == "hkd")
                                { finalrate = rootObject.rates.HKD;

                                }
                                else if (secondCurr.ToLower() == "hrk")
                                { finalrate = rootObject.rates.HRK;

                                }
                                else if (secondCurr.ToLower() == "huf")
                                { finalrate = rootObject.rates.HUF;

                                }
                                else if (secondCurr.ToLower() == "idr")
                                { finalrate = rootObject.rates.IDR;

                                }
                                else if (secondCurr.ToLower() == "ils")
                                { finalrate = rootObject.rates.ILS;

                                }
                                else if (secondCurr.ToLower() == "inr")
                                { finalrate = rootObject.rates.INR;

                                }
                                else if (secondCurr.ToLower() == "jpy")
                                { finalrate = rootObject.rates.JPY;

                                }
                                else if (secondCurr.ToLower() == "krw")
                                { finalrate = rootObject.rates.KRW;

                                }
                                else if (secondCurr.ToLower() == "mxn")
                                { finalrate = rootObject.rates.MXN;

                                }
                                else if (secondCurr.ToLower() == "myr")
                                { finalrate = rootObject.rates.MYR;

                                }
                                else if (secondCurr.ToLower() == "nok")
                                { finalrate = rootObject.rates.NOK;

                                }
                                else if (secondCurr.ToLower() == "nzd")
                                { finalrate = rootObject.rates.NZD;

                                }
                                else if (secondCurr.ToLower() == "php")
                                { finalrate = rootObject.rates.PHP;

                                }
                                else if (secondCurr.ToLower() == "pln")
                                { finalrate = rootObject.rates.PLN;

                                }
                                else if (secondCurr.ToLower() == "ron")
                                { finalrate = rootObject.rates.RON;

                                }
                                else if (secondCurr.ToLower() == "rub")
                                { finalrate = rootObject.rates.RUB;

                                }
                                else if (secondCurr.ToLower() == "sek")
                                { finalrate = rootObject.rates.SEK;

                                }
                                else if (secondCurr.ToLower() == "sgd")
                                { finalrate = rootObject.rates.SGD;

                                }
                                else if (secondCurr.ToLower() == "thb")
                                { finalrate = rootObject.rates.THB;

                                }
                                else if (secondCurr.ToLower() == "try")
                                { finalrate = rootObject.rates.TRY;

                                }
                                else if (secondCurr.ToLower() == "usd")
                                { finalrate = rootObject.rates.USD;

                                }
                                else if (secondCurr.ToLower() == "zar")
                                { finalrate = rootObject.rates.ZAR;

                                }
                                double finalAmount = currencyToConvert * finalrate;
                                replyString = $"${currencyToConvert} {inputs[1]} will give you ${Math.Round(finalAmount,2)} in {secondCurr}. ";



                            }else
                            {
                                replyString = "I could not convert those currencies";
                            }


                            userData.SetProperty<bool>("currencyAsk", false);
                            await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
                        }
                        else if (userMessage.ToLower().Contains("currency") | userMessage.ToLower().Contains("exchange"))
                        {
                            userData.SetProperty<bool>("currencyAsk", true);
                            await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
                            replyString = "How much and what type of currency do you want to exchange?";

                        }
                        else if (transaction.finalisedBool)
                        {
                            if (userMessage.ToLower().Contains("confirm"))
                            {
                                await accountManager.ProcessTransaction(transaction);
                                replyString = "Transaction has been processed";

                                transaction = new TransactionState();
                                userData.SetProperty<TransactionState>("TransactionAsk", transaction);
                                await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);

                            }
                        }
                        else if (transaction.descriptionBool)
                        {
                            if (userMessage.ToLower() == "no" | userMessage.ToLower() == "no thanks" | userMessage.ToLower() == "nah" | userMessage.ToLower() == "no thank you")
                            {
                                userMessage = "Sent from Contoso BankBot";
                            }

                            transaction.finalisedBool = true;
                            transaction.descriptionBool = false;
                            transaction.DescriptionInfo = userMessage;
                            userData.SetProperty<TransactionState>("TransactionAsk", transaction);
                            await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);



                            Activity replyToConversation = activity.CreateReply("Does this look correct?");
                            replyToConversation.Recipient = activity.From;
                            replyToConversation.Type = "message";
                            replyToConversation.Attachments = new List<Attachment>();

                            //List<CardImage> cardImages = new List<CardImage>();
                            //cardImages.Add(new CardImage(url: "Contoso.png"));

                            List<CardAction> cardButtons = new List<CardAction>();
                            CardAction confirmButton = new CardAction() {
                                Value = "Confirm",
                                Type = "postBack",
                                Title = "Confirm"
                            };
                            cardButtons.Add(confirmButton);
                            CardAction cancelButton = new CardAction()
                            {
                                Value = "cancel",
                                Type = "postBack",
                                Title = "Cancel"
                            };
                            cardButtons.Add(cancelButton);
                            var recieverAccount = await accountManager.CheckValidAccount(transaction.RecieverAccountId);
                            var senderAccount = await accountManager.CheckValidAccount(transaction.SenderAccountId);

                            string toReply = "";

                            toReply += $"\n\n From: {senderAccount.Name}";
                            toReply += $"\n\n To: {recieverAccount.Name}";
                            toReply += $"\n\n Amount: ${transaction.Amount.ToString()}";
                            toReply += $"\n\n Description: {transaction.DescriptionInfo}";
                            Activity reply = activity.CreateReply(toReply);
                            await connector.Conversations.ReplyToActivityAsync(reply);

                            //ReceiptItem lineItem1 = new ReceiptItem()
                            //{
                            //    Title = "From: ",
                            //    Subtitle = senderAccount.Name
                            //};
                            //ReceiptItem lineItem2 = new ReceiptItem()
                            //{
                            //    Title = "To: ",
                            //    Subtitle = recieverAccount.Name
                            //};
                            //ReceiptItem lineItem3 = new ReceiptItem()
                            //{
                            //    Title = "Amount: ",
                            //    Subtitle = $"${transaction.Amount.ToString()}"
                            //};
                            //ReceiptItem lineItem4 = new ReceiptItem()
                            //{
                            //    Title = "Transaction Description: ",
                            //    Subtitle = transaction.DescriptionInfo
                            //};
                            //List<ReceiptItem> receiptList = new List<ReceiptItem>();

                            //receiptList.Add(lineItem1);
                            //receiptList.Add(lineItem2);
                            //receiptList.Add(lineItem3);
                            //receiptList.Add(lineItem4);

                            HeroCard plCard = new HeroCard()
                            {
                                Buttons = cardButtons
                            };


                            replyString = "";
                            Attachment plAttatchment = plCard.ToAttachment();
                            replyToConversation.Attachments.Add(plAttatchment);


                            await connector.Conversations.SendToConversationAsync(replyToConversation);
                            //    //Means we have asked if they want a description
                            //    //if they say no create a default transaction
                            //    //if they have implimented a description then throw that in there
                            //    //the calculation is then done subtracting from reciever and sending to sender, creating a transaction and whatnot
                        }

                        else if (transaction.amountBool)
                        {
                            account = await accountManager.CheckValidAccount(transaction.SenderAccountId);
                            bool fail = false;
                            double userAmount = 0;
                            try
                            {
                                userAmount = Convert.ToDouble(userMessage);
                            }
                            catch
                            {
                                fail = true;
                                replyString = "I can only deal with numbers at the moment. Could you try again with a valid number";
                            }
                            if (fail == false)
                            {
                                if (userAmount > account.Balance)
                                {
                                    replyString = $"Sorry, you don't have enough money to make that transaction. you only have ${account.Balance}. Please try again.";
                                } else
                                {
                                    replyString = "Okay awesome, what kind of description do you want? If you don't want one just say no and I can make one for you";

                                    transaction.amountBool = false;
                                    transaction.descriptionBool = true;
                                    transaction.Amount = userAmount;
                                    userData.SetProperty<TransactionState>("TransactionAsk", transaction);
                                    await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);

                                }
                            }
                            //    //Means we have asked how much they want to transfer
                            //    //If user selects a valid amount then ask if the user wants to add a description
                            //    //
                        }
                        else if (transaction.senderBool)
                        {
                            var validAccount = await accountManager.CheckAccount(authUser, userMessage);
                            if (validAccount != null)
                            {

                                replyString = "Awesome, how much would you like to transfer?";

                                transaction.senderBool = false;
                                transaction.amountBool = true;
                                transaction.SenderAccountId = validAccount.Id;
                                userData.SetProperty<TransactionState>("TransactionAsk", transaction);
                                await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
                            } else
                            {
                                replyString = "Sorry something went wrong, please cancel and try again";
                            }
                            //if(sender == true)
                            //{
                            //    //means we have asked what account they want to use
                            //    //If user selects valid account then ask the amount to transfer  (Maybe put in how much they currently have)
                            //    //If not valid then same as above
                        }

                        else if (transaction.recieverBool)
                        {
                            //Means we have asked for account.
                            //If user gives us valid account then we should ask for what account they want to use
                            //if user does not give us a valid account then either ask again or terminate??
                            var possiblePayee = await accountManager.CheckValidPayee(authUser, userMessage);
                            if (possiblePayee != null) {
                                userMessage = possiblePayee.AccountNumber;
                            }

                            var validAccount = await accountManager.CheckValidAccount(userMessage);
                            if (validAccount != null)
                            {

                                Activity cardReply = activity.CreateReply("What account would you like to use?");
                                cardReply.Recipient = activity.From;
                                cardReply.Type = "message";
                                cardReply.Attachments = new List<Attachment>();

                                List<CardAction> actions = new List<CardAction>();

                                replyString = "";
                                var list = _context.Accounts.Where(x => x.UserId == authUser.Id);

                                foreach (AccountEntity blah in list)
                                {
                                    CardAction plButton = new CardAction()
                                    {
                                        Type = "postBack",
                                        Title = $"${blah.Balance} | {blah.Name}   ",
                                        Value = blah.Name
                                    };
                                    actions.Add(plButton);

                                }

                                HeroCard plcard = new HeroCard()
                                {
                                    Buttons = actions
                                };

                                Attachment plAttatchment = plcard.ToAttachment();
                                cardReply.Attachments.Add(plAttatchment);
                                await connector.Conversations.SendToConversationAsync(cardReply);

                                transaction.senderBool = true;
                                transaction.recieverBool = false;
                                transaction.RecieverAccountId = validAccount.Id;
                                userData.SetProperty<TransactionState>("TransactionAsk", transaction);
                                await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);

                            }
                            else
                            {
                                replyString = "Sorry, that is not a valid account";
                            }
                        }


                        else if (userMessage.Contains("create account"))
                        {
                            var newAccount = accountManager.CreateAccount(authUser, "savings");
                            if (newAccount != null)
                            {
                                Activity creationReply = activity.CreateReply($"Account with number { newAccount.Name} has been created to the user {authUser.Name}");
                                await connector.Conversations.ReplyToActivityAsync(creationReply);
                            }
                            else
                            {
                                Activity creationReply = activity.CreateReply("Something went wrong when creating the account");
                                await connector.Conversations.ReplyToActivityAsync(creationReply);
                            }
                        }

                        else if (userMessage.ToLower().Contains("hello") | userMessage.ToLower().Contains("hey"))
                        {
                            if (userData.GetProperty<bool>("greeted"))
                            {
                                replyString = "Hello there again, What can I do for you this time?";
                            }
                            else
                            {
                                replyString = "Hello, Welcome to the Contoso Bank. How can I help you?";
                                userData.SetProperty<bool>("greeted", true);
                                await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
                            }
                        }
                        else if (userMessage.Contains("make a transaction"))
                        {

                            transaction = new TransactionState { recieverBool = true };
                            replyString = "Please enter an account or payee to make a transaction to";
                            if (userMessage.Length >= 22)
                            {
                                if (userMessage.Contains("make a transaction to "))
                                {
                                    var array = userMessage.Substring(22).Split();
                                    if (array.Length >= 1)
                                    {
                                        var possiblePayee = await accountManager.CheckValidPayee(authUser, array[0]);

                                        if (possiblePayee != null)
                                        {
                                            var possibleAccount = await accountManager.CheckValidAccount(possiblePayee.AccountNumber);
                                            if (possibleAccount != null) {
                                                transaction.recieverBool = true;
                                                transaction.RecieverAccountId = possibleAccount.Id;
                                                replyString = "Please press enter ";
                                            } else
                                            {
                                                replyString = "Sorry, I couldn't find that payees account. Please enter a valid account number";
                                            }
                                            //do it with the users input
                                        }
                                        else
                                        {
                                            var possibleAccount = await accountManager.CheckValidAccount(possiblePayee.AccountNumber);
                                            if (possibleAccount != null)
                                            {
                                                transaction.recieverBool = false;
                                                transaction.senderBool = true;
                                                transaction.RecieverAccountId = possibleAccount.Id;
                                            }
                                            else
                                            {
                                                replyString = "Sorry, I couldn't find that account. Please enter a valid account number";
                                            }
                                            //do it with possiblepayee.accountname t
                                            //possiblePayee.ac
                                        }
                                    }
                                }
                            }

                            userData.SetProperty<TransactionState>("TransactionAsk", transaction);
                            await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
                        }


                        if (userMessage.Length >= 12)
                        {
                            if (userMessage.Substring(0, 12).ToLower().Equals("set a payee "))
                            {
                                var array = userMessage.Substring(12).Split();
                                if (array.Length == 0)
                                {
                                    replyString = "Please enter a name and account number";

                                }
                                else if (array.Length == 1)
                                {
                                    if (array[0].Substring(0, 3).Equals("43-"))
                                    {
                                        //Throw in states
                                        replyString = "Please enter a name and account number";
                                    }
                                    else
                                    {
                                        replyString = "Please enter a name and account number";

                                    };
                                }
                                else if (array.Length >= 2)
                                {
                                    var payee = await accountManager.CreatePayee(authUser, array[0], array[1]);
                                    replyString = $"Payee {payee.Name} was succesfully saved";
                                }
                            }
                        }
                        if (userMessage.Length >= 13)
                        {
                            if (userMessage.Substring(0, 13).ToLower().Equals("remove payee "))
                            {
                                var array = userMessage.Substring(13).Split();
                                if (array.Length == 0)
                                {
                                    replyString = "Please specify also specify a payee to remove";
                                }
                                else if (array.Length >= 1)
                                {
                                    var removedPayee = await accountManager.RemovePayee(authUser, array[0]);
                                    if (removedPayee != null)
                                    {
                                        replyString = $"Payee {removedPayee.Name} has been removed.";
                                    }
                                    else
                                    {
                                        replyString = $"Payee with the name {array[0]} could not be found.";
                                    }
                                }
                            }
                        }
                    }
                    //--------------------
                    else
                    {

                        if (userData.GetProperty<bool>("AskedAccount"))
                        {
                            if (userMessage.ToLower().Contains("ye"))
                            {
                                await accountManager.CreateUser(activity.From.Name, activity.From.Id);
                                //await accountManager.CreateUser("Robert Bryson Hall", "109906983001699");
                                userData.SetProperty<bool>("AskedAccount", false);
                                await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
                                replyString = "Okay cool, we've got you in the system now. What do you want to do?";
                            }
                            else if (userMessage.ToLower().Contains("no"))
                            {
                                replyString = "Sorry, but you are unable to access our services without an account";
                                userData.SetProperty<bool>("AskedAccount", false);
                                await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
                            }
                            else
                            {
                                replyString = "Sorry, I didn't catch that";
                            }
                        }
                        else
                        {
                            replyString = "You are not authorized to the bank, would you like to create an account?";
                            userData.SetProperty<bool>("AskedAccount", true);
                            await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
                        }

                    }

                    if (replyString != "")
                    {
                        Activity reply = activity.CreateReply(replyString);
                        await connector.Conversations.ReplyToActivityAsync(reply);
                    }
                    else
                    {
                        Activity reply = activity.CreateReply("Something went wrong, could you try again?");
                        await connector.Conversations.ReplyToActivityAsync(reply);
                    }
                    // calculate something for us to return

                   



                    //await _context.SaveChangesAsync();
                    // return our reply to the user

                    
                }
                catch(Exception e)
                {
                    Activity reply = activity.CreateReply(e.Message);
                    await connector.Conversations.ReplyToActivityAsync(reply);
                }
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}