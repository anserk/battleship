using Battleship.Core;

namespace Battleship.Api.Games;

public sealed record GameResponse(
    Guid GameId,
    int BoardSize,
    int ShotsFired,
    int ShipsRemaining,
    bool IsWon,
    IReadOnlyList<ShotResponse> Shots);

public sealed record ShotResponse(
    int X,
    int Y,
    ShotOutcome Outcome); // TODO: fix serialization for this.

public sealed record PostShotResponse(
    int X,
    int Y,
    ShotOutcome Outcome, // TODO: fix serialization for this.
    string? SunkShipName,
    int ShotsFired,
    int ShipsRemaining,
    bool IsWon);

public sealed record ShotRequest(
    int X,
    int Y
);

public static class GameDtos
{
    public static GameResponse ToGameResponse(Guid id, Game game) =>
        new(
            id,
            game.BoardSize,
            game.ShotsFired,
            game.ShipsRemaining,
            game.IsWon,
            game.Shots
                .Select(s => new ShotResponse(
                    s.Key.X,
                    s.Key.Y,
                    s.Value))
                .ToList());

    public static PostShotResponse ToPostShotResponse(ShotResult result) =>
        new(
            result.Coordinate.X,
            result.Coordinate.Y,
            result.Outcome,
            result.SunkShipName,
            result.ShotsFired,
            result.ShipsRemaining,
            result.IsWon);
}