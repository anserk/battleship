import { useMutation, useQueryClient } from "@tanstack/react-query";
import { shotOutcomeClasses, shotOutcomeLabels, type GameResponse } from "./types";
import { useState } from "react";
import { postGame, postShot } from "./api";


function GameView() {
    const queryClient = useQueryClient();
    const [game, setGame] = useState<GameResponse | null>(null);
    const [message, setMessage] = useState("");

    const newGameMutation = useMutation({
        mutationFn: postGame,
        onSuccess: (newGame) => {
            setGame(newGame);
            setMessage("New game started.");
        },
    });

    const shotMutation = useMutation({
        mutationFn: ({ x, y }: { x: number; y: number }) => {
            if (!game) {
                throw new Error("No active game.");
            }

            return postShot(game.gameId, x, y);
        },
        onSuccess: (result) => {
            if (!game) {
                return;
            }

            setGame((currentGame) => {
                if (!currentGame) {
                    return currentGame;
                }

                return {
                    ...currentGame,
                    shotsFired: result.shotsFired,
                    shipsRemaining: result.shipsRemaining,
                    isWon: result.isWon,
                    shots: [
                        ...currentGame.shots,
                        {
                            x: result.x,
                            y: result.y,
                            outcome: result.outcome,
                        },
                    ],
                };
            });


            if (result.isWon) {
                setMessage(`You won in ${result.shotsFired} shots!`);
                queryClient.invalidateQueries({ queryKey: ["summaries"] });
            } else if (result.sunkShipName) {
                setMessage(`You sunk the ${result.sunkShipName}.`);
            } else {
                setMessage(shotOutcomeLabels[result.outcome]);
            }
        },
    });

    const error = newGameMutation.error ?? shotMutation.error;
    const isStarting = newGameMutation.isPending;

    function getShot(x: number, y: number) {
        return game?.shots.find((shot) => shot.x === x && shot.y === y);
    }

    function handleNewGame() {
        setMessage("");
        newGameMutation.mutate();
    }

    function handleShot(x: number, y: number) {
        if (!game || game.isWon || getShot(x, y) || shotMutation.isPending) {
            return;
        }

        shotMutation.mutate({ x, y });
    }

    return (
        <>
            {error && (
                <p className="error">
                    {error instanceof Error ? error.message : "Something went wrong."}
                </p>
            )}

            {!game ? (
                <div className="message empty-game">
                    <span>Start a new game to begin firing shots.</span>
                    <button type="button" onClick={handleNewGame} disabled={isStarting}>
                        {isStarting ? "Starting..." : "New game"}
                    </button>
                </div>

            ) : (

                <>
                    <section className="status">
                        <span>Shots: {game.shotsFired}</span>
                        <span>Ships remaining: {game.shipsRemaining}</span>
                        <button type="button" onClick={handleNewGame} >
                            {isStarting ? "Starting..." : "New game"}
                        </button>
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
                                        onClick={() => handleShot(x, y)}
                                        disabled={Boolean(shot) || game.isWon || shotMutation.isPending}
                                        aria-label={`Fire at ${x}, ${y}`}
                                    >
                                        {shot ? shotOutcomeLabels[shot.outcome][0] : ""}
                                    </button>
                                );
                            },
                        )}
                    </section>
                </>
            )
            }
        </>

    );
}

export default GameView;