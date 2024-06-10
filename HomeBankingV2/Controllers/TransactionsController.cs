using HomeBankingV2.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HomeBankingV2.DTO_s;
using HomeBankingV2.Models;

namespace HomeBankingV2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IClientRepository _clientRepository;
        public TransactionsController(ITransactionRepository transactionRepository, IAccountRepository accountRepository, IClientRepository clientRepository)
        {
            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
            _clientRepository = clientRepository;
        }

        [HttpPost]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult Transfer ([FromBody] TransferDTO transferDTO)
        {            
            try
            {
                // Verificamos que los parámetros no estén vacíos
                if (transferDTO == null ||
                    string.IsNullOrEmpty(transferDTO.FromAccountNumber) ||
                    string.IsNullOrEmpty(transferDTO.ToAccountNumber) ||
                    transferDTO.Amount <= 0 ||
                    string.IsNullOrEmpty(transferDTO.Description))
                {
                    return BadRequest("Invalid transfer details.");
                }

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

                // Verificamos que exista la cuenta de origen y que pertenezca al cliente autenticado
                var fromAccount = _accountRepository.GetAccountByNumber(transferDTO.FromAccountNumber);
                if (fromAccount == null)
                {
                    return NotFound("Source account not found.");
                }
                else if (fromAccount.ClientId != client.Id)
                {
                    return StatusCode(403, "Source account does not belong to the authenticated client.");
                }

                // Verificamos que exista la cuenta de destino
                var toAccount = _accountRepository.GetAccountByNumber(transferDTO.ToAccountNumber);
                if (toAccount == null)
                {
                    return NotFound("Destination account not found.");
                }

                // Verificamos que la cuenta de origen tenga el monto disponible
                if (fromAccount.Balance < transferDTO.Amount)
                {
                    return StatusCode(403, "Insufficient funds in source account.");
                }


                // Creamos transacciones de débito y crédito
                Transaction debitTransaction = new Transaction
                {
                    AccountId = fromAccount.Id,
                    Amount = -transferDTO.Amount,
                    Description = transferDTO.Description + " - Enviado a cuenta N°: " + toAccount.Number,
                    Date = DateTime.Now,
                    Type = "DEBIT"
                };

                Transaction creditTransaction = new Transaction
                {
                    AccountId = toAccount.Id,
                    Amount = transferDTO.Amount,
                    Description = transferDTO.Description + " - Recibido de cuenta N°: " + fromAccount.Number,
                    Date = DateTime.Now,
                    Type = "CREDIT"
                };

                // Actualizamos los saldos de las cuentas
                fromAccount.Balance -= transferDTO.Amount;
                toAccount.Balance += transferDTO.Amount;

                // Guardamos las transacciones y actualizar las cuentas en la base de datos
                _transactionRepository.Save(debitTransaction);
                _transactionRepository.Save(creditTransaction);
                _accountRepository.Save(fromAccount);
                _accountRepository.Save(toAccount);

                return Ok("Transfer successful.");
            }
            catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }
    }
}
