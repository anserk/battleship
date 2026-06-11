namespace Battleship.Core;

public sealed class FleetPlacer
{
    public static IReadOnlyList<Ship> PlaceFleet(
        int boardSize,
        IReadOnlyList<ShipDefinition> fleet,
        IRandomGenerator random)
    {
        ArgumentNullException.ThrowIfNull(random);
        ArgumentNullException.ThrowIfNull(fleet);

        if (boardSize < 1)
        {
            throw new InvalidBoardSizeException();
        }

        if (fleet.Any(s => s is null))
        {
            throw new InvalidShipDefinitionException();
        }

        if (fleet.Count < 1)
        {
            throw new InvalidFleetSizeException();
        }

        if (fleet.Any(s => s.Length < 1 || s.Length > boardSize))
        {
            throw new InvalidShipSizeException();
        }

        if (fleet.Any(s => string.IsNullOrWhiteSpace(s.Name)))
        {
            throw new InvalidShipNameException();
        }

        List<Ship> ships = [];
        var occupied = new HashSet<Coordinate>();

        foreach (var shipDefinition in fleet)
        {
            var placed = false;

            // Use max attempt, avoid endless loop where placement is not possible.
            for (int attempt = 0; attempt < 100; attempt++)
            {
                var orientation = random.Next(0, 2) == 0
                    ? Orientation.Horizontal
                    : Orientation.Vertical;

                var startX = random.Next(1, boardSize + 1);
                var startY = random.Next(1, boardSize + 1);

                var coordinates = new List<Coordinate>();

                for (var offset = 0; offset < shipDefinition.Length; offset++)
                {
                    var x = orientation == Orientation.Horizontal
                        ? startX + offset
                        : startX;

                    var y = orientation == Orientation.Vertical
                        ? startY + offset
                        : startY;

                    coordinates.Add(new Coordinate(x, y));
                }

                var isInBounds = coordinates.All(c =>
                    c.X >= 1 &&
                    c.X <= boardSize &&
                    c.Y >= 1 &&
                    c.Y <= boardSize);

                var overlaps = coordinates.Any(occupied.Contains);

                // If the ship is not in bounds or is overlapping try to place again.
                if (!isInBounds || overlaps)
                {
                    continue;
                }

                var ship = new Ship(shipDefinition, coordinates);
                ships.Add(ship);

                foreach (var coordinate in coordinates)
                {
                    occupied.Add(coordinate);
                }

                placed = true;
                break;
            }

            // Check all the ships are placed.
            if (!placed)
            {
                throw new FleetPlacementException();
            }

        }

        return ships;
    }
}