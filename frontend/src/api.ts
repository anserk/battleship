import type {
  GameResponse,
  GameSummaryResponse,
  PostShotResponse,
} from "./types";

const API_BASE_URL =
  import.meta.env.VITE_API_BASE_URL ?? " http://localhost:5256";

async function parseJson<T>(
  response: Response,
  fallbackMessage: string,
): Promise<T> {
  if (!response.ok) {
    throw new Error(fallbackMessage);
  }

  return response.json() as Promise<T>;
}

export async function postGame() {
  const response = await fetch(`${API_BASE_URL}/games`, {
    method: "POST",
  });

  return parseJson<GameResponse>(response, "Could not create game.");
}

export async function postShot(gameId: string, x: number, y: number) {
  const response = await fetch(`${API_BASE_URL}/games/${gameId}/shots`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({ x, y }),
  });

  return parseJson<PostShotResponse>(response, "Could not fire shot.");
}

export async function getSummaries() {
  const response = await fetch(`${API_BASE_URL}/summaries`);

  return parseJson<GameSummaryResponse[]>(
    response,
    "Could not load summaries.",
  );
}
