namespace Battleship.Core;

public sealed record ShipDefinition(string Name, int Length)
{
    public static readonly IReadOnlyList<ShipDefinition> StandardFleet =
    [
        new("Carrier", 5),
        new("Battleship", 4),
        new("Cruiser", 3),
        new("Submarine", 3),
        new("Destroyer", 2)
    ];
}
