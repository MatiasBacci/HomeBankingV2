using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HomeBankingV2.DTO_s;
using HomeBankingV2.Models;
using HomeBankingV2.Services;



namespace HomeBankingV2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionServices _transactionServices;
        private readonly IClientServices _clientServices;
        
        public TransactionsController(ITransactionServices transactionServices, IClientServices clientServices)
        {
            _transactionServices = transactionServices;
            _clientServices = clientServices;    
        }


        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult GetAll()
        {
            try
            { 
                var transactionsDTO = _transactionServices.GetAllTransactionsDTO();

                return Ok(transactionsDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult Transfer ([FromBody] TransferDTO transferDTO)
        {            
            try
            {
                Client client = _clientServices.GetCurrentClient(User);
                Transaction transaction = _transactionServices.CreateTransaction(transferDTO, client);

                return Ok(new TransactionDTO(transaction));
            }
            catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }
    }
}
