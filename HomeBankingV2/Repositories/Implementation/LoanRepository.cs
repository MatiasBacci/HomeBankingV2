using HomeBankingV2.Models;


namespace HomeBankingV2.Repositories.Implementation
{
    public class LoanRepository : RepositoryBase<Loan>, ILoanRepository
    {
        public LoanRepository(HomeBankingV2Context repositoryContext) : base(repositoryContext)
        {
        }

        public Loan FindById(long id)
        {
            return FindByCondition(loan => loan.Id == id)
                .FirstOrDefault();
        }

        public IEnumerable<Loan> GetAllLoans()
        {
            return FindAll()
                  .ToList();   
        }

        public void Save(Loan loan)
        {
            Create(loan);
            SaveChanges();
        }
    }
}
