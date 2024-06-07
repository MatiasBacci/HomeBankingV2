using HomeBankingV2.Models;

namespace HomeBankingV2.Repositories
{
    public interface ILoanRepository
    {
        IEnumerable<Loan> GetAllLoans();
        Loan FindById(long id);
        void Save(Loan loan);
    }
}
