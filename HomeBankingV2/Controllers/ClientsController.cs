using Azure.Core.Pipeline;
using HomeBankingV2.DTO_s;
using HomeBankingV2.Models;
using HomeBankingV2.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System;
using System.Security.Cryptography;



namespace HomeBankingV2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IClientRepository _clientRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ICardRepository _cardRepository;

        public ClientsController(IClientRepository clientRepository, IAccountRepository accountRepository, ICardRepository cardRepository)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _cardRepository = cardRepository;
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Get()
        {
            try
            {
                var clients = _clientRepository.GetAllClients();
                var clientsDTO = clients.Select(c => new ClientDTO(c)).ToList();
                //returns status code 
                return Ok(clientsDTO);
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
                var client = _clientRepository.FindById(id);
                var clientDTO = new ClientDTO(client);

                return Ok(clientDTO);
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
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == string.Empty)
                {
                    return StatusCode(403, "Unauthoriced");
                }

                Client client = _clientRepository.FindByEmail(email);

                if (client == null)
                {
                    return StatusCode(403, "Unauthoriced");
                }

                var clientDTO = new ClientDTO(client);

                return Ok(clientDTO);
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
                //validamos datos antes
                if (String.IsNullOrEmpty(clientUserDTO.Email) || String.IsNullOrEmpty(clientUserDTO.Password) || String.IsNullOrEmpty(clientUserDTO.FirstName) || String.IsNullOrEmpty(clientUserDTO.LastName))
                    return StatusCode(403, "datos inválidos");

                //buscamos si ya existe el usuario
                Client user = _clientRepository.FindByEmail(clientUserDTO.Email);

                if (user != null)
                {
                    return StatusCode(403, "Email está en uso");
                }

                string newAccNumber="";
                 
                do{
                    newAccNumber = "VIN-" + RandomNumberGenerator.GetInt32(0, 99999999);

                } while (_accountRepository.GetAccountsByNumber(newAccNumber) != null);

                Client newClient = new Client
                {
                    Email = clientUserDTO.Email,
                    Password = clientUserDTO.Password,
                    FirstName = clientUserDTO.FirstName,
                    LastName = clientUserDTO.LastName,
                };
                
                _clientRepository.Save(newClient);

                Client user2 = _clientRepository.FindByEmail(clientUserDTO.Email);

                Account newAcc = new Account
                {
                    Number = newAccNumber.ToString(),
                    CreationDate = DateTime.Now,
                    Balance = 0,
                    ClientId = user2.Id
                };

                _accountRepository.Save(newAcc);

                return Created();

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
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == string.Empty)
                {
                    return StatusCode(403, "Unauthoriced");
                }

                Client client = _clientRepository.FindByEmail(email);
                 
                if (client == null)
                {
                    return StatusCode(403, "Unauthoriced");
                }

                var clientDTO = new ClientDTO(client);

                if (clientDTO.Accounts.Count() >= 3) {
                    return StatusCode(403, "Forbidden");
                    
                } else {
                    // Generar un nuevo número de cuenta único
                    string newAccNumber = "";

                    do
                    {
                        newAccNumber = "VIN-" + RandomNumberGenerator.GetInt32(0, 99999999);

                    } while (_accountRepository.GetAccountsByNumber(newAccNumber) != null);

                    // Crear una nueva cuenta
                    Account account = new Account
                    {
                        ClientId = client.Id,
                        Number = newAccNumber,
                        CreationDate = DateTime.Now,
                        Balance = 0
                    };

                    // Guardar la nueva cuenta en el repositorio
                    _accountRepository.Save(account);

                    return StatusCode(201, "Account created");
                }
            }

            catch (Exception ex)
            {
                return StatusCode(403, ex.Message);
            }
        }

        [HttpPost("current/cards")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult CreateCard([FromBody] CreateCardDTO CreateCardDTO) {

            
            //También ten en cuenta que los numeros de tarjeta y el cvv se deben generar de forma aleatoria.

            //Nota: El valor de la propiedad Number de card debe ser unico, por lo tanto debemos generar alguna comprobación para evitar numeros de tarjeta repetidos en nuestra base de datos.

            try
            {
                //traemos el cliente
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == string.Empty)
                {
                    return StatusCode(403, "Unauthoriced");
                }

                Client client = _clientRepository.FindByEmail(email);

                var card = _cardRepository.FindByType(CreateCardDTO.type, client.Id);

                //El máximo de tarjetas que podrá tener un cliente es de 6 (3 debit y 3 credit).
                if (card.Count() < 3 ) {
                    if (card.Any(card => card.Color == CreateCardDTO.color)){
                        return StatusCode(403, "Unauthoriced to create another card");
                    } else
                    {
                        Card newCard = new Card
                        {
                            CardHolder = client.FirstName + " " + client.LastName,
                            Type = CreateCardDTO.type,
                            Color = CreateCardDTO.color,
                            Number = "3325-6755-7876-4225",
                            Cvv = 355,
                            FromDate = DateTime.Now,
                            ThruDate = DateTime.Now.AddYears(5),
                            ClientId = client.Id,
                        };

                        _cardRepository.Save(newCard);

                        return StatusCode(201, "ESitooo");
                    }
                }
                else
                {
                    return StatusCode(403, "Unauthoriced to create another card");
                };
            }
            catch (Exception ex) 
            {
                return StatusCode(403, ex.Message);
            }
        }
    }
}


