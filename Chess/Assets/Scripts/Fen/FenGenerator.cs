public class FenGenerator : IFenGenerator
{
    public string Generate(GameStateData state)
    {
        string board = GenerateBoard(state.boardData);
        string active = state.activeColor == TeamColor.Light ? "w" : "b";

        return $"{board} {active} - - 0 1";
    }

    private string GenerateBoard(Grid<PieceData> boardState)
    {
        string fen = "";

        for (int rank = 7; rank >= 0; rank--)
        {
            int empty = 0;

            for (int file = 0; file < 8; file++)
            {
                PieceData piece = boardState.Get(file, rank);

                if (piece != null)
                {
                    empty++;
                }
                else
                {
                    if (empty > 0)
                    {
                        fen += empty;
                        empty = 0;
                    }

                    fen += GetChar(piece);
                }
            }

            if (empty > 0)
                fen += empty;

            if (rank > 0)
                fen += "/";
        }

        return fen;
    }

    private char GetChar(PieceData piece)
    {
        char c = piece.Type switch
        {
            PieceType.Pawn => 'p',
            PieceType.Rook => 'r',
            PieceType.Knight => 'n',
            PieceType.Bishop => 'b',
            PieceType.Queen => 'q',
            PieceType.King => 'k',
            _ => '?'
        };

        return piece.Color == TeamColor.Light ? char.ToUpper(c) : c;
    }
}