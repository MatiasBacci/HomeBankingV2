using HomeBankingV2.Models;

namespace HomeBankingV2.Repositories.Implementation
{
    public class ClientLoanRepository : RepositoryBase<ClientLoan>, IClientLoanRepository
    {
        public ClientLoanRepository(HomeBankingV2Context repositoryContext) : base(repositoryContext)
        {
        }

       public void Save(ClientLoan clientLoan)
        {
            Create(clientLoan);
            SaveChanges();
        }
    }
}
