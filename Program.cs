using dotnet.Data;
using dotnet.Endpoints;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connString = builder.Configuration.GetConnectionString("MySqlConnection");

builder.Services.AddDbContext<GameStoreContext>( options =>
    options.UseMySql(connString,
    new MySqlServerVersion(new Version(8, 0, 29)))
);

var app = builder.Build();

app.MapGamesEndpoints();
app.MigrateDb();

app.Run();
