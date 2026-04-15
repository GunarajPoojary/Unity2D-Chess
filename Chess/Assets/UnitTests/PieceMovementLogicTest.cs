using NUnit.Framework;
using System.Collections.Generic;

namespace Chess
{
    [TestFixture]
    public class PieceMovementLogicTest
    {
        private BoardState _boardState;

        [SetUp]
        public void Setup()
        {
            FenParser parser = new FenParser();
            GameStateData gameState = parser.Parse("8/8/8/3P4/8/8/8/8 w - - 0 1");
            _boardState = new BoardState(gameState.boardData);
        }

        // PAWN — Single Step
        [Test]
        public void PawnSingleStep_ValidMove()
        {
            PawnSingleStepMove strategy = new PawnSingleStepMove(true);
            List<Move> moves = new List<Move>();

            TileData start = new TileData(3, 4); // d5

            bool result = strategy.TryGetMoves(_boardState, TeamColor.Light, start, moves.Add);

            Assert.IsTrue(result);
            Assert.AreEqual(1, moves.Count);
            Assert.AreEqual(new TileData(3, 5), moves[0].To);
        }

        [Test]
        public void PawnSingleStep_Blocked()
        {
            // The pawn at d5 (3,4) is blocking the square directly in front of (3,3)
            PawnSingleStepMove strategy = new PawnSingleStepMove(true);
            List<Move> moves = new List<Move>();

            TileData start = new TileData(3, 3); // d4 — d5 is occupied

            bool result = strategy.TryGetMoves(_boardState, TeamColor.Light, start, moves.Add);

            Assert.IsFalse(result);
            Assert.AreEqual(0, moves.Count);
        }

        [Test]
        public void PawnSingleStep_OutOfBoard()
        {
            PawnSingleStepMove strategy = new PawnSingleStepMove(true);
            List<Move> moves = new List<Move>();

            TileData start = new TileData(3, 7); // d8 — one step upward leaves the board

            bool result = strategy.TryGetMoves(_boardState, TeamColor.Light, start, moves.Add);

            Assert.IsFalse(result);
            Assert.AreEqual(0, moves.Count);
        }

        // PAWN — Double Step
        [Test]
        public void PawnDoubleStep_ValidMove()
        {
            PawnDoubleStepMove strategy = new PawnDoubleStepMove(true);
            List<Move> moves = new List<Move>();

            TileData start = new TileData(3, 4); // d5 — both d6 and d7 are empty

            bool result = strategy.TryGetMoves(_boardState, TeamColor.Light, start, moves.Add);

            Assert.IsTrue(result);
            Assert.AreEqual(1, moves.Count);
            Assert.AreEqual(new TileData(3, 6), moves[0].To);
        }

        [Test]
        public void PawnDoubleStep_FirstTileBlocked()
        {
            PawnDoubleStepMove strategy = new PawnDoubleStepMove(true);
            List<Move> moves = new List<Move>();

            TileData start = new TileData(3, 3); // d4 — d5 is occupied (first tile blocked)

            bool result = strategy.TryGetMoves(_boardState, TeamColor.Light, start, moves.Add);

            Assert.IsFalse(result);
            Assert.AreEqual(0, moves.Count);
        }

        [Test]
        public void PawnDoubleStep_SecondTileBlocked()
        {
            // Place a second piece at d6 (3,5) to block the second step from d4
            // We re-parse a board with pawns at d5 and d6
            FenParser parser = new FenParser();
            GameStateData state = parser.Parse("8/8/3P4/3P4/8/8/8/8 w - - 0 1");
            BoardState board = new BoardState(state.boardData);

            PawnDoubleStepMove strategy = new PawnDoubleStepMove(true);
            List<Move> moves = new List<Move>();

            TileData start = new TileData(3, 3); // d4 — d5 is empty but d6 is occupied

            bool result = strategy.TryGetMoves(board, TeamColor.Light, start, moves.Add);

            Assert.IsFalse(result);
            Assert.AreEqual(0, moves.Count);
        }

