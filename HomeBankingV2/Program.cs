using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using HomeBankingV2.Models;
using HomeBankingV2.Repositories;
using HomeBankingV2.Repositories.Implementation;
using HomeBankingV2.Services;
using HomeBankingV2.Services.Implementation;



//Creamos la variable Builder para despues ir agregando distintos elementos para al final correr
//la app con todo lo necesario
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Agregamos el contexto al builder para que la app tenga acceso
builder.Services.AddDbContext<HomeBankingV2Context>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("HomeBankingConexion")));

//Agregamos los repos
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<ICardRepository, CardRepository>();
builder.Services.AddScoped<ILoanRepository, LoanRepository>();
builder.Services.AddScoped<IClientLoanRepository, ClientLoanRepository>();

//Agregamos los servicios 
builder.Services.AddScoped<IClientServices, ClientServices>();
builder.Services.AddScoped<IAccountServices, AccountServices>();
builder.Services.AddScoped<ICardServices, CardServices>();
builder.Services.AddScoped<ILoanServices, LoanServices>();
builder.Services.AddScoped<ITransactionServices, TransactionServices>();
builder.Services.AddScoped<IAuthServices, AuthServices>();


//autenticación
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
      .AddCookie(options =>
      {
          options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
          options.LoginPath = new PathString("/index.html");
      });


//autorización
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ClientOnly", policy => policy.RequireClaim("Client"));
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim("Admin"));
});


//el metodo build "pega" todo lo anteriormente preparado y lo unifica
var app = builder.Build();

//creamos un espacio para poder ejecutar codigo en memoria para ejecutar las siguientes lineas
//sin tener que crear un archivo ni nada
//using (var scope = app.Services.CreateScope())
//{
//    //Aqui obtenemos todos los services registrados en la App
//    var services = scope.ServiceProvider;
//    try
//    {
//        // En este paso buscamos un service que este con la clase HomeBankingV2Context
//        var context = services.GetRequiredService<HomeBankingV2Context>();
//        DBInitializer.Initialize(context);//Recien aca se ejecuta el Initializer
//    }
//    catch (Exception ex)
//    {
//        var logger = services.GetRequiredService<ILogger<Program>>();
//        logger.LogError(ex, "Ha ocurrido un error al enviar la información a la base de datos!");
//    }
//}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
} else
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//metodo para utilizar el wwwroot
app.UseDefaultFiles();
app.UseStaticFiles();


//metodo para utilizar el enrutamiento del back
app.UseRouting();

app.MapControllers();

app.UseAuthentication();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
