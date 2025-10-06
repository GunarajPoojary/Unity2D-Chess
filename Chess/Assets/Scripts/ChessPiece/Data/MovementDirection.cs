/// <summary>
/// Represents the forward movement direction of pieces relative to the board.
/// Used by pawns to determine which way is forward.
/// Independent of piece color.
/// </summary>
public enum MovementDirection
{
    None = 0,
    Up = 1,    // moves toward the top of the board
    Down = -1  // moves toward the bottom of the board
}