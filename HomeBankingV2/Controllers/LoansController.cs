using HomeBankingV2.Repositories;
using HomeBankingV2.DTO_s;
using Microsoft.AspNetCore.Mvc;
using HomeBankingV2.Models;
using HomeBankingV2.Services;


namespace HomeBankingV2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoansController : ControllerBase
    {
        private readonly ILoanServices _loanServices;
        private readonly IClientServices _clientServices;


        public LoansController(ILoanServices loanServices, IClientServices clientServices)
        {
            _loanServices = loanServices;
            _clientServices = clientServices;
        }

        [HttpGet]
        
        public IActionResult GetAll()
        {
            try
            {
                var loansDTO = _loanServices.GetAllLoans();
                       
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
                var loan = _loanServices.GetById(id);
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
                Client client = _clientServices.GetCurrentClient(User);
                ClientLoan loan = _loanServices.CreateLoan (loanAppDTO, client);

                return Ok("Requested loan successfuly accredited in account.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
     } 
}
