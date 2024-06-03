using HomeBankingV2.Models;
using HomeBankingV2.Repositories;
using Microsoft.EntityFrameworkCore;
using HomeBankingV2.Repositories.Implementation;


//Creamos la variable Builder para despues ir agregando distintos elementos para al final correr
//la app con todo lo necesario
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

//Agregamos el contexto al builder para que la app tenga acceso
builder.Services.AddDbContext<HomeBankingV2Context>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("HomeBankingConexion")));

//Agregamos el repo de cliente 
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();


//el metodo build "pega" todo lo anteriormente preparado y lo unifica
var app = builder.Build();

//creamos un espacio para poder ejecutar codigo en memoria para ejecutar las siguientes lineas
//sin tener que crear un archivo ni nada
using (var scope = app.Services.CreateScope())
{
    //Aqui obtenemos todos los services registrados en la App
    var services = scope.ServiceProvider;
    try
    {
        // En este paso buscamos un service que este con la clase HomeBankingV2Context
        var context = services.GetRequiredService<HomeBankingV2Context>();
        DBInitializer.Initialize(context);//Recien aca se ejecuta el Initializer
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ha ocurrido un error al enviar la información a la base de datos!");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
//metodo para utilizar el wwwroot
app.UseStaticFiles();

//metodo para utilizar el enrutamiento del back
app.UseRouting();

app.MapControllers();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