        [Test]
        public void PawnDoubleStep_OutOfBoard()
        {
            PawnDoubleStepMove strategy = new PawnDoubleStepMove(true);
            List<Move> moves = new List<Move>();

            TileData start = new TileData(3, 7); // d8 — two steps go out of board

            bool result = strategy.TryGetMoves(_boardState, TeamColor.Light, start, moves.Add);

            Assert.IsFalse(result);
            Assert.AreEqual(0, moves.Count);
        }

        // PAWN — Capture
        [Test]
        public void PawnCapture_CanCaptureOpponent()
        {
            // Light pawn at d5 (3,4), dark pieces at c6 (2,5) and e6 (4,5)
            FenParser parser = new FenParser();
            GameStateData state = parser.Parse("8/8/2pPp3/3P4/8/8/8/8 w - - 0 1");
            BoardState board = new BoardState(state.boardData);

            PawnCaptureMove strategy = new PawnCaptureMove(true);
            List<Move> moves = new List<Move>();

            TileData start = new TileData(3, 4); // d5

            bool result = strategy.TryGetMoves(board, TeamColor.Light, start, moves.Add);

            Assert.IsTrue(result);
            Assert.AreEqual(2, moves.Count);
        }

        [Test]
        public void PawnCapture_NoOpponent_NoMove()
        {
            // Default board — no enemy pieces on capture diagonals
            PawnCaptureMove strategy = new PawnCaptureMove(true);
            List<Move> moves = new List<Move>();

            TileData start = new TileData(3, 4); // d5

            bool result = strategy.TryGetMoves(_boardState, TeamColor.Light, start, moves.Add);

            Assert.IsFalse(result);
            Assert.AreEqual(0, moves.Count);
        }

        [Test]
        public void PawnCapture_FriendlyPiece_NoCapture()
        {
            // Friendly pieces on diagonals should not be capturable
            FenParser parser = new FenParser();
            // Light pawns at c6 (2,5) and e6 (4,5), light pawn at d5 (3,4)
            GameStateData state = parser.Parse("8/8/2PPP3/3P4/8/8/8/8 w - - 0 1");
            BoardState board = new BoardState(state.boardData);

            PawnCaptureMove strategy = new PawnCaptureMove(true);
            List<Move> moves = new List<Move>();

            TileData start = new TileData(3, 4); // d5

            bool result = strategy.TryGetMoves(board, TeamColor.Light, start, moves.Add);

            Assert.IsFalse(result);
            Assert.AreEqual(0, moves.Count);
        }

        [Test]
        public void PawnCapture_Downward_CanCaptureOpponent()
        {
            // Dark pawn moving downward from d5; light pieces at c4 and e4
            FenParser parser = new FenParser();
            GameStateData state = parser.Parse("8/8/8/3p4/2P1P3/8/8/8 w - - 0 1");
            BoardState board = new BoardState(state.boardData);

            PawnCaptureMove strategy = new PawnCaptureMove(false); // downward (dark pawn)
            List<Move> moves = new List<Move>();

            TileData start = new TileData(3, 4); // d5

            bool result = strategy.TryGetMoves(board, TeamColor.Dark, start, moves.Add);

            Assert.IsTrue(result);
            Assert.AreEqual(2, moves.Count);
        }

        // ROOK
        [Test]
        public void Rook_OpenBoard_HasMovesInAllDirections()
        {
            // Rook alone on an otherwise empty board at d4 (3,3)
            FenParser parser = new FenParser();
            GameStateData state = parser.Parse("8/8/8/8/3R4/8/8/8 w - - 0 1");
            BoardState board = new BoardState(state.boardData);

            RookMoveStrategy strategy = new RookMoveStrategy();
            List<Move> moves = new List<Move>();

            TileData start = new TileData(3, 3); // d4

            bool result = strategy.TryGetMoves(board, TeamColor.Light, start, moves.Add);

            // 7 up + 7 right + 3 down + 3 left = 7+4+3+3 = 7+7 = 14
            // col 3: rows 0-2 down (3), rows 4-7 up (4); row 3: cols 0-2 left (3), cols 4-7 right (4) = 14
            Assert.IsTrue(result);
            Assert.AreEqual(14, moves.Count);
        }

