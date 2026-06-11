# Architecture decisions
I decided to use a monorepo solution for this project with two main folders, `frontend` and `backend`.

## Backend
Backend is split in 3 projects:

### API
This project exposes the `Core` logic of the game via API endpoints; keeps an in-memory dictionary of active games and adds a persistence layer with `SQLite` for game summaries.

The API allows multiple games to be played at the same time; `ConcurrentDictionary` is used for dealing with concurrent access to a game.

Ship coordinates are never returned by the API; the client only receives public game state, shot outcomes, and the sunk ship name when applicable.

Duplicate shots and shots after a completed game are rejected with `409 Conflict`.

> Note: Active games are stored in memory while summaries are stored in SQLite.

### Core
The Core project contains the main game domain logic. It creates the board, places the fleet, and progresses the game when shots are fired.

It doesn't have any dependencies on the other layers of the application, this makes it self contained and easy to test.

#### Models
These are just some basic files to model the domain problem.
- Coordinate;
- DomainExceptions;
- Orientation;
- ShotOutcome;
- ShotResults;
- ShipDefinition.

#### Game
This file contains the main game logic, it handles what happens when a shot is fired, knows when a game is won and tracks some metrics for shots fired and ships remaining.

#### FleetPlacer
This file is responsible for placing the fleet on the board, making sure the board dimensions are valid and each ship in the fleet can fit on the board with no overlap.

### Ship
This file contains the ship logic, each ship keeps track of its own coordinates, and hits. 
It also determines whether a coordinate is a hit or miss and exposes whether the ship has sunk.

### Tests
This project tests the `API` and `Core` projects. 

For `Core` all the tests are unit tests without needing a database.
Main tests are the correctness of the game logic and fleet placement.

For `API` instead I am using integration tests, where an API is running with a real DB (in memory) so I can test the full cycle of the application. 
Testing that endpoint returns the expected response.
Testing that the persistence layer does store games summary correctly after a game is won.

#### Randomness tests
Just a small note about randomness, in order to test it in a deterministic way, both `Core` and `API` inject or use a factory to get their random generator service (`IRandomGenerator`); this way I can create a deterministic fake random generator in the tests project.

## Frontend
This project offers a UI to interact with our API. 

It allows a user to create a new game, play the game and see a leaderboard of completed games.

It is a single page app with a single route and no tests.

# Tradeoff and incomplete areas

Due to some time constraints there are some areas of the application that can be better and some 'Nice to Have' that I wasn't able to complete.

## Missing nice to haves
- No configurable board size and fleet;
- No "No adjacency" placement rule (ships can't touch);
- No docker compose support.

## Tradeoff

### Frontend
Focusing more on the backend left the frontend with no tests and no data validation. 

- add `Zod` to validate and parse API response;
- maybe split the routes in `game`, `summary` instead of a single page app;
- the backend supports rehydrating a game, but the frontend does not currently provide a resume flow.

### API
- Stricter request body validation for `ShotRequest`: validate non-negative coordinates;
- more structure, maybe something like `CQRS` and `Mediator`;
- Docker setup;
- hardcoded connection string and `CORS` setting;
- swagger documentation could be better.

### Core
- allow custom board size;
- allow custom fleet;
- allow no touching ship rule while placing fleet;
- the `100` iteration loop for the placement is not too great;
- validate ship definition (name is non empty, size).

# How to run the application
To run the backend API, `cd` into `backend/Battleship.Api` and run the following command: 
```bash
dotnet run
```

To run the backend tests, `cd` into `backend` and run the following command: 
```bash 
dotnet test
```

To run the frontend, `cd` into `frontend` and run the following command: 
```bash
pnpm install
pnpm dev
```

> ### Note
> .NET 10 SDK is required for the backend project.
>
> pnpm is required for the frontend project.
>
> You might need to update `frontend/src/api.ts` with the correct backend URL on line `8`.
>
> You might need to update `backend/Battleship.Api/Program.cs` with the correct url for origin on line `14`.
