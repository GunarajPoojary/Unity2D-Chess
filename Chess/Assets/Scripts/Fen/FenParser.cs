using System;

public class FenParser : IFenParser
{
    public GameStateData Parse(string fen)
    {
        string[] parts = fen.Split(' ');

        GameStateData state = new GameStateData();

        ParseBoard(parts[0], state);
        state.activeColor = parts[1] == "w" ? TeamColor.Light : TeamColor.Dark;

        return state;
    }

    private void ParseBoard(string boardPart, GameStateData state)
    {
        string[] ranks = boardPart.Split('/');

        for (int rank = 0; rank < 8; rank++)
        {
            int file = 0;

            foreach (char c in ranks[rank])
            {
                if (char.IsDigit(c))
                {
                    file += c - '0';
                }
                else
                {
                    state.boardData.Set(file, 7 - rank, CreatePiece(c));
                    file++;
                }
            }
        }
    }

    private PieceData CreatePiece(char c)
    {
        bool isLight = char.IsUpper(c);
        char lower = char.ToLower(c);

        TeamColor color = isLight ? TeamColor.Light : TeamColor.Dark;

        return lower switch
        {
            'p' => new PieceData { Type = PieceType.Pawn, Color = color },
            'r' => new PieceData { Type = PieceType.Rook, Color = color },
            'n' => new PieceData { Type = PieceType.Knight, Color = color },
            'b' => new PieceData { Type = PieceType.Bishop, Color = color },
            'q' => new PieceData { Type = PieceType.Queen, Color = color },
            'k' => new PieceData { Type = PieceType.King, Color = color },
            _ => throw new Exception("Invalid FEN char: " + c)
        };
    }
}