import { shotOutcomeClasses, shotOutcomeLabels, type GameResponse } from "./types";

type GameViewProps = {
    game: GameResponse | null;
    message: string;
    onShot: (x: number, y: number) => void;
};

function GameView({ game, message, onShot }: GameViewProps) {
    function getShot(x: number, y: number) {
        return game?.shots.find((shot) => shot.x === x && shot.y === y);
    }

    if (!game) {
        return <p className="message">Start a new game to begin firing shots.</p>;
    }

    return (
        <>
            <section className="status">
                <span>Shots: {game.shotsFired}</span>
                <span>Ships remaining: {game.shipsRemaining}</span>
                <span>{game.isWon ? "Won" : "In progress"}</span>
            </section>

            {message && <p className="message">{message}</p>}

            <section
                className="board"
                style={{
                    gridTemplateColumns: `repeat(${game.boardSize}, minmax(0, 1fr))`,
                }}
                aria-label="Battleship board"
            >
                {Array.from(
                    { length: game.boardSize * game.boardSize },
                    (_, index) => {
                        const x = (index % game.boardSize) + 1;
                        const y = Math.floor(index / game.boardSize) + 1;
                        const shot = getShot(x, y);
                        const className = shot
                            ? `cell ${shotOutcomeClasses[shot.outcome]}`
                            : "cell";

                        return (
                            <button
                                key={`${x}-${y}`}
                                type="button"
                                className={className}
                                onClick={() => onShot(x, y)}
                                disabled={Boolean(shot) || game.isWon}
                                aria-label={`Fire at ${x}, ${y}`}
                            >
                                {shot ? shotOutcomeLabels[shot.outcome][0] : ""}
                            </button>
                        );
                    },
                )}
            </section>
        </>
    );
}

export default GameView;