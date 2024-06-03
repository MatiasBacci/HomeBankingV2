﻿using HomeBankingV2.Models;

namespace HomeBankingV2.Repositories
{
    public interface IClientRepository
    {
        IEnumerable<Client> GetAllClients();
        Client FindById(long id);
        void Save(Client client);
    }
}
