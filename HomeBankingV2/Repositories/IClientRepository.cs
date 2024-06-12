﻿using HomeBankingV2.Models;
using System.Security.Claims;

namespace HomeBankingV2.Repositories
{
    public interface IClientRepository
    {
        IEnumerable<Client> GetAllClients();
        IEnumerable<Client> GetOnlyClients();
        Client FindById(long id);
        Client FindByEmail(string email);
        void Save(Client client);
    }
}
