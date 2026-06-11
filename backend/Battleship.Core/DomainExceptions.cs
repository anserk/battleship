namespace Battleship.Core;

public sealed class InvalidBoardSizeException : Exception;
public sealed class InvalidFleetSizeException : Exception;
public sealed class InvalidShipSizeException : Exception;
public sealed class InvalidShipNameException : Exception;
public sealed class InvalidShipDefinitionException : Exception;
public sealed class FleetPlacementException : Exception;
public sealed class ShotOutOfBoundsException : Exception;
public sealed class DuplicateShotException : Exception;
public sealed class GameAlreadyWonException : Exception;
