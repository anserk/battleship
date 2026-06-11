using Battleship.Core;

namespace Battleship.Api.Games;

public sealed class GameFactory(IRandomGenerator random)
{
    public Game Create(int boardSize = 10)
    {
        var ships = FleetPlacer.PlaceFleet(
            boardSize,
            ShipDefinition.StandardFleet,
            random);

        return new Game(boardSize, ships);
    }
}