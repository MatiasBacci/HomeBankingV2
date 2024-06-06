using HomeBankingV2.Models;


namespace HomeBankingV2.Repositories.Implementation
{
    public class CardRepository : RepositoryBase<Card>, ICardRepository
    {
        public CardRepository(HomeBankingV2Context repositoryContext) : base(repositoryContext)
        {
        }

        public IEnumerable<Card> FindByType(string type, long ClientId)
        {
            return FindByCondition(card => card.Type == type && ClientId == card.ClientId)
                .ToList();
        }

        public void Save(Card card) {
            Create(card);
            SaveChanges();
        }
    }

}
