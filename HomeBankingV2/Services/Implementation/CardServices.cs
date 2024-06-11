using HomeBankingV2.DTO_s;
using HomeBankingV2.Models;
using HomeBankingV2.Repositories;
using System.Security.Cryptography;

namespace HomeBankingV2.Services.Implementation
{
    public class CardServices : ICardServices
    {
        private readonly ICardRepository _cardRepository;

        public CardServices(ICardRepository cardRepository) => _cardRepository = cardRepository;

        public IEnumerable<CardDTO> GetCardsByClientId(long clientId)
        {
            var cards = _cardRepository.GetCardsByClient(clientId);

            return cards.Select(c => new CardDTO(c));
        }

        public Card CreateNewCard(Client client, CreateCardDTO CreateCardDTO) 
        {
            var card = _cardRepository.FindByType(CreateCardDTO.type, client.Id);

            //El máximo de tarjetas que podrá tener un cliente es de 6 (3 debit y 3 credit).
            if (card.Count() < 3)
            {
                if (card.Any(card => card.Color == CreateCardDTO.color))
                {
                    throw new Exception("Card already in use");
                }
                else
                {
                    //creamos un Numero de TC
                    string cardNumber = GenerateCardNumber();
                    //creamos un codigo se seguridad
                    int numberCVV = RandomNumberGenerator.GetInt32(100, 1000);

                    //creamos nuevo obj tarjeta al cliente
                    Card newCard = new()
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

                    return newCard;
                }
            }
            else
            {
                throw new Exception("Unauthoriced to create another card");
            }
        }

        public string GenerateCardNumber()
        {
            var allCards = _cardRepository.GetAllCards();
            string numberToString;

            //validamos con un bucle para asegurarnos que la tarjeta creada no exista en la coleccion de tarjetas totales
            do
            {
                ///creamos numero aleatorio de tarjeta mediante una lista para luego concatenar
                List<int> cardDigits = new List<int>();

                for (int i = 0; i < 4; i++)
                {
                    // generamos un número aleatorio de 4 dígitos
                    int number = RandomNumberGenerator.GetInt32(1, 10000);
                    cardDigits.Add(number);
                }

                // concatenamos los números con formato "xxxx-xxxx-xxxx-xxxx"
                numberToString = string.Join("-", cardDigits.ConvertAll(d => d.ToString("D4")));

            } while (allCards.Any(card => card.Number == numberToString));

            return numberToString;
        }
    }
}
