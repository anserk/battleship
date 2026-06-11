namespace Battleship.Core;

public sealed record ShotResult(
    ShotOutcome Outcome,
    Coordinate Coordinate,
    string? SunkShipName,
    int ShotsFired,
    int ShipsRemaining,
    bool IsWon);