        [Test]
        public void Rook_BlockedByFriendly_StopsBeforeFriendly()
        {
            // Rook at d4, friendly piece at d6 — rook can reach d5 but not d6
            FenParser parser = new FenParser();
            GameStateData state = parser.Parse("8/8/3R4/8/3R4/8/8/8 w - - 0 1");
            BoardState board = new BoardState(state.boardData);

            RookMoveStrategy strategy = new RookMoveStrategy();
            List<Move> moves = new List<Move>();

            TileData start = new TileData(3, 3); // lower rook at d4

            strategy.TryGetMoves(board, TeamColor.Light, start, moves.Add);

            // Upward direction: only d5 (row 4) should be reachable; d6 (row 5) is friendly
            bool movesToD6 = moves.Exists(m => m.To == new TileData(3, 5));
            bool movesToD5 = moves.Exists(m => m.To == new TileData(3, 4));

            Assert.IsTrue(movesToD5);
            Assert.IsFalse(movesToD6);
        }

        [Test]
        public void Rook_CanCaptureOpponent()
        {
            // Rook at d4, dark piece at d7
            FenParser parser = new FenParser();
            GameStateData state = parser.Parse("8/3r4/8/8/3R4/8/8/8 w - - 0 1");
            BoardState board = new BoardState(state.boardData);

            RookMoveStrategy strategy = new RookMoveStrategy();
            List<Move> moves = new List<Move>();

            TileData start = new TileData(3, 3); // d4

            strategy.TryGetMoves(board, TeamColor.Light, start, moves.Add);

            bool captureMove = moves.Exists(m => m.To == new TileData(3, 6) && m.CapturedPiece != null);
            Assert.IsTrue(captureMove);
        }

        [Test]
        public void Rook_NoMoves_SurroundedByFriendly()
        {
            // Rook completely surrounded by friendly pieces on all 4 sides
            FenParser parser = new FenParser();
            GameStateData state = parser.Parse("8/8/8/3R4/2RRR3/3R4/8/8 w - - 0 1");
            BoardState board = new BoardState(state.boardData);

            RookMoveStrategy strategy = new RookMoveStrategy();
            List<Move> moves = new List<Move>();

            TileData start = new TileData(3, 3); // centre rook at d4

            bool result = strategy.TryGetMoves(board, TeamColor.Light, start, moves.Add);

            Assert.IsFalse(result);
            Assert.AreEqual(0, moves.Count);
        }

        // BISHOP
        [Test]
        public void Bishop_OpenBoard_HasMovesOnAllDiagonals()
        {
            // Bishop at d4 (3,3) on an otherwise empty board
            FenParser parser = new FenParser();
            GameStateData state = parser.Parse("8/8/8/8/3B4/8/8/8 w - - 0 1");
            BoardState board = new BoardState(state.boardData);

            BishopMoveStrategy strategy = new BishopMoveStrategy();
            List<Move> moves = new List<Move>();

            TileData start = new TileData(3, 3);

            bool result = strategy.TryGetMoves(board, TeamColor.Light, start, moves.Add);

            // UpRight: e5,f6,g7,h8 (4); UpLeft: c5,b6,a7 (3); DownRight: e3,f2,g1 (3); DownLeft: c3,b2,a1 (3) = 13
            Assert.IsTrue(result);
            Assert.AreEqual(13, moves.Count);
        }

