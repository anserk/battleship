using Battleship.Core;

namespace Battleship.Tests;

public class ShipTests
{
    private static Ship CreateShip() =>
        new(
            new ShipDefinition("Destroyer", 2),
            [
                new Coordinate(1, 1),
                new Coordinate(2, 1)
            ]);

    [Fact]
    public void Occupies_WhenCoordinateBelongsToShip_ReturnsTrue()
    {
        var ship = CreateShip();

        var occupies = ship.Occupies(new Coordinate(1, 1));

        Assert.True(occupies);
    }

    [Fact]
    public void Occupies_WhenCoordinateDoesNotBelongToShip_ReturnsFalse()
    {
        var ship = CreateShip();

        var occupies = ship.Occupies(new Coordinate(5, 5));

        Assert.False(occupies);
    }

    [Fact]
    public void RegisterHit_WhenCoordinateBelongsToShip_ReturnsTrue()
    {
        var ship = CreateShip();

        var registered = ship.RegisterHit(new Coordinate(1, 1));

        Assert.True(registered);
        Assert.Contains(new Coordinate(1, 1), ship.Hits);
    }

    [Fact]
    public void RegisterHit_WhenCoordinateDoesNotBelongToShip_ReturnsFalse()
    {
        var ship = CreateShip();

        var registered = ship.RegisterHit(new Coordinate(5, 5));

        Assert.False(registered);
        Assert.Empty(ship.Hits);
    }

    [Fact]
    public void RegisterHit_WhenCoordinateWasAlreadyHit_ReturnsFalse()
    {
        var ship = CreateShip();

        ship.RegisterHit(new Coordinate(1, 1));
        var registered = ship.RegisterHit(new Coordinate(1, 1));

        Assert.False(registered);
        Assert.Single(ship.Hits);
    }

    [Fact]
    public void IsSunk_WhenNoCoordinatesAreHit_ReturnsFalse()
    {
        var ship = CreateShip();

        Assert.False(ship.IsSunk);
    }

    [Fact]
    public void IsSunk_WhenSomeCoordinatesAreHit_ReturnsFalse()
    {
        var ship = CreateShip();

        ship.RegisterHit(new Coordinate(1, 1));

        Assert.False(ship.IsSunk);
    }

    [Fact]
    public void IsSunk_WhenAllCoordinatesAreHit_ReturnsTrue()
    {
        var ship = CreateShip();

        ship.RegisterHit(new Coordinate(1, 1));
        ship.RegisterHit(new Coordinate(2, 1));

        Assert.True(ship.IsSunk);
    }
}
