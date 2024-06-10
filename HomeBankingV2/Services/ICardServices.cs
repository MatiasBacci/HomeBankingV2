using HomeBankingV2.DTO_s;
using HomeBankingV2.Models;
using System.Runtime.CompilerServices;

namespace HomeBankingV2.Services
{
    public interface ICardServices
    {
        public IEnumerable<CardDTO> GetCardsByClientId(long clientId);
        public Card CreateNewCard(Client client, CreateCardDTO CreateCardDTO);
    }
}
