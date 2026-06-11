namespace Battleship.Api.Summaries;

public sealed record GameSummaryResponse(
    Guid GameId,
    int BoardSize,
    int ShipCount,
    int TotalShots,
    DateTime CompletedAtUtc);