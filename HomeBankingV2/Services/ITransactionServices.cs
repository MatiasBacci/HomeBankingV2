using HomeBankingV2.DTO_s;
using HomeBankingV2.Models;


namespace HomeBankingV2.Services
{
    public interface ITransactionServices
    {
        public IEnumerable<TransactionDTO> GetAllTransactionsDTO();

        public Transaction CreateTransaction(TransferDTO transferDTO, Client currentClient);

    }
}