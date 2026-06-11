using System.Diagnostics.CodeAnalysis;
using Battleship.Core;

namespace Battleship.Api.Games;

public interface IGameStore
{
    Guid Create(Game game);
    bool TryUseGame<TResult>(
        Guid id,
        Func<Game, TResult> action,
        [NotNullWhen(true)] out TResult? result);
}