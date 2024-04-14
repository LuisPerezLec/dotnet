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
        group.MapGet("/", (GameStoreContext dbContext) => {
            return dbContext.Games
                .Include(game => game.Genre)
                .Select(game => game.ToGameSummaryDto())
                .AsNoTracking();
        });

        // GET /games/1
        group.MapGet("/{id}", (int id, GameStoreContext dbContext) =>
        {
            Game? game = dbContext.Games.Find(id);

            return game is null ?
            Results.NotFound() : Results.Ok(game.ToGameDetailsDto());
        });

        // POST /games
        group.MapPost("/", (CreateGameDto newGame, GameStoreContext dbContext) => { //Using the record CreateGameDto, and dependency dbContext
            Game game = newGame.ToEntity();

            dbContext.Games.Add(game);
            dbContext.SaveChanges();

            return Results.CreatedAtRoute(
                GetGameEndpointName,
                new { id = game.Id },
                game.ToGameDetailsDto());
        }); //For now, this posts, but breakes the code and stops the server, need to fix

        // PUT /games
        group.MapPut("/{id}", (int id, UpdateGameDto updatedGame, GameStoreContext dbContext) => {
            var existingGame = dbContext.Games.Find(id);
            
            if (existingGame is null){
                return Results.NotFound();
            }

            dbContext.Entry(existingGame)
                .CurrentValues
                .SetValues(updatedGame.ToEntity(id));
            dbContext.SaveChanges();

            return Results.NoContent();
        });

        // DELETE /games/1
        group.MapDelete("/{id}", (int id, GameStoreContext dbContext) => {
            var gameToRemove = dbContext.Games.Find(id);
            if(gameToRemove != null){
                dbContext.Games.Remove(gameToRemove);
                dbContext.SaveChanges();
            }

            return Results.NoContent();
        });

        return group;
    }
}
