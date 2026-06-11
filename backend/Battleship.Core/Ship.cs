namespace Battleship.Core;

public sealed class Ship(
    ShipDefinition shipDefinition,
    IEnumerable<Coordinate> coordinates)
{
    private readonly HashSet<Coordinate> _coordinates = [.. coordinates];

    private readonly HashSet<Coordinate> _hits = [];

    public ShipDefinition ShipDefinition => shipDefinition;

    public IReadOnlySet<Coordinate> Coordinates => _coordinates;

    public IReadOnlySet<Coordinate> Hits => _hits;

    public bool Occupies(Coordinate coordinate) =>
        _coordinates.Contains(coordinate);

    public bool IsSunk =>
        _coordinates.Count > 0 && _coordinates.All(_hits.Contains);

    public bool RegisterHit(Coordinate coordinate)
    {
        if (!Occupies(coordinate))
        {
            return false;
        }

        return _hits.Add(coordinate);
    }
}