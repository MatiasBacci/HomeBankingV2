using HomeBankingV2.Models;

namespace HomeBankingV2.Repositories
{
    public interface IAccountRepository
    {
        IEnumerable<Account> GetAllAccounts();
        IEnumerable<Account> GetAccountsByClient(long clientId);
        Account FindById(long id);
        Account GetAccountsByNumber(string newAccNumber);
        void Save(Account account);
        
    }
}
