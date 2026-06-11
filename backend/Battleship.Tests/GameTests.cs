namespace Battleship.Tests;

using Battleship.Core;

public class GameTests
{
    private static Game CreateGame()
    {
        var ship = new Ship(
            new ShipDefinition("Destroyer", 2),
            [
                new Coordinate(1, 1),
                new Coordinate(2, 1)
            ]);

        return new Game(5, [ship]);
    }

    [Fact]
    public void FireShot_WhenCoordinateDoesNotHitShip_ReturnsMiss()
    {
        var game = CreateGame();
        var coordinate = new Coordinate(5, 5);

        var result = game.FireShot(coordinate);

        Assert.Equal(ShotOutcome.Miss, result.Outcome);
        Assert.Equal(coordinate, result.Coordinate);
        Assert.Null(result.SunkShipName);
        Assert.Equal(1, result.ShotsFired);
        Assert.Equal(1, result.ShipsRemaining);
        Assert.False(result.IsWon);
    }

    [Fact]
    public void FireShot_WhenCoordinateHitsShip_ReturnsHit()
    {
        var game = CreateGame();
        var coordinate = new Coordinate(1, 1);

        var result = game.FireShot(coordinate);

        Assert.Equal(ShotOutcome.Hit, result.Outcome);
        Assert.Equal(coordinate, result.Coordinate);
        Assert.Null(result.SunkShipName);
        Assert.Equal(1, result.ShotsFired);
        Assert.Equal(1, result.ShipsRemaining);
        Assert.False(result.IsWon);
    }

    [Fact]
    public void FireShot_WhenCoordinateSinksShip_ReturnsSunk()
    {
        var game = CreateGame();
        game.FireShot(new Coordinate(1, 1));

        var result = game.FireShot(new Coordinate(2, 1));

        Assert.Equal(ShotOutcome.Sunk, result.Outcome);
        Assert.Equal(new Coordinate(2, 1), result.Coordinate);
        Assert.Equal("Destroyer", result.SunkShipName);
        Assert.Equal(2, result.ShotsFired);
        Assert.Equal(0, result.ShipsRemaining);
        Assert.True(result.IsWon);
    }

    [Fact]
    public void FireShot_WhenCoordinateIsOutOfBounds_ThrowsShotOutOfBoundsException()
    {
        var game = CreateGame();

        Assert.Throws<ShotOutOfBoundsException>(() =>
            game.FireShot(new Coordinate(6, 1)));
    }

    [Fact]
    public void FireShot_WhenCoordinateWasAlreadyShot_ThrowsDuplicateShotException()
    {
        var game = CreateGame();
        game.FireShot(new Coordinate(5, 5));

        Assert.Throws<DuplicateShotException>(() =>
            game.FireShot(new Coordinate(5, 5)));
    }

    [Fact]
    public void FireShot_WhenGameAlreadyWon_ThrowsGameAlreadyWonException()
    {
        var game = CreateGame();
        game.FireShot(new Coordinate(1, 1));
        game.FireShot(new Coordinate(2, 1));

        Assert.Throws<GameAlreadyWonException>(() =>
            game.FireShot(new Coordinate(5, 5)));
    }
}
