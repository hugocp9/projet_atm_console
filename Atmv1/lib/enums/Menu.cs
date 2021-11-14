using System.ComponentModel;

namespace Atmv1.lib.enums
{
    public enum Menu
    {
        [Description("Check balance")]
        CheckBalance = 1,

        [Description("Place Deposit")]
        PlaceDeposit = 2,

        [Description("Make Withdrawal")]
        MakeWithdrawal = 3,

        [Description("Transfer")]
        Transfer = 4,

        [Description("Transaction")]
        ViewTransaction = 5,

        [Description("Logout")]
        Logout = 6
    }
}