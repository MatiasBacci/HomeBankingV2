using HomeBankingV2.DTO_s;
using HomeBankingV2.Models;
using HomeBankingV2.Repositories;

using System.Security.Cryptography;

namespace HomeBankingV2.Services.Implementation
{
    public class AccountServices : IAccountServices
    {
        private readonly IClientRepository _clientRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ICardRepository _cardRepository;

        public AccountServices(IClientRepository clientRepository, IAccountRepository accountRepository, ICardRepository cardRepository)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _cardRepository = cardRepository;
        }

        public IEnumerable<AccountDTO> GetAllAccountsDTOList()
        {
            var accounts = _accountRepository.GetAllAccounts();
            return accounts.Select(a => new AccountDTO(a)).ToList();
        }

        public Account CreateAccount(ClientDTO clientDTO)
        {
            if (clientDTO.Accounts.Count() >= 3)
            {
                throw new Exception("Forbidden");
            }
            else
            {
                // Generar un nuevo número de cuenta único
                string newAccNumber = "";

                do
                {
                    newAccNumber = "VIN-" + RandomNumberGenerator.GetInt32(0, 99999999);

                } while (_accountRepository.GetAccountByNumber(newAccNumber) != null);

                // Crear una nueva cuenta
                Account account = new Account
                {
                    ClientId = clientDTO.Id,
                    Number = newAccNumber,
                    CreationDate = DateTime.Now,
                    Balance = 0
                };

                // Guardar la nueva cuenta en el repositorio 
                _accountRepository.Save(account);
                return account;
            }
        }

        public IEnumerable<AccountDTO> GetAccountByClientId(long clientId) 
        {
            var account = _accountRepository.GetAccountsByClient(clientId);
            
            return account.Select(a => new AccountDTO (a));
        }

        public Account GetById(long id)
        {
            var account = _accountRepository.FindById(id);
           
            return account;
        }
    }
}
