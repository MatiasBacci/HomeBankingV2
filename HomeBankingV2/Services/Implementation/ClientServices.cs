using HomeBankingV2.DTO_s;
using HomeBankingV2.Models;
using HomeBankingV2.Repositories;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;



namespace HomeBankingV2.Services.Implementation
{
    public class ClientServices : IClientServices
    {
        private readonly IClientRepository _clientRepository;
        private readonly PasswordHasher<Client> _passwordHasher;

        public ClientServices(IClientRepository clientRepository) 
        {
            _clientRepository = clientRepository;
            _passwordHasher = new PasswordHasher<Client>();
        }

        public List <Claim> AddClaims(Client client)
        {
            var claims = new List<Claim>
                {
                    new Claim("Client", client.Email)
                };

            if (client.Email == "michael@gmail.com")
            {
                claims.Add(new Claim("Admin", "true"));
            }

            return claims;
        }

        public List<ClientDTO> GetAllClients()
        {
            var clients = _clientRepository.GetAllClients();
            return clients.Select(c => new ClientDTO(c)).ToList();  
        }

        public ClientDTO GetById(long id) 
        {
            var client = _clientRepository.FindById(id);
            return (new ClientDTO(client));
        }

        public Client GetByEmail(string mail)
        {
            var client = _clientRepository.FindByEmail(mail);
            return client ?? throw new UnauthorizedAccessException("Unauthorized");
        }

        public Client GetCurrentClient(ClaimsPrincipal User)
        {
            string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
            if (email == string.Empty)
                throw new UnauthorizedAccessException("Unauthorized");

            Client client = _clientRepository.FindByEmail(email);

            return client ?? throw new UnauthorizedAccessException("Unauthorized");
        }

        public ClientDTO GetCurrentClientDTO(ClaimsPrincipal User)
        {
            Client client = GetCurrentClient(User);

            return (new ClientDTO(client));
        }

        public Client CreateNewClient(ClientUserDTO clientUserDTO) 
        {            
            if (String.IsNullOrEmpty(clientUserDTO.Email) || String.IsNullOrEmpty(clientUserDTO.Password) || String.IsNullOrEmpty(clientUserDTO.FirstName) || String.IsNullOrEmpty(clientUserDTO.LastName))
                throw new UnauthorizedAccessException("Invalid Data");

            var existsClient = _clientRepository.FindByEmail(clientUserDTO.Email);

            if (existsClient != null)
                throw new UnauthorizedAccessException("Email is in use");


            Client newClient = new()
            {
                Email = clientUserDTO.Email,
                Password = clientUserDTO.Password,
                FirstName = clientUserDTO.FirstName,
                LastName = clientUserDTO.LastName,
            };

            // Hashear la contraseña antes de guardarla
            newClient.Password = _passwordHasher.HashPassword(newClient, clientUserDTO.Password);

            _clientRepository.Save(newClient);

            return newClient;
        }

    }
}