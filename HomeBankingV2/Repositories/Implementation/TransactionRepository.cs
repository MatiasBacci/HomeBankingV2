using HomeBankingV2.Models;


namespace HomeBankingV2.Repositories.Implementation
{
    public class TransactionRepository : RepositoryBase<Transaction>, ITransactionRepository
    {
        public TransactionRepository(HomeBankingV2Context repositoryContext) : base(repositoryContext)
        {
        }

        public IEnumerable<Transaction> GetAllTransactions()
        {
            return FindAll()
                .ToList();
        }

        public Transaction FindById(long id)
        {
            return FindByCondition(transaction => transaction.Id == id)
                .FirstOrDefault();
        }

        public void Save(Transaction transaction)
        {
            Create(transaction);
            SaveChanges();
        }
    }
}