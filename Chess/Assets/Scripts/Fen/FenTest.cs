using UnityEngine;

public class FenTest : MonoBehaviour
{
    public string fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w - - 0 1";
    
    [ContextMenu("Test")]
    void Start()
    {
        FenParser parser = new FenParser();
        GameStateData state = parser.Parse(fen);

        Debug.Log("Parsed board:");

        FenGenerator generator = new FenGenerator();
        string generatedFen = generator.Generate(state);

        Debug.Log("Generated FEN:");
        Debug.Log(generatedFen);
    }

    // Debug helper
    private string BoardToString(PieceData[,] board)
    {
        string result = "";

        for (int rank = 7; rank >= 0; rank--)
        {
            for (int file = 0; file < 8; file++)
            {
                PieceData piece = board[rank, file];
                result += piece == null ? "." : GetChar(piece);
                result += " ";
            }
            result += "\n";
        }

        return result;
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