using HomeBankingV2.DTO_s;
using HomeBankingV2.Models;
using HomeBankingV2.Repositories;


namespace HomeBankingV2.Services.Implementation
{
    public class TransactionServices : ITransactionServices
    {
        public readonly ITransactionRepository _transactionRepository;
        public readonly IAccountRepository _accountRepository;

        public TransactionServices (IAccountRepository accountRepository, 
                                    ITransactionRepository transactionRepository)
        {
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
        }

        public IEnumerable <TransactionDTO> GetAllTransactionsDTO() 
        {
            var transactions = _transactionRepository.GetAllTransactions();
          
            return (transactions.Select(t => new TransactionDTO(t)).ToList());
        }

        public Transaction CreateTransaction(TransferDTO transferDTO, Client currentClient)
        {
            // Verificamos que los parámetros no estén vacíos
            if (transferDTO == null ||
                string.IsNullOrEmpty(transferDTO.FromAccountNumber) ||
                string.IsNullOrEmpty(transferDTO.ToAccountNumber) ||
                transferDTO.Amount <= 0 ||
                string.IsNullOrEmpty(transferDTO.Description))
            {
                throw new Exception("Invalid transfer details.");
            }

            // Verificamos que exista la cuenta de origen y que pertenezca al cliente autenticado
            var fromAccount = _accountRepository.GetAccountByNumber(transferDTO.FromAccountNumber);
            
            if (fromAccount == null)
            {
                throw new Exception("Account not found.");
            }
            else if (fromAccount.ClientId != currentClient.Id)
            {
                throw new Exception("Account does not belong to the authenticated client.");
            }

            // Verificamos que exista la cuenta de destino
            var toAccount = _accountRepository.GetAccountByNumber(transferDTO.ToAccountNumber);
            if (toAccount == null)
            {
                throw new Exception("Account not found.");
            }

            // Verificamos que la cuenta de origen tenga el monto disponible
            if (fromAccount.Balance < transferDTO.Amount)
            {
                throw new Exception("Insufficient funds in source account.");
            }

            // Creamos transacciones de débito y crédito
            Transaction debitTransaction = new()
            {
                AccountId = fromAccount.Id,
                Amount = -transferDTO.Amount,
                Description =  transferDTO.Description + " - Enviado a cuenta N°: " + toAccount.Number,
                Date = DateTime.Now,
                Type = "DEBIT"
            };

            Transaction creditTransaction = new()
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

            return (debitTransaction);
        }
        
    }
}
