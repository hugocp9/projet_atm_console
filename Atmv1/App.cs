using Atmv1.lib.utils;
using Atmv1.lib.entities;
using Atmv1.lib.interfaces;
using Atmv1.lib.enums;
using ConsoleTables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Atmv1
{
    public class App : IApp, ITransaction, IUserBankAccount
    {
        // This class in charge of main application where by Initialization and Execute 
        // method will be the only methods to be called when client code run this application.

        // This list is used in replace of database.
        private List<UserBankAccount> _accountList;
        private UserBankAccount selectedAccount;
        private const decimal minimum_kept_amt = 20;
        private List<Transaction> _listOfTransactions;
        private readonly Screen screen;

        public App()
        {
            screen = new Screen();
        }

        public void Initialization()
        {
            _accountList = new List<UserBankAccount>
            {
                new UserBankAccount() { Id=1, FullName = "Homer", AccountNumber=333111, CardNumber = 111111, CardPin = 111111, AccountBalance = 2000.00m, IsLocked = false },
                new UserBankAccount() { Id=2, FullName = "Marge", AccountNumber=111222, CardNumber = 456456, CardPin = 222222, AccountBalance = 1500.30m, IsLocked = true },
                new UserBankAccount() { Id=3, FullName = "Bart", AccountNumber=888555, CardNumber = 789789, CardPin = 333333, AccountBalance = 2900.12m, IsLocked = false }
            };

            _listOfTransactions = new List<Transaction>();
        }

        public void Execute()
        {
            Screen.WelcomeATM();

            CheckCardNoPassword();
            Screen.WelcomeCustomer(selectedAccount.FullName);

            while (true)
            {
                Screen.DisplaySecureMenu();
                ProcessMenuOption();
            }
        }

        public void CheckCardNoPassword()
        {
            bool isLoginPassed = false;

            while (isLoginPassed == false)
            {
                var inputAccount = screen.LoginForm();

                Screen.LoginProgress();

                foreach (UserBankAccount account in _accountList)
                {
                    selectedAccount = account;
                    if (inputAccount.CardNumber.Equals(account.CardNumber))
                    {
                        selectedAccount.TotalLogin++;

                        if (inputAccount.CardPin.Equals(account.CardPin))
                        {
                            selectedAccount = account;
                            if (selectedAccount.IsLocked)
                            {
                                Screen.PrintLockAccount();
                            }
                            else
                            {
                                selectedAccount.TotalLogin = 0;
                                isLoginPassed = true;
                                break;
                            }
                        }
                    }
                }

                if (isLoginPassed == false)
                {
                    Utility.PrintMessage("Invalid card number or PIN.", false);

                    selectedAccount.IsLocked = selectedAccount.TotalLogin == 3;
                    if (selectedAccount.IsLocked)
                        Screen.PrintLockAccount();
                }

                Console.Clear();
            }
        }

        private void ProcessMenuOption()
        {
            switch (Validator.Convert<int>("your option"))
            {
                case (int)Menu.CheckBalance:
                    CheckBalance();
                    break;
                case (int)Menu.PlaceDeposit:
                    PlaceDeposit();
                    break;
                case (int)Menu.MakeWithdrawal:
                    MakeWithdrawal();
                    break;
                case (int)Menu.Transfer:
                    var Transfer = screen.TransferForm();
                    PerformTransfer(Transfer);
                    break;
                case (int)Menu.ViewTransaction:
                    ViewTransaction();
                    break;
                case (int)Menu.Logout:
                    Screen.LogoutProgress();
                    Utility.PrintConsoleWriteLine("You have succesfully logout. Please collect your ATM card.");
                    Console.Clear();
                    Execute();
                    break;
                default:
                    Utility.PrintMessage("Invalid Option Entered.", false);

                    break;
            }
        }

        public void CheckBalance()
        {
            Screen.PrintCheckBalanceScreen();
            Utility.PrintConsoleWriteLine(Utility.FormatAmount(selectedAccount.AccountBalance), false);
        }

        public void PlaceDeposit()
        {
            Utility.PrintConsoleWriteLine("\nNote: Actual ATM system will just\nlet you " +
            "place bank notes into physical ATM machine. \n");

            var transaction_amt = Validator.Convert<int>($"amount {Screen.cur}");

            Utility.PrintUserInputLabel("\nCheck and counting bank notes.");
            Utility.printDotAnimation();
            Console.SetCursorPosition(0, Console.CursorTop - 3);
            Console.WriteLine("");

            if (transaction_amt <= 0)
            {
                Utility.PrintMessage("Amount needs to be more than zero. Try again.", false);
                return;
            }

            if (transaction_amt % 10 != 0)
            {
                Utility.PrintMessage($"Key in the deposit amount only with multiply of 10. Try again.", false);
                return;
            }

            if (PreviewBankNotesCount(transaction_amt) == false)
            {
                Utility.PrintMessage($"You have cancelled your action.", false);
                return;
            }

            InsertTransaction(selectedAccount.Id, TransactionType.Deposit, +
                transaction_amt, "");

            selectedAccount.AccountBalance = selectedAccount.AccountBalance + transaction_amt;

            Utility.PrintMessage($"You have successfully deposited {Utility.FormatAmount(transaction_amt)}. " +
                "Please collect the bank slip. ", true);
        }

        public void MakeWithdrawal()
        {
            Console.WriteLine("\nNote: For GUI or actual ATM system, user can ");
            Console.Write("choose some default withdrawal amount or custom amount. \n\n");

            var transaction_amt = Validator.Convert<int>($"amount {Screen.cur}");

            if (transaction_amt <= 0)
            {
                Utility.PrintMessage("Amount needs to be more than zero. Try again.", false);
                return;
            }

            if (transaction_amt % 10 != 0)
            {
                Utility.PrintMessage($"Key in the deposit amount only with multiply of 10. Try again.", false);
                return;
            }


            if (transaction_amt > selectedAccount.AccountBalance)
            {
                Utility.PrintMessage($"Withdrawal failed. You do not have enough fund to withdraw {Utility.FormatAmount(transaction_amt)}", false);
                return;
            }

            if ((selectedAccount.AccountBalance - transaction_amt) < minimum_kept_amt)
            {
                Utility.PrintMessage($"Withdrawal failed. Your account needs to have minimum {Utility.FormatAmount(minimum_kept_amt)}", false);
                return;
            }


            InsertTransaction(selectedAccount.Id, TransactionType.Withdrawal, +
                -transaction_amt, "");

            selectedAccount.AccountBalance = selectedAccount.AccountBalance - transaction_amt;

            Utility.PrintMessage("Please collect your money. You have successfully withdraw " +
                $"{Utility.FormatAmount(transaction_amt)}. Please collect your bank slip.", true);

        }

        public void PerformTransfer(Transfer Transfer)
        {
            if (Transfer.TransferAmount <= 0)
            {
                Utility.PrintMessage("Amount needs to be more than zero. Try again.", false);
                return;
            }

            if (Transfer.TransferAmount > selectedAccount.AccountBalance)
            {
                Utility.PrintMessage("Withdrawal failed. You do not have enough " +
                    $"fund to withdraw {Utility.FormatAmount(Transfer.TransferAmount)}", false);
                return;
            }

            if (selectedAccount.AccountBalance - Transfer.TransferAmount < minimum_kept_amt)
            {
                Utility.PrintMessage($"Withdrawal failed. Your account needs to have " +
                    $"minimum {Utility.FormatAmount(minimum_kept_amt)}", false);
                return;
            }

            var selectedBankAccountReceiver = (from b in _accountList
                                               where b.AccountNumber == Transfer.RecipientBankAccountNumber
                                               select b).FirstOrDefault();

            if (selectedBankAccountReceiver == null)
            {
                Utility.PrintMessage($"Transfer failed. Receiver bank account number is invalid.", false);
                return;
            }

            if (selectedBankAccountReceiver.FullName != Transfer.RecipientBankAccountName)
            {
                Utility.PrintMessage($"Transfer failed. Recipient's account name does not match.", false);
                return;
            }

            InsertTransaction(selectedAccount.Id, TransactionType.Transfer, +
                -Transfer.TransferAmount, "Transfered " +
                $" to {selectedBankAccountReceiver.AccountNumber} ({selectedBankAccountReceiver.FullName})");

            selectedAccount.AccountBalance = selectedAccount.AccountBalance - Transfer.TransferAmount;

            InsertTransaction(selectedBankAccountReceiver.Id, TransactionType.Transfer, +
                Transfer.TransferAmount, "Transfered " +
                $" from {selectedAccount.AccountNumber} ({selectedAccount.FullName})");
                        
            selectedBankAccountReceiver.AccountBalance = selectedBankAccountReceiver.AccountBalance + Transfer.TransferAmount;

            Utility.PrintMessage("You have successfully transferred out " +
                $" {Utility.FormatAmount(Transfer.TransferAmount)} to {Transfer.RecipientBankAccountName}", true);
        }

        private bool PreviewBankNotesCount(decimal amount)
        {
            int hundredNotesCount = (int)amount / 100;
            int fiftyNotesCount = ((int)amount % 100) / 50;
            int tenNotesCount = ((int)amount % 50) / 10;

            Utility.PrintUserInputLabel("\nSummary                                                  ", true);
            Utility.PrintUserInputLabel("-------", true);
            Utility.PrintUserInputLabel($"{Screen.cur} 100 x {hundredNotesCount} = {100 * hundredNotesCount}", true);
            Utility.PrintUserInputLabel($"{Screen.cur} 50 x {fiftyNotesCount} = {50 * fiftyNotesCount}", true);
            Utility.PrintUserInputLabel($"{Screen.cur} 10 x {tenNotesCount} = {10 * tenNotesCount}", true);
            Utility.PrintUserInputLabel($"Total amount: {Utility.FormatAmount(amount)}\n\n", true);

            char opt = Validator.Convert<char>("1 to confirm");
            return opt.Equals('1');
        }

        public void ViewTransaction()
        {
           
            var filteredTransactionList = _listOfTransactions.Where(t => t.UserBankAccountId == selectedAccount.Id).ToList();

            if (filteredTransactionList.Count <= 0)
                Utility.PrintMessage($"There is no transaction yet.", true);
            else
            {
                var table = new ConsoleTable("Id", "Transaction Date", "Type", "Descriptions", "Amount " + Screen.cur);

                foreach (var tran in filteredTransactionList)
                {
                    table.AddRow(tran.TransactionId, tran.TransactionDate, tran.TransactionType, tran.Description, tran.TransactionAmount);
                }
                table.Options.EnableCount = false;
                table.Write();
                Utility.PrintMessage($"You have {filteredTransactionList.Count} transaction(s).", true);
            }
        }

        public void InsertTransaction(long _UserBankAccountId, TransactionType _tranType, decimal _tranAmount, string _desc)
        {
            var transaction = new Transaction()
            {
                TransactionId = Utility.GetTransactionId(),
                UserBankAccountId = _UserBankAccountId,
                TransactionDate = DateTime.Now,
                TransactionType = _tranType,
                TransactionAmount = _tranAmount,
                Description = _desc
            };

            _listOfTransactions.Add(transaction);
        }

    }
}