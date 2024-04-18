using dotnet.Data;
using dotnet.Dtos;
using dotnet.Entities;
using dotnet.Mapping;
using Microsoft.EntityFrameworkCore;

namespace dotnet.Endpoints;

public static class GamesEndpoints
{
    const string GetGameEndpointName = "GetGame";

    public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app){
        
        var group = app.MapGroup("games")
            .WithParameterValidation();
        
        // GET /games
        group.MapGet("/", async (GameStoreContext dbContext) => 
            await dbContext.Games
                .Include(game => game.Genre)
                .Select(game => game.ToGameSummaryDto())
                .AsNoTracking()
                .ToListAsync());

        // GET /games/1
        group.MapGet("/{id}", async (int id, GameStoreContext dbContext) =>
        {
            Game? game = await dbContext.Games.FindAsync(id);

            return game is null ?
            Results.NotFound() : Results.Ok(game.ToGameDetailsDto());
        }).WithName(GetGameEndpointName);

        // POST /games
        group.MapPost("/", async (CreateGameDto newGame, GameStoreContext dbContext) => { //Using the record CreateGameDto, and dependency dbContext
            Game game = newGame.ToEntity();

            dbContext.Games.Add(game);
            await dbContext.SaveChangesAsync();

            return Results.CreatedAtRoute(
                GetGameEndpointName,
                new { id = game.Id },
                game.ToGameDetailsDto());
        }); //Fixed, this searches the endpoint with a ".WithName() and the same identifier (for this is GetGameEndpointName)

        // PUT /games
        group.MapPut("/{id}", async (int id, UpdateGameDto updatedGame, GameStoreContext dbContext) => {
            var existingGame = await dbContext.Games.FindAsync(id);
            
            if (existingGame is null){
                return Results.NotFound();
            }

            dbContext.Entry(existingGame)
                .CurrentValues
                .SetValues(updatedGame.ToEntity(id));
            await dbContext.SaveChangesAsync();

            return Results.NoContent();
        });

        // DELETE /games/1
        group.MapDelete("/{id}", async (int id, GameStoreContext dbContext) => {
            var gameToRemove = await dbContext.Games.FindAsync(id);
            if(gameToRemove != null){
                dbContext.Games.Remove(gameToRemove);
                await dbContext.SaveChangesAsync();
            }

            return Results.NoContent();
        });

        return group;
    }
}
