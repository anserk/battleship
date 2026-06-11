using Battleship.Core;

namespace Battleship.Tests;

public class FleetPlacerTests
{
    private sealed class FakeRandomGenerator(params int[] values) : IRandomGenerator
    {
        private readonly Queue<int> _values = new(values);

        public int Next(
            int minValueInclusive,
            int maxValueExclusive
        ) =>
            _values.Dequeue();
    }

    #region Input validation

    [Fact]
    public void PlaceFleet_WhenRandomIsNull_ThrowsArgumentNullException()
    {
        var fleet = new[]
        {
            new ShipDefinition("Destroyer", 2)
        };

        Assert.Throws<ArgumentNullException>(() =>
            FleetPlacer.PlaceFleet(10, fleet, null!));
    }

    [Fact]
    public void PlaceFleet_WhenFleetIsNull_ThrowsArgumentNullException()
    {
        var randomGenerator = new FakeRandomGenerator(1);
        Assert.Throws<ArgumentNullException>(() =>
            FleetPlacer.PlaceFleet(10, null!, randomGenerator));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void PlaceFleet_WhenInvalidBoardSize_ThrowsInvalidBoardSizeException(int boardSize)
    {
        var randomGenerator = new FakeRandomGenerator(1);
        var fleet = new[]
        {
            new ShipDefinition("Destroyer", 2)
        };
        Assert.Throws<InvalidBoardSizeException>(() =>
            FleetPlacer.PlaceFleet(boardSize, fleet, randomGenerator));
    }

    [Fact]
    public void PlaceFleet_WhenNullShip_ThrowsInvalidShipDefinitionException()
    {
        var randomGenerator = new FakeRandomGenerator(1);
        var fleet = new List<ShipDefinition> { null! };

        Assert.Throws<InvalidShipDefinitionException>(() =>
            FleetPlacer.PlaceFleet(2, fleet, randomGenerator));
    }

    [Fact]
    public void PlaceFleet_WhenInvalidFleetSize_ThrowsInvalidFleetSizeException()
    {
        var randomGenerator = new FakeRandomGenerator(1);
        var fleet = new List<ShipDefinition>();
        Assert.Throws<InvalidFleetSizeException>(() =>
            FleetPlacer.PlaceFleet(2, fleet, randomGenerator));
    }

    [Fact]
    public void PlaceFleet_WhenShipSizeIsZero_ThrowsInvalidShipSizeException()
    {
        var randomGenerator = new FakeRandomGenerator(1);
        var fleet = new[]
        {
            new ShipDefinition("Destroyer", 0)
        };

        Assert.Throws<InvalidShipSizeException>(() =>
            FleetPlacer.PlaceFleet(2, fleet, randomGenerator));
    }

    [Fact]
    public void PlaceFleet_WhenShipLengthExceedsBoardSize_ThrowsInvalidShipSizeException()
    {
        var randomGenerator = new FakeRandomGenerator(1);
        var fleet = new[]
        {
            new ShipDefinition("Destroyer", 4)
        };

        Assert.Throws<InvalidShipSizeException>(() =>
            FleetPlacer.PlaceFleet(2, fleet, randomGenerator));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void PlaceFleet_WhenShipNameIsInvalid_ThrowsInvalidShipNameException(string? shipName)
    {
        var randomGenerator = new FakeRandomGenerator(1);
        var fleet = new[]
        {
            new ShipDefinition(shipName!, 1)
        };

        Assert.Throws<InvalidShipNameException>(() =>
            FleetPlacer.PlaceFleet(2, fleet, randomGenerator));
    }

    #endregion

    #region Happy path

    [Fact]
    public void PlaceFleet_PlacesEveryShip()
    {
        var fleet = new[]
        {
            new ShipDefinition("Destroyer", 2),
            new ShipDefinition("Submarine", 3)
        };
        var randomGenerator = new FakeRandomGenerator(
            0, 1, 1,
            1, 3, 2);

        var ships = FleetPlacer.PlaceFleet(5, fleet, randomGenerator);

        Assert.Equal(2, ships.Count);
        Assert.Equal(fleet[0], ships[0].ShipDefinition);
        Assert.Equal(fleet[1], ships[1].ShipDefinition);
        Assert.Equal(
            [new Coordinate(1, 1), new Coordinate(2, 1)],
            ships[0].Coordinates);
        Assert.Equal(
            [new Coordinate(3, 2), new Coordinate(3, 3), new Coordinate(3, 4)],
            ships[1].Coordinates);
    }

    [Fact]
    public void PlaceFleet_DoesNotOverlapShips()
    {
        var fleet = new[]
            {
            new ShipDefinition("Destroyer", 2),
            new ShipDefinition("Submarine", 3),
            new ShipDefinition("Cruiser", 3)
        };
        var randomGenerator = new FakeRandomGenerator(
            0, 1, 1,
            0, 1, 2,
            1, 5, 1);

        var ships = FleetPlacer.PlaceFleet(5, fleet, randomGenerator);

        var coordinates = ships.SelectMany(s => s.Coordinates).ToList();

        Assert.Equal(coordinates.Count, coordinates.Distinct().Count());
    }

    #endregion

    #region Placement fails

    [Fact]
    public void PlaceFleet_WhenShipCannotBePlaced_ThrowsFleetPlacementException()
    {
        var fleet = new[]
        {
            new ShipDefinition("Destroyer", 2)
        };

        var randomGenerator = new FakeRandomGenerator(
            [.. Enumerable.Repeat(new[] { 0, 2, 1 }, 100).SelectMany(values => values)]);

        Assert.Throws<FleetPlacementException>(() =>
            FleetPlacer.PlaceFleet(2, fleet, randomGenerator));
    }

    #endregion
}
