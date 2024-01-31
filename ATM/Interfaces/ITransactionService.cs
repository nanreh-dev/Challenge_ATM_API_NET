using ATM.Models;
using ATM.Models.CustomModels;

namespace ATM.Interfaces
{
    public interface ITransactionService
    {
        TransactionResult Transactions(string card, byte type, decimal amount);
        Transaction? Get(int userId);
        TransactionResult GetSaldo(string cardNumber);
        PaginationModel GetMovements(string page, User user);
    }
}