        [Test]
        public void Bishop_CanCaptureOpponent_OnDiagonal()
        {
            // Bishop at d4, dark piece at f6
            FenParser parser = new FenParser();
            GameStateData state = parser.Parse("8/8/5b2/8/3B4/8/8/8 w - - 0 1");
            BoardState board = new BoardState(state.boardData);

            BishopMoveStrategy strategy = new BishopMoveStrategy();
            List<Move> moves = new List<Move>();

            TileData start = new TileData(3, 3); // d4

            strategy.TryGetMoves(board, TeamColor.Light, start, moves.Add);

            bool captureMove = moves.Exists(m => m.To == new TileData(5, 5) && m.CapturedPiece != null);
            Assert.IsTrue(captureMove);
        }

        [Test]
        public void Bishop_BlockedByFriendly_StopsBeforeFriendly()
        {
            // Bishop at d4, friendly piece at f6 — bishop reaches e5 only
            FenParser parser = new FenParser();
            GameStateData state = parser.Parse("8/8/5B2/8/3B4/8/8/8 w - - 0 1");
            BoardState board = new BoardState(state.boardData);

            BishopMoveStrategy strategy = new BishopMoveStrategy();
            List<Move> moves = new List<Move>();

            TileData start = new TileData(3, 3); // d4

            strategy.TryGetMoves(board, TeamColor.Light, start, moves.Add);

            bool movesToE5 = moves.Exists(m => m.To == new TileData(4, 4));
            bool movesToF6 = moves.Exists(m => m.To == new TileData(5, 5));

            Assert.IsTrue(movesToE5);
            Assert.IsFalse(movesToF6);
        }

        [Test]
        public void Bishop_NoMoves_SurroundedByFriendly()
        {
            // Bishop at d4, friendly pieces on all 4 diagonal neighbours
            FenParser parser = new FenParser();
            GameStateData state = parser.Parse("8/8/8/2B1B3/3B4/2B1B3/8/8 w - - 0 1");
            BoardState board = new BoardState(state.boardData);

            BishopMoveStrategy strategy = new BishopMoveStrategy();
            List<Move> moves = new List<Move>();

            TileData start = new TileData(3, 3); // centre bishop

            bool result = strategy.TryGetMoves(board, TeamColor.Light, start, moves.Add);

            Assert.IsFalse(result);
            Assert.AreEqual(0, moves.Count);
        }

        // KNIGHT
        [Test]
        public void Knight_OpenBoard_HasEightMoves()
        {
            // Knight at d4 (3,3) — all 8 L-shaped targets are on the board
            FenParser parser = new FenParser();
            GameStateData state = parser.Parse("8/8/8/8/3N4/8/8/8 w - - 0 1");
            BoardState board = new BoardState(state.boardData);

            KnightMoveStrategy strategy = new KnightMoveStrategy();
            List<Move> moves = new List<Move>();

            TileData start = new TileData(3, 3);

            bool result = strategy.TryGetMoves(board, TeamColor.Light, start, moves.Add);

            Assert.IsTrue(result);
            Assert.AreEqual(8, moves.Count);
        }

        [Test]
        public void Knight_CornerPosition_HasTwoMoves()
        {
            // Knight at a1 (0,0) — only 2 valid L-shaped squares exist
            FenParser parser = new FenParser();
            GameStateData state = parser.Parse("8/8/8/8/8/8/8/N7 w - - 0 1");
            BoardState board = new BoardState(state.boardData);

            KnightMoveStrategy strategy = new KnightMoveStrategy();
            List<Move> moves = new List<Move>();

            TileData start = new TileData(0, 0);

            bool result = strategy.TryGetMoves(board, TeamColor.Light, start, moves.Add);

            Assert.IsTrue(result);
            Assert.AreEqual(2, moves.Count);
        }

