using HomeBankingV2.DTO_s;
using HomeBankingV2.Models;
using HomeBankingV2.Repositories;
using HomeBankingV2.Repositories.Implementation;
using HomeBankingV2.Services;
using HomeBankingV2.Services.Implementation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography;



namespace HomeBankingV2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IClientServices _clientServices;
        private readonly IAccountServices _accountServices;
        private readonly ICardServices _cardServices;

        public ClientsController(IClientServices clientServices, IAccountServices accountServices, ICardServices cardServices)
        {
            _clientServices = clientServices;
            _accountServices = accountServices;
            _cardServices = cardServices;
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Get()
        {
            try
            {
                var clients = _clientServices.GetAllClients();

                return Ok(clients);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        
        [HttpGet("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Get(long id)
        {
            try
            {
                var client = _clientServices.GetById(id);

                return Ok(client);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        
        [HttpGet("current")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult GetCurrent()
        {
            try
            {
                var current = _clientServices.GetCurrentClientDTO(User);

                return Ok(current);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        
        [HttpGet("current/accounts")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult GetAllAcounts()
        {
            try
            {
                Client client = _clientServices.GetCurrentClient(User);
                var accountsDTO = _accountServices.GetAccountByClientId(client.Id);
                
                return Ok(accountsDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        
        [HttpGet("current/cards")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult GetClientCards()
        {
            try
            {
                Client client = _clientServices.GetCurrentClient(User);
                var cardsDTO = _cardServices.GetCardsByClientId(client.Id);
                
                return Ok(cardsDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        
        [HttpPost]
        public IActionResult Post([FromBody] ClientUserDTO clientUserDTO)
        {
            try
            {
                Client newClient = _clientServices.CreateNewClient(clientUserDTO);
                Client clientebuscado = _clientServices.GetByEmail(newClient.Email);
                Account newAccount = _accountServices.CreateAccount(clientebuscado);

                return Ok(new ClientDTO (clientebuscado));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("current/accounts")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult CreateAccount()
        {
            try
            {
                var client = _clientServices.GetCurrentClient(User);
                var account = _accountServices.CreateAccount(client);

                return Ok(account);
            }
            catch (Exception ex)
            {
                return StatusCode(403, ex.Message);
            }
        }

        
        [HttpPost("current/cards")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult CreateCard([FromBody] CreateCardDTO CreateCardDTO) {

            try
            {
                Client client = _clientServices.GetCurrentClient(User);
                Card newCard = _cardServices.CreateNewCard(client, CreateCardDTO);
                return Ok(newCard);
            }
            catch (Exception ex) 
            {
                return StatusCode(403, ex.Message);
            }
        }
    }
}


