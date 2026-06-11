import { useState } from "react";
import "./App.css";
import GameView from "./Game";
import SummaryView from "./Summary";

type ActiveTab = "game" | "summaries";

function App() {

  const [activeTab, setActiveTab] = useState<ActiveTab>("game");

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

      <div hidden={activeTab != "game"} >
        <GameView />
      </div>

      {
        activeTab === "summaries" && (
          <SummaryView />
        )
      }
    </main >
  );
}

export default App;