using dotnet.Data;
using dotnet.Endpoints;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connString = builder.Configuration.GetConnectionString("MySqlConnection");

builder.Services.AddDbContext<GameStoreContext>( options => //Registers an AddScoped injection of the dataBase Dependency
    options.UseMySql(connString,
    new MySqlServerVersion(new Version(8, 0, 29)))
);

var app = builder.Build();

app.MapGamesEndpoints();
app.MapGenresEndpoints();
await app.MigrateDbAsync();

app.Run();
