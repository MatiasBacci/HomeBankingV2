﻿namespace HomeBankingV2.Models
{
    public class Card
    {
        public int Id { get; set; }
        public string CardHolder { get; set; }
        public string Type { get; set; }
        public string Color { get; set; }
        public string Number { get; set; }
        public int Cvv { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ThruDate { get; set; }
        public long ClientId { get; set; }
    }
}
