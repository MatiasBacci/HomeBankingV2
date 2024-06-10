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
        private readonly ICardRepository _cardRepository;

        public ClientsController(IClientServices clientServices, IAccountServices accountServices, ICardRepository cardRepository)
        {
            _clientServices = clientServices;
            _accountServices = accountServices;
            _cardRepository = cardRepository;
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Get()
        {
            try
            {
                return Ok(_clientServices.GetAllClients());
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
                return Ok(_clientServices.GetById(id));
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
                var clientDTO = _clientServices.GetCurrentClientDTO(User);

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
                Client client = _clientServices.GetCurrentClient(User);
                var accounts = _accountServices.GetAccountByClientId(client.Id);
                //returns status code 
                return Ok(accounts);
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
                Client client= _clientServices.GetCurrentClient(User);
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
                Client newClient = _clientServices.CreateNewClient(clientUserDTO);

                return Created();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /*
        [HttpPost("current/accounts")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult CreateAccount()
        {
            try
            {
                var clientDTO = _clientServices.GetCurrentClientDTO(User);

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
                        ClientId = clientDTO.Id,
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
        /*
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
        }*/
    }
}


