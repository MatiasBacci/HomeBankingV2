namespace HomeBankingV2.Models
{
    public class DBInitializer
    {   //usamos static que permitirá utilizar la clase sin tener que instanciarla 
        public static void Initialize(HomeBankingV2Context context)
        {
            //Cargamos clientes
            if (!context.Clients.Any())
            {
                var clients = new Client[]
                {
                    new Client { Email = "michael@gmail.com", FirstName="Michael", LastName="Scott", Password="123456"},
                    new Client { Email = "jim@gmail.com", FirstName="Jim", LastName="Halpert", Password="123456"},
                    new Client { Email = "pam@gmail.com", FirstName="Pam", LastName="Beezley", Password="123456"}   
                };

                //se agrega al contexto de clientes
                context.Clients.AddRange(clients);

                //guardamos los cambios
                context.SaveChanges();
            }
            //Cargamos cuentas
            if (!context.Accounts.Any())
            {
                var account1 = context.Clients.FirstOrDefault(c => c.Email == "michael@gmail.com");
                if (account1 != null)
                {
                    var accounts = new Account[]
                    {
                        new Account {ClientId = account1.Id, CreationDate = DateTime.Now, Number = "VIN001", Balance = 10000 },
                        new Account {ClientId = account1.Id, CreationDate = DateTime.Now, Number = "VIN002", Balance = 20000 }
                    };
                    context.Accounts.AddRange(accounts);
                    context.SaveChanges();
                }
                var account2 = context.Clients.FirstOrDefault(c => c.Email == "jim@gmail.com");
                if (account2 != null)
                {
                    var accounts = new Account[]
                    {
                        new Account {ClientId = account2.Id, CreationDate = DateTime.Now, Number = "VIN003", Balance = 200000 }
                    };
                    context.Accounts.AddRange(accounts);
                    context.SaveChanges();
                }
            }
            if (!context.Transactions.Any())
            {
                var account1 = context.Accounts.FirstOrDefault(c => c.Number == "VIN001");
                if (account1 != null)
                {
                    var transactions = new Transaction[]
                    {
                        new Transaction { AccountId= account1.Id, Amount = 10000, Date= DateTime.Now.AddHours(-5), Description = "Transferencia reccibida", Type = TransactionType.CREDIT.ToString() },
                        new Transaction { AccountId= account1.Id, Amount = -2000, Date= DateTime.Now.AddHours(-6), Description = "Compra en carniceria", Type = TransactionType.DEBIT.ToString() },
                        new Transaction { AccountId= account1.Id, Amount = -3000, Date= DateTime.Now.AddHours(-7), Description = "Compra en verduleria", Type = TransactionType.DEBIT.ToString() },
                    };

                context.Transactions.AddRange(transactions);
                context.SaveChanges();
                }
            }
            if (!context.Loans.Any())
            {
                //crearemos 3 prestamos Hipotecario, Personal y Automotriz
                var loans = new Loan[]
                {
                    new Loan { Name = "Hipotecario", MaxAmount = 500000, Payments = "12,24,36,48,60" },
                    new Loan { Name = "Personal", MaxAmount = 100000, Payments = "6,12,24" },
                    new Loan { Name = "Automotriz", MaxAmount = 300000, Payments = "6,12,24,36" },
                };

                context.Loans.AddRange(loans);
                context.SaveChanges();

                //ahora agregaremos los clientloan (Prestamos del cliente)
                var client1 = context.Clients.FirstOrDefault(c => c.Email == "michael@gmail.com");
                if (client1 != null)
                {
                    //ahora usaremos los 3 tipos de prestamos
                    var loan1 = context.Loans.FirstOrDefault(l => l.Name == "Hipotecario");
                    if (loan1 != null)
                    {
                        var clientLoan1 = new ClientLoan
                        {
                            Amount = 400000,
                            ClientId = client1.Id,
                            LoanId = loan1.Id,
                            Payments = "60"
                        };
                        context.ClientLoans.Add(clientLoan1);
                    }

                    var loan2 = context.Loans.FirstOrDefault(l => l.Name == "Personal");
                    if (loan2 != null)
                    {
                        var clientLoan2 = new ClientLoan
                        {
                            Amount = 50000,
                            ClientId = client1.Id,
                            LoanId = loan2.Id,
                            Payments = "12"
                        };
                        context.ClientLoans.Add(clientLoan2);
                    }

                    var loan3 = context.Loans.FirstOrDefault(l => l.Name == "Automotriz");
                    if (loan3 != null)
                    {
                        var clientLoan3 = new ClientLoan
                        {
                            Amount = 100000,
                            ClientId = client1.Id,
                            LoanId = loan3.Id,
                            Payments = "24"
                        };
                        context.ClientLoans.Add(clientLoan3);
                    }

                    //guardamos todos los prestamos
                    context.SaveChanges();
                }
            }
            if (!context.Cards.Any())
            {
                //buscamos cliente
                var client1 = context.Clients.FirstOrDefault(c => c.Email == "michael@gmail.com");
                if (client1 != null)
                {
                    //le agregamos 2 tarjetas de crédito una GOLD y una TITANIUM, de tipo DEBITO Y CREDITO RESPECTIVAMENTE
                    var cards = new Card[]
                    {
                        new Card {
                            ClientId= client1.Id,
                            CardHolder = client1.FirstName + " " + client1.LastName,
                            Type = CardType.DEBIT.ToString(),
                            Color = CardColor.GOLD.ToString(),
                            Number = "3325-6745-7876-4445",
                            Cvv = 990,
                            FromDate= DateTime.Now,
                            ThruDate= DateTime.Now.AddYears(4),
                        },
                        new Card {
                            ClientId= client1.Id,
                            CardHolder = client1.FirstName + " " + client1.LastName,
                            Type = CardType.CREDIT.ToString(),
                            Color = CardColor.TITANIUM.ToString(),
                            Number = "2234-6745-552-7888",
                            Cvv = 750,
                            FromDate= DateTime.Now,
                            ThruDate= DateTime.Now.AddYears(5),
                        },
                    };

                    context.Cards.AddRange(cards);
                    context.SaveChanges();
                }
            }
        }
    }
}
