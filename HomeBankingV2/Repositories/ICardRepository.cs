using HomeBankingV2.Models;

namespace HomeBankingV2.Repositories
{
    public interface ICardRepository
    {
        IEnumerable<Card> GetAllCards();
        IEnumerable<Card> FindByType(string Type, long ClientId);
        IEnumerable<Card> GetCardsByClient(long ClientId);

        void Save(Card card);
    }
}
