namespace Battleship.Core;

public sealed class Game(
    int boardSize,
    IReadOnlyList<Ship> ships)
{
    private readonly Dictionary<Coordinate, ShotOutcome> _shots = [];

    private readonly IReadOnlyList<Ship> _ships = [.. ships];

    public int BoardSize => boardSize;

    public IReadOnlyList<Ship> Ships => _ships;

    public IReadOnlyDictionary<Coordinate, ShotOutcome> Shots => _shots;

    public bool IsWon =>
        _ships.Count > 0 && _ships.All(s => s.IsSunk);

    public int ShotsFired =>
        _shots.Count;

    public int ShipsRemaining =>
        _ships.Count(s => !s.IsSunk);

    public ShotResult FireShot(Coordinate coordinate)
    {
        if (IsWon)
        {
            throw new GameAlreadyWonException();
        }

        if (coordinate.X > BoardSize
            || coordinate.X < 1
            || coordinate.Y > BoardSize
            || coordinate.Y < 1)
        {
            throw new ShotOutOfBoundsException();
        }

        if (_shots.ContainsKey(coordinate))
        {
            throw new DuplicateShotException();
        }

        var ship = _ships.SingleOrDefault(s => s.Occupies(coordinate));

        if (ship == null)
        {
            _shots.Add(coordinate, ShotOutcome.Miss);
            return new ShotResult(
                ShotOutcome.Miss,
                coordinate,
                null,
                ShotsFired,
                ShipsRemaining,
                IsWon);
        }

        ship.RegisterHit(coordinate);
        var shotOutcome = ship.IsSunk ? ShotOutcome.Sunk : ShotOutcome.Hit;
        var sunkShipName = shotOutcome == ShotOutcome.Sunk
            ? ship.ShipDefinition.Name
            : null;

        _shots.Add(coordinate, shotOutcome);
        return new ShotResult(
            shotOutcome,
            coordinate,
            sunkShipName,
            ShotsFired,
            ShipsRemaining,
            IsWon);
    }
}