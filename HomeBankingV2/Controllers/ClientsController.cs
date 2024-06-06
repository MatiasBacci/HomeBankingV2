using HomeBankingV2.DTO_s;
using HomeBankingV2.Models;
using HomeBankingV2.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("current/accounts")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult GetAllAcounts()
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
                    return NotFound("Client not found");
                }

                var account = _accountRepository.GetAccountsByClient(client.Id);
                var accountDTO = account.Select(c => new AccountDTO(c)).ToList();
                //returns status code 
                return Ok(accountDTO);
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

                var cards = _cardRepository.GetCardsByClient(client.Id);
                var cardsDTO = cards.Select(c => new CardDTO(c)).ToList();

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
                //validamos datos antes
                if (String.IsNullOrEmpty(clientUserDTO.Email) || String.IsNullOrEmpty(clientUserDTO.Password) || String.IsNullOrEmpty(clientUserDTO.FirstName) || String.IsNullOrEmpty(clientUserDTO.LastName))
                    return StatusCode(403, "Invalid Data");

                //buscamos si ya existe el usuario
                Client user = _clientRepository.FindByEmail(clientUserDTO.Email);

                if (user != null)
                {
                    return StatusCode(403, "Email is already in use");
                }

                string newAccNumber="";
                 
                do{
                    newAccNumber = "VIN-" + RandomNumberGenerator.GetInt32(0, 99999999);

                } while (_accountRepository.GetAccountByNumber(newAccNumber) != null);

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
                    return StatusCode(403, "Forbidden");
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

                    } while (_accountRepository.GetAccountByNumber(newAccNumber) != null);

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
                        return StatusCode(403, "Card already in use");
                    } else
                    {
                        var allCards = _cardRepository.GetAllCards();
                        string cardNumber;

                        //validamos con un bucle para asegurarnos que la tarjeta creada no exista en la coleccion de tarjetas totales
                        do
                        {
                            ///creamos numero aleatorio de tarjeta mediante una lista para luego concatenar
                            List<int> cardDigits = new List<int>();

                            for (int i = 0; i < 4; i++)
                            {
                                // generaramos un número aleatorio de 4 dígitos
                                int number = RandomNumberGenerator.GetInt32(1, 10000);
                                cardDigits.Add(number);
                            }

                            // concatenamos los números con formato "xxxx-xxxx-xxxx-xxxx"
                            cardNumber = string.Join("-", cardDigits.ConvertAll(d => d.ToString("D4")));
  
                        } while (allCards.Any(card => card.Number == cardNumber));

                        //creamos un codigo se seguridad
                        int numberCVV = RandomNumberGenerator.GetInt32(100, 1000);

                        //creamos nuevo obj tarjeta al cliente
                        Card newCard = new Card
                        {
                            CardHolder = client.FirstName + " " + client.LastName,
                            Type = CreateCardDTO.type,
                            Color = CreateCardDTO.color,
                            Number = cardNumber,
                            Cvv = numberCVV,
                            FromDate = DateTime.Now,
                            ThruDate = DateTime.Now.AddYears(5),
                            ClientId = client.Id,
                        };

                        _cardRepository.Save(newCard);

                        return StatusCode(201, "Card created succesfully");
                    }
                }
                else
                {
                    return StatusCode(403, "Unauthoriced to create another card");
                }
            }
            catch (Exception ex) 
            {
                return StatusCode(403, ex.Message);
            }
        }
    }
}


