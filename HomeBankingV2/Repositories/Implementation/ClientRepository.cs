﻿using HomeBankingV2.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeBankingV2.Repositories.Implementation
{
    public class ClientRepository : RepositoryBase<Client>, IClientRepository
    {
        public ClientRepository(HomeBankingV2Context repositoryContext) : base(repositoryContext)
        {
        }

        public Client FindById(long id)
        {
            return FindByCondition(client => client.Id == id)
                .Include(client => client.Accounts)
                .Include(client => client.ClientLoans)
                    .ThenInclude(cl => cl.Loan)
                .Include(client => client.Cards)
                .FirstOrDefault();
        }

        public IEnumerable<Client> GetAllClients()
        {
            return FindAll()
                .Include(client => client.Accounts)
                .Include(client => client.ClientLoans)
                    .ThenInclude(cl => cl.Loan)
                .Include(client => client.Cards)
                .ToList();
        }

        public void Save(Client client)
        {
            Create(client);
            SaveChanges();
        }
    }
}
