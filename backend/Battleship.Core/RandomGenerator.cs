namespace Battleship.Core;

public sealed class SystemRandomSource : IRandomGenerator
{
    public int Next(int minValueInclusive, int maxValueExclusive)
        => Random.Shared.Next(minValueInclusive, maxValueExclusive);
}