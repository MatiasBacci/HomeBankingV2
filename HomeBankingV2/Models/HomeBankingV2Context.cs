using Microsoft.EntityFrameworkCore;

namespace HomeBankingV2.Models
{
    //creamos la clase HomeBankingV2Context que hereda de DbContext que esta importada desde el EFC
    public class HomeBankingV2Context : DbContext

    {
        //option es un objeto de tipo DBcontextoptions que trabaja con Hombankingv2context
        public HomeBankingV2Context(DbContextOptions<HomeBankingV2Context> options) : base(options) { }

        //Agregamos las entidades que vamos a utilizar en el contexto (crea las bases de datos)
        //La tabla se va a llamar Clients y va a usar el tipo de dato Client
        public DbSet<Client> Clients { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<ClientLoan> ClientLoans { get; set; }
        public DbSet<Card> Cards { get; set; }
    }
}
