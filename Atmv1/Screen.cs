using System;
using Atmv1.lib.entities;
using Atmv1.lib.utils;

namespace Atmv1
{
    internal class Screen
    {
        // This class in charge of printing out text in user interface.

        internal const string cur = "$ ";

        internal static void WelcomeATM()
        {
            Console.Clear();
            Console.Title = "YouBank ATM System.";
            Console.WriteLine("Welcome to YouBank ATM.\n");
            Console.WriteLine("Please insert your ATM card.");
            Utility.PrintEnterMessage();
        }

        internal static void WelcomeCustomer(string fullName)
        {
            Utility.PrintConsoleWriteLine("Welcome back, " + fullName);
        }


        internal static void PrintLockAccount()
        {
            Console.Clear();
            Utility.PrintMessage("Your account is locked. Please go to " +
                "the nearest branch to unlocked your account. Thank you.", true);

            Utility.PrintEnterMessage();
            Environment.Exit(1);
        }

        internal UserBankAccount LoginForm()
        {
            var vmUserBankAccount = new UserBankAccount();

            vmUserBankAccount.CardNumber = Validator.Convert<long>("card number");

            vmUserBankAccount.CardPin = Convert.ToInt32(Utility.GetHiddenConsoleInput("Enter card pin: "));

            return vmUserBankAccount;
        }

        internal static void LoginProgress()
        {
            Console.Write("\nChecking card number and card pin.");
            Utility.printDotAnimation();
            Console.Clear();
        }

        internal static void LogoutProgress()
        {
            Console.WriteLine("Thank you for using YouBank ATM system.");
            Utility.printDotAnimation(5);
            Console.Clear();
        }


        internal static void DisplaySecureMenu()
        {
            Console.Clear();
            Console.WriteLine(" ---------------------------");
            Console.WriteLine("| Meybank ATM Secure Menu    |");
            Console.WriteLine("|                            |");
            Console.WriteLine("| 1. Balance Enquiry         |");
            Console.WriteLine("| 2. Cash Deposit            |");
            Console.WriteLine("| 3. Withdrawal              |");
            Console.WriteLine("| 4. Third Party Transfer    |");
            Console.WriteLine("| 5. Transactions            |");
            Console.WriteLine("| 6. Logout                  |");
            Console.WriteLine("|                            |");
            Console.WriteLine(" ---------------------------");
        }

        internal static void PrintCheckBalanceScreen()
        {
            Console.Write("Account balance amount: ");
        }

        internal Transfer TransferForm()
        {
            var Transfer = new Transfer();

            Transfer.RecipientBankAccountNumber = Validator.Convert<long>("recipient's account number");

            Transfer.TransferAmount = Validator.Convert<decimal>($"amount {cur}");

            Transfer.RecipientBankAccountName = Utility.GetRawInput("recipient's account name");
            
            return Transfer;
        }

        
    }
}