using HomeBankingV2.Repositories;
using HomeBankingV2.DTO_s;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HomeBankingV2.Models;


namespace HomeBankingV2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoansController : ControllerBase
    {
        private readonly ILoanRepository _loanRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IClientLoanRepository _clientloanRepository;
        private readonly ITransactionRepository _transactionRepository;

        public LoansController(ILoanRepository loanRepository, IAccountRepository accountRepository, IClientRepository clientRepository, IClientLoanRepository clientloanRepository, ITransactionRepository transactionRepository)
        {
            _loanRepository = loanRepository;
            _accountRepository = accountRepository;
            _clientRepository = clientRepository;
            _clientloanRepository = clientloanRepository;
            _transactionRepository = transactionRepository;
        }

        [HttpGet]
        
        public IActionResult GetAll()
        {
            try
            {
                var loan = _loanRepository.GetAllLoans();
                var loansDTO = loan.Select(c => new LoanDTO(c)).ToList();
                //returns status code 
                return Ok(loansDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]

        public IActionResult GetId(long id)
        {
            try
            {
                var loan = _loanRepository.FindById(id);
                var loanDTO = new LoanDTO(loan);

                return Ok(loanDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        
        public IActionResult CreateLoan(LoanApplicationDTO loanAppDTO)
        {
            try
            {
                // Verificamos que los parámetros no estén vacíos o erróneos
                if (loanAppDTO == null || string.IsNullOrEmpty(loanAppDTO.Payments) || int.Parse(loanAppDTO.Payments) == 0 ||
                    loanAppDTO.Amount <= 0 || string.IsNullOrEmpty(loanAppDTO.ToAccountNumber))
                {
                    return BadRequest("Please review the fields");
                }

                //Verificamos que el préstamo exista
                Loan loan = _loanRepository.FindById(loanAppDTO.LoanId);

                if (loan == null)
                    return StatusCode(403, "Loan not found");

                //Verificamos que el monto solicitado no exceda el monto máximo del préstamo
                if (loanAppDTO.Amount >= loan.MaxAmount)
                    return StatusCode(403, "Amount exceeds permit limit");

                //Verificamos que la cantidad de cuotas se encuentre entre las disponibles del préstamo
                List<int> listOfPayments = loan.Payments.Split(',').Select(int.Parse).ToList();

                if (!listOfPayments.Contains(int.Parse(loanAppDTO.Payments)))
                    return StatusCode(403, "Invalid payments");

                //Verificamos que la cuenta de destino exista
                Account toAccount = _accountRepository.GetAccountByNumber(loanAppDTO.ToAccountNumber);

                if (loanAppDTO.ToAccountNumber != toAccount.Number)
                    return StatusCode(403, "Invalid Account Number");

                // Obtenemos mail del cliente autenticado
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == string.Empty)
                {
                    return StatusCode(403, "Unauthoriced");
                }
                // buscamos el cliente
                Client client = _clientRepository.FindByEmail(email);

                if (client == null)
                {
                    return NotFound("Client not found");
                }

                //Verificar que la cuenta de destino pertenezca al cliente autenticado
                if (toAccount.ClientId != client.Id)
                    return StatusCode(403, "Client does not have requested account number");

                //Se debe crear una solicitud de préstamo con el monto solicitado sumando el 20 % del mismo
                ClientLoan requestedLoan = new ClientLoan()
                {
                    Amount = loanAppDTO.Amount * 1.20,
                    Payments = loanAppDTO.Payments,
                    ClientId = client.Id,
                    LoanId = loan.Id
                };

                //Creamos transaccion
                Transaction transaction = new Transaction()
                {
                    Amount = loanAppDTO.Amount,
                    Type = "CREDIT",
                    Description = loan.Name + " loan approved",
                    Date = DateTime.Now,
                    AccountId = toAccount.Id,
                };

                toAccount.Balance += transaction.Amount;

                _clientloanRepository.Save(requestedLoan);
                _transactionRepository.Save(transaction);
                _accountRepository.Save(toAccount);

                return Ok("Requested loan successfuly accredited in account.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
     } 
}
