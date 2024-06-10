using HomeBankingV2.DTO_s;
using HomeBankingV2.Services;
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
                var accountsDTO = _accountServices.GetAllAccountsDTOList();
                
                return Ok(accountsDTO);
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

                return Ok(new AccountDTO(account));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}

