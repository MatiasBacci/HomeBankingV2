using HomeBankingV2.DTO_s;
using HomeBankingV2.Models;
using HomeBankingV2.Repositories;
using System.Security.Claims;
using System.Security.Cryptography;


namespace HomeBankingV2.Services.Implementation
{
    public class ClientServices : IClientServices
    {
        private readonly IClientRepository _clientRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ICardRepository _cardRepository;

        public ClientServices(IClientRepository clientRepository, IAccountRepository accountRepository, ICardRepository cardRepository)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _cardRepository = cardRepository;
        }

        public List<ClientDTO> GetAllClients()
        {
            var clients = _clientRepository.GetAllClients();
            return clients.Select(c => new ClientDTO(c)).ToList();  
        }

        public ClientDTO GetById(long id) 
        {
            var client = _clientRepository.FindById(id);
            var clientDTO = new ClientDTO(client);
            return (clientDTO);
        }

        public Client GetByEmail(string mail)
        {
            var client = _clientRepository.FindByEmail(mail);
            if (client == null)
                throw new UnauthorizedAccessException("Unauthorized");

            return (client);
        }

        public Client GetCurrentClient(ClaimsPrincipal User)
        {
            string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
            if (email == string.Empty)
                throw new UnauthorizedAccessException("Unauthorized");

            Client client = _clientRepository.FindByEmail(email);

            if (client == null)
                throw new UnauthorizedAccessException("Unauthorized");

            return (client);
        }

        public ClientDTO GetCurrentClientDTO(ClaimsPrincipal User)
        {
            string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
            if (email == string.Empty)
                throw new UnauthorizedAccessException("Unauthorized");
            
            Client client = _clientRepository.FindByEmail(email);

            if (client == null)
                throw new UnauthorizedAccessException("Unauthorized");

            var clientDTO = new ClientDTO(client);

            return (clientDTO);
        }

        public Client CreateNewClient(ClientUserDTO clientUserDTO) 
        {            
            if (String.IsNullOrEmpty(clientUserDTO.Email) || String.IsNullOrEmpty(clientUserDTO.Password) || String.IsNullOrEmpty(clientUserDTO.FirstName) || String.IsNullOrEmpty(clientUserDTO.LastName))
                throw new UnauthorizedAccessException("Invalid Data");

            var existsClient = _clientRepository.FindByEmail(clientUserDTO.Email);

            if (existsClient != null)
                throw new UnauthorizedAccessException("Email is in use");

            string newAccNumber = "";

            do
            {
                newAccNumber = "VIN-" + RandomNumberGenerator.GetInt32(0, 99999999);

            } while (_accountRepository.GetAccountByNumber(newAccNumber) != null);

            Client newClient = new Client
            {
                Email = clientUserDTO.Email,
                Password = clientUserDTO.Password,
                FirstName = clientUserDTO.FirstName,
                LastName = clientUserDTO.LastName,
            };

            _clientRepository.Save(newClient);

            Client user2 = _clientRepository.FindByEmail(clientUserDTO.Email);

            Account newAcc = new Account
            {
                Number = newAccNumber.ToString(),
                CreationDate = DateTime.Now,
                Balance = 0,
                ClientId = user2.Id
            };

            _accountRepository.Save(newAcc);
            return newClient;
        }


    }
}