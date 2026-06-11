export type ShotOutcome = 0 | 1 | 2;

export type ShotResponse = {
  x: number;
  y: number;
  outcome: ShotOutcome;
};

export type GameResponse = {
  gameId: string;
  boardSize: number;
  shotsFired: number;
  shipsRemaining: number;
  isWon: boolean;
  shots: ShotResponse[];
};

export type PostShotResponse = {
  x: number;
  y: number;
  outcome: ShotOutcome;
  sunkShipName: string | null;
  shotsFired: number;
  shipsRemaining: number;
  isWon: boolean;
};

export type GameSummaryResponse = {
  gameId: string;
  boardSize: number;
  shipCount: number;
  totalShots: number;
  completedAtUtc: string;
};

export const shotOutcomeLabels: Record<ShotOutcome, "Miss" | "Hit" | "Sunk"> = {
  0: "Miss",
  1: "Hit",
  2: "Sunk",
};

export const shotOutcomeClasses: Record<ShotOutcome, string> = {
  0: "miss",
  1: "hit",
  2: "sunk",
};