        [Test]
        public void Knight_CanCaptureOpponent()
        {
            // Knight at d4, dark piece at e6 (one of the 8 targets)
            FenParser parser = new FenParser();
            GameStateData state = parser.Parse("8/8/4n3/8/3N4/8/8/8 w - - 0 1");
            BoardState board = new BoardState(state.boardData);

            KnightMoveStrategy strategy = new KnightMoveStrategy();
            List<Move> moves = new List<Move>();

            TileData start = new TileData(3, 3); // d4

            strategy.TryGetMoves(board, TeamColor.Light, start, moves.Add);

            bool captureMove = moves.Exists(m => m.To == new TileData(4, 5) && m.CapturedPiece != null);
            Assert.IsTrue(captureMove);
        }

        [Test]
        public void Knight_FriendlyPieceOnTarget_Skipped()
        {
            // Friendly piece at e6 — knight should not move there
            FenParser parser = new FenParser();
            GameStateData state = parser.Parse("8/8/4N3/8/3N4/8/8/8 w - - 0 1");
            BoardState board = new BoardState(state.boardData);

            KnightMoveStrategy strategy = new KnightMoveStrategy();
            List<Move> moves = new List<Move>();

            TileData start = new TileData(3, 3); // d4

            strategy.TryGetMoves(board, TeamColor.Light, start, moves.Add);

            bool movesToE6 = moves.Exists(m => m.To == new TileData(4, 5));
            Assert.IsFalse(movesToE6);
        }

        [Test]
        public void Knight_JumpsOverPieces()
        {
            // Fill all squares adjacent to d4 — knight should still reach its L-shaped targets
            FenParser parser = new FenParser();
            GameStateData state = parser.Parse("8/8/8/2PPP3/2PNP3/2PPP3/8/8 w - - 0 1");
            BoardState board = new BoardState(state.boardData);

            KnightMoveStrategy strategy = new KnightMoveStrategy();
            List<Move> moves = new List<Move>();

            TileData start = new TileData(3, 3); // d4

            bool result = strategy.TryGetMoves(board, TeamColor.Light, start, moves.Add);

            // Targets not blocked by adjacent pieces (knight jumps): b3,b5,c2,c6,e2,e6,f3,f5 minus any out-of-board
            Assert.IsTrue(result);
        }

        // KING
        [Test]
        public void King_OpenBoard_HasEightMoves()
        {
            // King at d4 (3,3) — all 8 surrounding squares are on the board and empty
            FenParser parser = new FenParser();
            GameStateData state = parser.Parse("8/8/8/8/3K4/8/8/8 w - - 0 1");
            BoardState board = new BoardState(state.boardData);

            KingMoveStrategy strategy = new KingMoveStrategy();
            List<Move> moves = new List<Move>();

            TileData start = new TileData(3, 3);

            bool result = strategy.TryGetMoves(board, TeamColor.Light, start, moves.Add);

            Assert.IsTrue(result);
            Assert.AreEqual(8, moves.Count);
        }

        [Test]
        public void King_CornerPosition_HasThreeMoves()
        {
            // King at a1 (0,0) — only 3 reachable squares
            FenParser parser = new FenParser();
            GameStateData state = parser.Parse("8/8/8/8/8/8/8/K7 w - - 0 1");
            BoardState board = new BoardState(state.boardData);

            KingMoveStrategy strategy = new KingMoveStrategy();
            List<Move> moves = new List<Move>();

            TileData start = new TileData(0, 0);

            bool result = strategy.TryGetMoves(board, TeamColor.Light, start, moves.Add);

            Assert.IsTrue(result);
            Assert.AreEqual(3, moves.Count);
        }

        [Test]
        public void King_CanCaptureOpponent()
        {
            // King at d4, dark piece at d5
            FenParser parser = new FenParser();
            GameStateData state = parser.Parse("8/8/8/3k4/3K4/8/8/8 w - - 0 1");
            BoardState board = new BoardState(state.boardData);

            KingMoveStrategy strategy = new KingMoveStrategy();
            List<Move> moves = new List<Move>();

            TileData start = new TileData(3, 3); // d4

            strategy.TryGetMoves(board, TeamColor.Light, start, moves.Add);

            bool captureMove = moves.Exists(m => m.To == new TileData(3, 4) && m.CapturedPiece != null);
            Assert.IsTrue(captureMove);
        }

