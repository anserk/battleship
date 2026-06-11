namespace Battleship.Api.Summaries;

public sealed class GameSummary
{
    public int Id { get; set; }
    public Guid GameId { get; set; }
    public int BoardSize { get; set; }
    public int ShipCount { get; set; }
    public int TotalShots { get; set; }
    public DateTime CompletedAtUtc { get; set; }
}