using HomeBankingV2.Models;

namespace HomeBankingV2.Repositories
{
    public interface ITransactionRepository
    {
        IEnumerable<Transaction> GetAllTransactions();
        Transaction FindById(long id);
        void Save(Transaction transaction);
    }
}
