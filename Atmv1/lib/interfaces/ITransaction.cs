using Atmv1.lib.enums;
using Atmv1.lib.entities;

namespace Atmv1.lib.interfaces
{
    public interface ITransaction
    {
        void InsertTransaction(long _UserBankAccountId, TransactionType _tranType, decimal _tranAmount, string _desc);
        void ViewTransaction();
    }
}
