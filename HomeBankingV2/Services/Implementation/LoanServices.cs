using HomeBankingV2.DTO_s;
using HomeBankingV2.Models;
using HomeBankingV2.Repositories;


namespace HomeBankingV2.Services.Implementation
{
    public class LoanServices : ILoanServices
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ILoanRepository _loanRepository;
        private readonly IClientLoanRepository _clientloanRepository;
        private readonly ITransactionRepository _transactionRepository;

        public LoanServices(IAccountRepository accountRepository,
                            IClientLoanRepository clientloanRepository,
                            ITransactionRepository transactionRepository,
                            ILoanRepository loanRepository)
        {
            _accountRepository = accountRepository;
            _clientloanRepository = clientloanRepository;
            _transactionRepository = transactionRepository;
            _loanRepository = loanRepository;
        }

        public IEnumerable<LoanDTO> GetAllLoans()
        {
            var loans = _loanRepository.GetAllLoans();
            return loans.Select(l => new LoanDTO(l)).ToList();
        }

        public Loan GetById(long id)
        {
            return _loanRepository.FindById(id);
        }

        public ClientLoan CreateLoan (LoanApplicationDTO applicationDTO, Client currentClient)
        {
            // Verificamos que los parámetros no estén vacíos o erróneos
            if (applicationDTO == null || string.IsNullOrEmpty(applicationDTO.Payments) || int.Parse(applicationDTO.Payments) == 0 ||
                applicationDTO.Amount <= 0 || string.IsNullOrEmpty(applicationDTO.ToAccountNumber))
            {
                throw new Exception("Please review the fields");
            }

            //Verificamos que el préstamo exista
            Loan loan = _loanRepository.FindById(applicationDTO.LoanId);

            if (loan == null)
                throw new Exception("Loan not found");

            //Verificamos que el monto solicitado no exceda el monto máximo del préstamo
            if (applicationDTO.Amount >= loan.MaxAmount)
                throw new Exception("Max amount exceeded");

            //Verificamos que la cantidad de cuotas se encuentre entre las disponibles del préstamo
            List<int> listOfPayments = loan.Payments.Split(',').Select(int.Parse).ToList();

            if (!listOfPayments.Contains(int.Parse(applicationDTO.Payments)))
                throw new Exception("Invalid number of payments");

            //Verificamos que la cuenta de destino exista
             Account toAccount = _accountRepository.GetAccountByNumber(applicationDTO.ToAccountNumber);

            if (applicationDTO.ToAccountNumber != toAccount.Number)
                throw new Exception("Invalid Account Number");
 
            if (currentClient == null)
            {
                throw new Exception("Client not found");
            }

            //Verificar que la cuenta de destino pertenezca al cliente autenticado
            if (toAccount.ClientId != currentClient.Id)
                throw new Exception("Client does not have requested account number");

            //Se debe crear una solicitud de préstamo con el monto solicitado sumando el 20 % del mismo
            ClientLoan requestedLoan = new()
            {
                Amount = applicationDTO.Amount * 1.20,
                Payments = applicationDTO.Payments,
                ClientId = currentClient.Id,
                LoanId = loan.Id
            };

            //Creamos transaccion
            Transaction transaction = new()
            {
                Amount = applicationDTO.Amount,
                Type = "CREDIT",
                Description = loan.Name + " loan approved",
                Date = DateTime.Now,
                AccountId = toAccount.Id,
            };

            toAccount.Balance += transaction.Amount;

            _clientloanRepository.Save(requestedLoan);
            _transactionRepository.Save(transaction);
            _accountRepository.Save(toAccount);

            return requestedLoan;
        }
    }
}
