using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Battleship.Core;

namespace Battleship.Api.Games;

public sealed class InMemoryGameStore : IGameStore
{
    private sealed record StoredGame(Game Game, object SyncRoot);

    private readonly ConcurrentDictionary<Guid, StoredGame> _games = [];

    public Guid Create(Game game)
    {
        var id = Guid.NewGuid();
        _games[id] = new StoredGame(game, new object());
        return id;
    }

    public bool TryUseGame<TResult>(
        Guid id,
        Func<Game, TResult> action,
        [NotNullWhen(true)] out TResult? result)
    {
        if (!_games.TryGetValue(id, out var storedGame))
        {
            result = default;
            return false;
        }

        lock (storedGame.SyncRoot)
        {
            result = action(storedGame.Game)!;
            return true;
        }
    }
}