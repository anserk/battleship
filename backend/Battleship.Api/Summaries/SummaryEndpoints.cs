using Battleship.Api.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Battleship.Api.Summaries;

public static class SummaryEndpoints
{
    public static IEndpointRouteBuilder MapSummaryEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/summaries", GetSummaries);

        return app;
    }

    private static async Task<IResult> GetSummaries(BattleshipDbContext db)
    {
        var summaries = await db.GameSummaries
            .OrderBy(summary => summary.TotalShots)
            .ThenBy(summary => summary.CompletedAtUtc)
            .Select(summary => new GameSummaryResponse(
                summary.GameId,
                summary.BoardSize,
                summary.ShipCount,
                summary.TotalShots,
                summary.CompletedAtUtc))
            .ToListAsync();

        return Results.Ok(summaries);
    }
}