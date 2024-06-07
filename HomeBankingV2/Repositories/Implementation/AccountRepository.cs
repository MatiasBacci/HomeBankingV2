using HomeBankingV2.Models;
using Microsoft.EntityFrameworkCore;


namespace HomeBankingV2.Repositories.Implementation
{
    public class AccountRepository : RepositoryBase<Account>, IAccountRepository
    {
        public AccountRepository(HomeBankingV2Context repositoryContext) : base(repositoryContext)
        {
        }

        public IEnumerable<Account> GetAllAccounts()
        {
            return FindAll()
                .Include(account => account.Transactions)
                .ToList();
        }

        public IEnumerable<Account> GetAccountsByClient(long clientId)
        {
            return FindByCondition(account => account.ClientId == clientId)
                .Include(account => account.Transactions)
                .ToList();
        }

        public Account FindById(long id)
        {
            return FindByCondition(account => account.Id == id)
               .Include(account => account.Transactions)
               .FirstOrDefault();
        }

        public Account GetAccountByNumber(string number)
        {
            return FindByCondition(account => account.Number == number)
                .FirstOrDefault();
        }

        public Account GetAccountByNumberWithTransaction(string number)
        {
            return FindByCondition(account => account.Number == number)
                .Include(account => account.Transactions)
                .FirstOrDefault();
        }

        public void UpdateAccount(Account account)
        {
            Update(account);
            SaveChanges();
        }

        public void Save(Account account)
        {
            Create(account);
            SaveChanges();
        }
    }
}
