﻿using HomeBankingV2.Models;


namespace HomeBankingV2.DTO_s
{
    public class ClientDTO
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public ICollection<AccountClientDTO> Accounts { get; set; }
        public ICollection<ClientLoanDTO> Loans { get; set; }
        public ICollection<CardDTO> Cards { get; set; }

        public ClientDTO (Client client)
        {
            Id = client.Id;
            FirstName = client.FirstName;
            LastName = client.LastName;
            Email = client.Email;
            Accounts = client.Accounts.Select(a => new AccountClientDTO(a)).ToList();
            Loans = client.ClientLoans.Select(cl =>new ClientLoanDTO(cl)).ToList();
            Cards = client.Cards.Select(ca => new CardDTO(ca)).ToList();    
        }

        public ClientDTO (ClientUserDTO client)
        {
            FirstName = client.FirstName;
            LastName = client.LastName;
            Email = client.Email;
        }
    }
}
