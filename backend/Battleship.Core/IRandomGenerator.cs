namespace Battleship.Core;

public interface IRandomGenerator
{
    int Next(int minValueInclusive, int maxValueExclusive);
}