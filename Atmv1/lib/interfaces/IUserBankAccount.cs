using System;
using System.Collections.Generic;
using System.Text;

namespace Atmv1.lib.interfaces
{
    public interface IUserBankAccount
    {
        void CheckBalance();
        void PlaceDeposit();
        void MakeWithdrawal();
    }
}