        [Test]
        public void King_FriendlyPieces_BlocksMoves()
        {
            // King surrounded by friendly pieces — no valid moves
            FenParser parser = new FenParser();
            GameStateData state = parser.Parse("8/8/8/2PPP3/2PKP3/2PPP3/8/8 w - - 0 1");
            BoardState board = new BoardState(state.boardData);

            KingMoveStrategy strategy = new KingMoveStrategy();
            List<Move> moves = new List<Move>();

            TileData start = new TileData(3, 3); // d4

            bool result = strategy.TryGetMoves(board, TeamColor.Light, start, moves.Add);

            Assert.IsFalse(result);
            Assert.AreEqual(0, moves.Count);
        }

        // QUEEN
        [Test]
        public void Queen_OpenBoard_CombinesRookAndBishopMoves()
        {
            // Queen at d4 (3,3) on an otherwise empty board — 7+7+13 = 27 moves
            FenParser parser = new FenParser();
            GameStateData state = parser.Parse("8/8/8/8/3Q4/8/8/8 w - - 0 1");
            BoardState board = new BoardState(state.boardData);

            QueenMoveStrategy strategy = new QueenMoveStrategy();
            List<Move> moves = new List<Move>();

            TileData start = new TileData(3, 3);

            bool result = strategy.TryGetMoves(board, TeamColor.Light, start, moves.Add);

            Assert.IsTrue(result);
            Assert.AreEqual(27, moves.Count);
        }

        [Test]
        public void Queen_CanCaptureOpponent_Diagonal()
        {
            // Queen at d4, dark piece at f6 (diagonal)
            FenParser parser = new FenParser();
            GameStateData state = parser.Parse("8/8/5q2/8/3Q4/8/8/8 w - - 0 1");
            BoardState board = new BoardState(state.boardData);

            QueenMoveStrategy strategy = new QueenMoveStrategy();
            List<Move> moves = new List<Move>();

            TileData start = new TileData(3, 3);

            strategy.TryGetMoves(board, TeamColor.Light, start, moves.Add);

            bool captureMove = moves.Exists(m => m.To == new TileData(5, 5) && m.CapturedPiece != null);
            Assert.IsTrue(captureMove);
        }

        [Test]
        public void Queen_CanCaptureOpponent_Straight()
        {
            // Queen at d4, dark piece at d7
            FenParser parser = new FenParser();
            GameStateData state = parser.Parse("8/3q4/8/8/3Q4/8/8/8 w - - 0 1");
            BoardState board = new BoardState(state.boardData);

            QueenMoveStrategy strategy = new QueenMoveStrategy();
            List<Move> moves = new List<Move>();

            TileData start = new TileData(3, 3);

            strategy.TryGetMoves(board, TeamColor.Light, start, moves.Add);

            bool captureMove = moves.Exists(m => m.To == new TileData(3, 6) && m.CapturedPiece != null);
            Assert.IsTrue(captureMove);
        }

        [Test]
        public void Queen_NoMoves_SurroundedByFriendly()
        {
            // Queen surrounded on all 8 squares by friendly pieces
            FenParser parser = new FenParser();
            GameStateData state = parser.Parse("8/8/8/2PPP3/2PQP3/2PPP3/8/8 w - - 0 1");
            BoardState board = new BoardState(state.boardData);

            QueenMoveStrategy strategy = new QueenMoveStrategy();
            List<Move> moves = new List<Move>();

            TileData start = new TileData(3, 3);

            bool result = strategy.TryGetMoves(board, TeamColor.Light, start, moves.Add);

            Assert.IsFalse(result);
            Assert.AreEqual(0, moves.Count);
        }
    }
}