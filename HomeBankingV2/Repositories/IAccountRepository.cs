using HomeBankingV2.Models;

namespace HomeBankingV2.Repositories
{
    public interface IAccountRepository
    {
        IEnumerable<Account> GetAllAccounts();
        IEnumerable<Account> GetAccountsByClient(long clientId);
        Account FindById(long id);
        Account GetAccountByNumber(string newAccNumber);
        Account GetAccountByNumberWithTransaction(string username);
        void Save(Account account);
        void UpdateAccount(Account Account);
    }
}
