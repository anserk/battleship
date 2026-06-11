using Battleship.Api.Persistence;
using Battleship.Api.Summaries;
using Battleship.Core;
using Microsoft.EntityFrameworkCore;

namespace Battleship.Api.Games;

public static class GameEndpoints
{
    public static IEndpointRouteBuilder MapGameEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/games", CreateGame);
        app.MapGet("/games/{id:guid}", GetGame);
        app.MapPost("/games/{id:guid}/shots", FireShot);

        return app;
    }

    private static IResult CreateGame(GameFactory factory, IGameStore store)
    {
        var game = factory.Create();
        var gameId = store.Create(game);

        return Results.Created($"/games/{gameId}", GameDtos.ToGameResponse(gameId, game));
    }

    private static IResult GetGame(Guid id, IGameStore store)
    {
        if (!store.TryUseGame(id, ToResponse, out var response))
        {
            return Results.NotFound();
        }

        return response;

        IResult ToResponse(Game game) =>
            Results.Ok(GameDtos.ToGameResponse(id, game));
    }

    private static async Task<IResult> FireShot(
        Guid id,
        ShotRequest request,
        IGameStore store,
        BattleshipDbContext db)
    {
        Game? completedGame = null;

        if (!store.TryUseGame(id, ToResponse, out var response))
        {
            return Results.NotFound();
        }

        if (completedGame is not null)
        {
            var summaryExists = await db.GameSummaries
                .AnyAsync(summary => summary.GameId == id);

            if (!summaryExists)
            {
                db.GameSummaries.Add(new GameSummary
                {
                    GameId = id,
                    BoardSize = completedGame.BoardSize,
                    ShipCount = completedGame.Ships.Count,
                    TotalShots = completedGame.ShotsFired,
                    CompletedAtUtc = DateTime.UtcNow
                });

                await db.SaveChangesAsync();
            }

        }

        return response;

        IResult ToResponse(Game game)
        {
            try
            {
                var result = game.FireShot(new Coordinate(request.X, request.Y));

                if (result.IsWon)
                {
                    completedGame = game;
                }

                return Results.Ok(GameDtos.ToPostShotResponse(result));
            }
            catch (ShotOutOfBoundsException)
            {
                return Results.BadRequest();
            }
            catch (DuplicateShotException)
            {
                return Results.Conflict();
            }
            catch (GameAlreadyWonException)
            {
                return Results.Conflict();
            }
        }
    }
}