import type { GameSummaryResponse } from "./types";

type SummaryViewProps = {
    summaries: GameSummaryResponse[];
    isLoading: boolean;
    onRefresh: () => void;
};

function SummaryView({ summaries, isLoading, onRefresh }: SummaryViewProps) {
    return (
        <section className="summary-list">
            <div className="summary-header">
                <div>
                    <p className="eyebrow">Leaderboard</p>
                    <h2>Completed games</h2>
                </div>

                <button type="button" onClick={onRefresh} disabled={isLoading}>
                    {isLoading ? "Loading..." : "Refresh"}
                </button>
            </div>

            {summaries.length === 0 ? (
                <p className="message">No completed games yet.</p>
            ) : (
                <ol>
                    {summaries.map((summary) => (
                        <li key={summary.gameId}>
                            <span>{summary.totalShots} shots</span>
                            <span>{summary.shipCount} ships</span>
                            <span>
                                {summary.boardSize}x{summary.boardSize}
                            </span>
                            <span>{new Date(summary.completedAtUtc).toLocaleString()}</span>
                        </li>
                    ))}
                </ol>
            )}
        </section>
    );
}

export default SummaryView;