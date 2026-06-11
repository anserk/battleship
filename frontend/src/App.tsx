import { useState } from "react";
import "./App.css";
import {
  type GameResponse,
  type GameSummaryResponse,
  shotOutcomeLabels,
} from "./types";
import { getSummaries, postGame, postShot } from "./api";
import GameView from "./Game";
import SummaryView from "./Summary";

type ActiveTab = "game" | "summaries";

function App() {

  const [game, setGame] = useState<GameResponse | null>(null);
  const [message, setMessage] = useState("");
  const [error, setError] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [activeTab, setActiveTab] = useState<ActiveTab>("game");
  const [summaries, setSummaries] = useState<GameSummaryResponse[]>([]);
  const [isSummaryLoading, setIsSummaryLoading] = useState(false);

  async function handleNewGame() {
    setIsLoading(true);
    setError("");
    setMessage("");

    try {
      const newGame = await postGame();
      setGame(newGame);
      setActiveTab("game");
      setMessage("New game started.");
    } catch (error) {
      setError(error instanceof Error ? error.message : "Something went wrong.");
    } finally {
      setIsLoading(false);
    }
  }

  async function handleShot(x: number, y: number) {
    if (!game || game.isWon || getShot(x, y)) {
      return;
    }

    setError("");

    try {
      const result = await postShot(game.gameId, x, y);

      setGame({
        ...game,
        shotsFired: result.shotsFired,
        shipsRemaining: result.shipsRemaining,
        isWon: result.isWon,
        shots: [
          ...game.shots,
          {
            x: result.x,
            y: result.y,
            outcome: result.outcome,
          },
        ],
      });

      if (result.isWon) {
        setMessage(`You won in ${result.shotsFired} shots!`);
      } else if (result.sunkShipName) {
        setMessage(`You sunk the ${result.sunkShipName}.`);
      } else {
        setMessage(shotOutcomeLabels[result.outcome]);
      }
    } catch (error) {
      setError(error instanceof Error ? error.message : "Something went wrong.");
    }
  }

  async function handleLoadSummaries() {
    setIsSummaryLoading(true);
    setError("");

    try {
      const data = await getSummaries();
      setSummaries(data);
    } catch (error) {
      setError(error instanceof Error ? error.message : "Something went wrong.");
    } finally {
      setIsSummaryLoading(false);
    }
  }

  function getShot(x: number, y: number) {
    return game?.shots.find((shot) => shot.x === x && shot.y === y);
  }

  return (
    <main className="app">
      <section className="panel">
        <div>
          <p className="eyebrow">Battleship</p>
          <h1>Fire at the hidden fleet</h1>
          <p className="summary">
            Pick cells to find and sink every ship. Ship locations stay hidden
            until you win.
          </p>
        </div>

        <button type="button" onClick={handleNewGame} disabled={isLoading}>
          {isLoading ? "Starting..." : "New game"}
        </button>
      </section>

      <nav className="tabs" aria-label="Battleship views">
        <button
          type="button"
          className={activeTab === "game" ? "active" : ""}
          onClick={() => setActiveTab("game")}
        >
          Game
        </button>

        <button
          type="button"
          className={activeTab === "summaries" ? "active" : ""}
          onClick={() => setActiveTab("summaries")}
        >
          Summaries
        </button>
      </nav>

      {error && <p className="error">{error}</p>}

      {activeTab === "game" && (
        <GameView game={game} message={message} onShot={handleShot} />
      )}

      {activeTab === "summaries" && (
        <SummaryView
          summaries={summaries}
          isLoading={isSummaryLoading}
          onRefresh={handleLoadSummaries}
        />
      )}
    </main>
  );
}

export default App;