using HomeBankingV2.Models;

namespace HomeBankingV2.Repositories
{
    public interface ICardRepository
    {
      
        IEnumerable<Card> FindByType(string Type, long ClientId);
     
        void Save(Card card);
    }
}
