using HomeBankingV2.DTO_s;
using HomeBankingV2.Services;
using HomeBankingV2.Services.Implementation;
using Microsoft.AspNetCore.Mvc;



namespace HomeBankingV2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountServices _accountServices;
        public AccountsController(IAccountServices accountServices)
        {
            _accountServices = accountServices;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var accountDTO = _accountServices.GetAllAccountsDTOList();

                //returns status code 
                return Ok(accountDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public IActionResult Get(long id)
        {
            try
            {
                var account = _accountServices.GetById(id);
                var accountDTO = new AccountDTO(account);

                return Ok(accountDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}

