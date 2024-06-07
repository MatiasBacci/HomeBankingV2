using HomeBankingV2.Models;

namespace HomeBankingV2.Repositories
{
    public interface IClientLoanRepository
    {
        void Save(ClientLoan ClientLoan);
    }
}
