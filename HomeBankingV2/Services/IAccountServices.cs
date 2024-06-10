using HomeBankingV2.DTO_s;
using HomeBankingV2.Models;

namespace HomeBankingV2.Services
{
    public interface IAccountServices
    {
        public Account CreateAccount(Client client);
        public IEnumerable<AccountDTO> GetAllAccountsDTOList();
        public IEnumerable<AccountDTO> GetAccountByClientId(long clientId);
        public Account GetById(long id);
    }


}
