using System.Net;
using System.Net.Http.Json;
using Battleship.Api.Games;
using Battleship.Api.Summaries;

namespace Battleship.Tests.Integration;

public sealed class GameApiTests
{
    [Fact]
    public async Task GetGame_WhenGameDoesNotExist_ReturnsNotFound()
    {
        using var factory = new TestApplicationFactory();
        var client = factory.CreateClient();

        var response = await client.GetAsync($"/games/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task FireShot_WhenShotIsOutOfBounds_ReturnsBadRequest()
    {
        using var factory = new TestApplicationFactory();
        var client = factory.CreateClient();
        var game = await CreateGame(client);

        var response = await client.PostAsJsonAsync(
            $"/games/{game.GameId}/shots",
            new ShotRequest(11, 1));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task FireShot_WhenShotIsDuplicate_ReturnsConflict()
    {
        using var factory = new TestApplicationFactory();
        var client = factory.CreateClient();
        var game = await CreateGame(client);

        var request = new ShotRequest(10, 10);

        var firstResponse = await client.PostAsJsonAsync($"/games/{game.GameId}/shots", request);
        var secondResponse = await client.PostAsJsonAsync($"/games/{game.GameId}/shots", request);

        Assert.Equal(HttpStatusCode.OK, firstResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, secondResponse.StatusCode);
    }

    [Fact]
    public async Task FireShot_WhenGameAlreadyWon_ReturnsConflict()
    {
        using var factory = new TestApplicationFactory();
        var client = factory.CreateClient();
        var game = await CreateGame(client);

        foreach (var shot in WinningShots())
        {
            var response = await client.PostAsJsonAsync($"/games/{game.GameId}/shots", shot);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        var afterWinResponse = await client.PostAsJsonAsync(
            $"/games/{game.GameId}/shots",
            new ShotRequest(10, 10));

        Assert.Equal(HttpStatusCode.Conflict, afterWinResponse.StatusCode);
    }

    [Fact]
    public async Task FireShot_WhenGameIsCompleted_PersistsSummaryWithCorrectShotCount()
    {
        using var factory = new TestApplicationFactory();
        var client = factory.CreateClient();
        var game = await CreateGame(client);

        foreach (var shot in WinningShots())
        {
            var response = await client.PostAsJsonAsync($"/games/{game.GameId}/shots", shot);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        var summaries = await client.GetFromJsonAsync<List<GameSummaryResponse>>("/summaries");

        Assert.NotNull(summaries);

        var summary = Assert.Single(summaries, s => s.GameId == game.GameId);
        Assert.Equal(10, summary.BoardSize);
        Assert.Equal(5, summary.ShipCount);
        Assert.Equal(17, summary.TotalShots);
    }

    private static async Task<GameResponse> CreateGame(HttpClient client)
    {
        var response = await client.PostAsync("/games", null);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var game = await response.Content.ReadFromJsonAsync<GameResponse>();

        Assert.NotNull(game);
        return game;
    }

    private static IEnumerable<ShotRequest> WinningShots()
    {
        // Matches deterministic ship placement in TestApplicationFactory.
        for (var x = 1; x <= 5; x++)
        {
            yield return new ShotRequest(x, 1);
        }

        for (var x = 1; x <= 4; x++)
        {
            yield return new ShotRequest(x, 2);
        }

        for (var x = 1; x <= 3; x++)
        {
            yield return new ShotRequest(x, 3);
        }

        for (var x = 1; x <= 3; x++)
        {
            yield return new ShotRequest(x, 4);
        }

        for (var x = 1; x <= 2; x++)
        {
            yield return new ShotRequest(x, 5);
        }
    }
}