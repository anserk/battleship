namespace Battleship.Core;

public sealed class SystemRandomSource : IRandomGenerator
{
    private readonly Random _random = new();

    public int Next(int minValueInclusive, int maxValueExclusive)
        => _random.Next(minValueInclusive, maxValueExclusive);
}