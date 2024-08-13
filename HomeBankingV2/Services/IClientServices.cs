using HomeBankingV2.DTO_s;
using HomeBankingV2.Models;
using System.Security.Claims;

namespace HomeBankingV2.Services
{
    public interface IClientServices
    {
        public List<Claim> AddClaims(Client client);
        public List<ClientDTO> GetAllClients();
        public ClientDTO GetById(long id);
        public Client GetByEmail(string mail);
        public Client GetCurrentClient(ClaimsPrincipal User);
        public ClientDTO GetCurrentClientDTO(ClaimsPrincipal User);
        public Client CreateNewClient(ClientUserDTO dto);
        
    }

}
