using HomeBankingV2.DTO_s;
using HomeBankingV2.Models;

namespace HomeBankingV2.Services
{
    public interface ILoanServices
    {
        public IEnumerable<LoanDTO> GetAllLoans();
        public Loan GetById (long id);
        public ClientLoan CreateLoan(LoanApplicationDTO applicationDTO, Client client);
    }
}
