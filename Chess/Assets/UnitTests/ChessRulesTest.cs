using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Chess
{
    public class MockBoardService : IBoardService
    {
        public HashSet<TileData> FriendlyTiles { get; } = new();
        public Dictionary<TileData, ChessPiece> EnemyTiles { get; } = new();

        public int Rows { get; set; } = 8;
        public int Columns { get; set; } = 8;

        public bool IsWithinBoard(TileData tile)
            => tile.rowIndex >= 0 && tile.rowIndex < Rows
            && tile.columnIndex >= 0 && tile.columnIndex < Columns;

        public bool IsTileEmptyAt(TileData tile)
            => !FriendlyTiles.Contains(tile) && !EnemyTiles.ContainsKey(tile);

        public bool TryGetPieceByColor(TeamColor color, TileData tile, out ChessPiece piece)
        {
            if (EnemyTiles.TryGetValue(tile, out piece))
                return true;

            piece = null;
            return false;
        }

        public bool TryGetPieceAt(TileData tile, out ChessPiece piece)
        {
            throw new NotImplementedException();
        }
    }

    public static class BoardFactory
    {
        public static MockBoardService Empty() => new MockBoardService();

        public static MockBoardService WithFriendlyRing(TileData center, IEnumerable<TileData> blocked)
        {
            MockBoardService board = new MockBoardService();
            foreach (TileData t in blocked)
                board.FriendlyTiles.Add(t);
            return board;
        }
    }

    [TestFixture]
    public class ChessRulesTest
    {
        [Test]
        public void Rook_OpenBoard_GeneratesMovesInAllFourDirections()
        {
            MockBoardService board = BoardFactory.Empty();
            RookMoveStrategy strategy = new RookMoveStrategy();
            TileData origin = new TileData(3, 3);

            List<Move> moves = CollectMoves(board, strategy, TeamColor.Light, origin);

            Assert.AreEqual(14, moves.Count, "Rook should have 14 moves from (3,3) on an empty board.");
        }

        [Test]
        public void Rook_BlockedByFriendly_StopsBeforeBlocker()
        {
            MockBoardService board = BoardFactory.Empty();
            TileData origin = new TileData(0, 0);
            TileData blocker = new TileData(2, 0);
            board.FriendlyTiles.Add(blocker);

            RookMoveStrategy strategy = new RookMoveStrategy();
            List<Move> moves = CollectMoves(board, strategy, TeamColor.Light, origin);

            Assert.IsFalse(moves.Exists(m => m.To.columnIndex >= 2 && m.To.rowIndex == 0),
                "Rook must not move to or past a friendly piece.");
            Assert.IsTrue(moves.Exists(m => m.To == new TileData(1, 0)),
                "Rook should be able to move to the tile immediately before the blocker.");
        }

        [Test]
        public void Rook_CanCaptureEnemy_ButNotMovePast()
        {
            MockBoardService board = BoardFactory.Empty();
            TileData origin = new TileData(0, 0);
            TileData enemy = new TileData(3, 0);
            board.EnemyTiles[enemy] = null;

            RookMoveStrategy strategy = new RookMoveStrategy();
            List<Move> moves = CollectMoves(board, strategy, TeamColor.Light, origin);

            Assert.IsTrue(moves.Exists(m => m.To == enemy && m.CapturedPiece != null),
                "Rook should include the enemy tile as a capture move.");
            Assert.IsFalse(moves.Exists(m => m.To.columnIndex > 3 && m.To.rowIndex == 0),
                "Rook must not move past a captured enemy piece.");
        }

        [Test]
        public void Rook_AtEdge_DoesNotLeaveBoard()
        {
            MockBoardService board = BoardFactory.Empty();
            RookMoveStrategy strategy = new RookMoveStrategy();
            TileData corner = new TileData(0, 0);

            List<Move> moves = CollectMoves(board, strategy, TeamColor.Light, corner);

            Assert.IsTrue(moves.TrueForAll(m => board.IsWithinBoard(m.To)),
                "All rook moves must remain within the board.");
        }

        [Test]
        public void Bishop_OpenBoard_GeneratesDiagonalMovesOnly()
        {
            MockBoardService board = BoardFactory.Empty();
            BishopMoveStrategy strategy = new BishopMoveStrategy();
            TileData origin = new TileData(3, 3);

            List<Move> moves = CollectMoves(board, strategy, TeamColor.Light, origin);

            foreach (Move move in moves)
            {
                int dCol = Math.Abs(move.To.columnIndex - origin.columnIndex);
                int dRow = Math.Abs(move.To.rowIndex - origin.rowIndex);
                Assert.AreEqual(dCol, dRow,
                    $"Bishop move to ({move.To.columnIndex},{move.To.rowIndex}) is not diagonal.");
            }
        }

        [Test]
        public void Bishop_OpenBoard_CorrectMoveCount()
        {
            MockBoardService board = BoardFactory.Empty();
            BishopMoveStrategy strategy = new BishopMoveStrategy();
            List<Move> moves = CollectMoves(board, strategy, TeamColor.Light, new TileData(3, 3));

            Assert.AreEqual(13, moves.Count, "Bishop should have 13 moves from (3,3) on an empty 8x8 board.");
        }

        [Test]
        public void Bishop_CanCaptureEnemy_OnDiagonal()
        {
            MockBoardService board = BoardFactory.Empty();
            TileData origin = new TileData(0, 0);
            TileData enemy = new TileData(3, 3);
            board.EnemyTiles[enemy] = null;

            BishopMoveStrategy strategy = new BishopMoveStrategy();
            List<Move> moves = CollectMoves(board, strategy, TeamColor.Light, origin);

            Assert.IsTrue(moves.Exists(m => m.To == enemy && m.CapturedPiece != null),
                "Bishop should be able to capture an enemy piece on the diagonal.");
        }

        [Test]
        public void Bishop_BlockedByFriendly_CannotPassThrough()
        {
            MockBoardService board = BoardFactory.Empty();
            TileData origin = new TileData(0, 0);
            board.FriendlyTiles.Add(new TileData(2, 2));

            BishopMoveStrategy strategy = new BishopMoveStrategy();
            List<Move> moves = CollectMoves(board, strategy, TeamColor.Light, origin);

            Assert.IsFalse(moves.Exists(m => m.To.columnIndex > 2 && m.To.rowIndex > 2),
                "Bishop must not move past a friendly piece on a diagonal.");
        }

        [Test]
        public void Queen_OpenBoard_CombinesRookAndBishopMoves()
        {
            MockBoardService board = BoardFactory.Empty();
            TileData origin = new TileData(3, 3);
            RookMoveStrategy rook = new RookMoveStrategy();
            BishopMoveStrategy bishop = new BishopMoveStrategy();
            QueenMoveStrategy queen = new QueenMoveStrategy();

            int rookCount = CollectMoves(board, rook, TeamColor.Light, origin).Count;
            int bishopCount = CollectMoves(board, bishop, TeamColor.Light, origin).Count;
            int queenCount = CollectMoves(board, queen, TeamColor.Light, origin).Count;

            Assert.AreEqual(rookCount + bishopCount, queenCount,
                "Queen move count must equal rook + bishop move counts from the same position.");
        }

        [Test]
        public void Queen_CanCaptureEnemyOnAnyLine()
        {
            MockBoardService board = BoardFactory.Empty();
            TileData origin = new TileData(3, 3);
            TileData enemyOrtho = new TileData(3, 6);
            TileData enemyDiag = new TileData(5, 5);

            board.EnemyTiles[enemyOrtho] = null;
            board.EnemyTiles[enemyDiag] = null;

            QueenMoveStrategy strategy = new QueenMoveStrategy();
            List<Move> moves = CollectMoves(board, strategy, TeamColor.Light, origin);

            Assert.IsTrue(moves.Exists(m => m.To == enemyOrtho && m.CapturedPiece != null));
            Assert.IsTrue(moves.Exists(m => m.To == enemyDiag && m.CapturedPiece != null));
        }

        [Test]
        public void Knight_OpenBoard_HasEightMoves()
        {
            MockBoardService board = BoardFactory.Empty();
            KnightMoveStrategy strategy = new KnightMoveStrategy();
            List<Move> moves = CollectMoves(board, strategy, TeamColor.Light, new TileData(3, 3));

            Assert.AreEqual(8, moves.Count, "Knight should have exactly 8 moves from an open centre position.");
        }

        [Test]
        public void Knight_AtCorner_HasOnlyTwoMoves()
        {
            MockBoardService board = BoardFactory.Empty();
            KnightMoveStrategy strategy = new KnightMoveStrategy();
            List<Move> moves = CollectMoves(board, strategy, TeamColor.Light, new TileData(0, 0));

            Assert.AreEqual(2, moves.Count, "Knight at corner (0,0) should have exactly 2 moves.");
        }

        [Test]
        public void Knight_JumpsOverPieces()
        {
            MockBoardService board = BoardFactory.Empty();
            TileData origin = new TileData(3, 3);

            board.FriendlyTiles.Add(new TileData(3, 4));
            board.FriendlyTiles.Add(new TileData(4, 3));
            board.FriendlyTiles.Add(new TileData(3, 2));
            board.FriendlyTiles.Add(new TileData(2, 3));
            board.FriendlyTiles.Add(new TileData(4, 4));
            board.FriendlyTiles.Add(new TileData(2, 4));
            board.FriendlyTiles.Add(new TileData(4, 2));
            board.FriendlyTiles.Add(new TileData(2, 2));

            KnightMoveStrategy strategy = new KnightMoveStrategy();
            List<Move> moves = CollectMoves(board, strategy, TeamColor.Light, origin);

            Assert.AreEqual(8, moves.Count, "Knight must jump over all adjacent friendly pieces and still reach 8 L-shaped targets.");
        }

        [Test]
        public void Knight_CanCaptureEnemy()
        {
            MockBoardService board = BoardFactory.Empty();
            TileData origin = new TileData(3, 3);
            TileData enemy = new TileData(4, 5);
            board.EnemyTiles[enemy] = null;

            KnightMoveStrategy strategy = new KnightMoveStrategy();
            List<Move> moves = CollectMoves(board, strategy, TeamColor.Light, origin);

            Assert.IsTrue(moves.Exists(m => m.To == enemy && m.CapturedPiece != null),
                "Knight should capture an enemy on a valid L-shaped target.");
        }

        [Test]
        public void Knight_DoesNotLandOnFriendly()
        {
            MockBoardService board = BoardFactory.Empty();
            TileData origin = new TileData(3, 3);
            TileData friendly = new TileData(4, 5);
            board.FriendlyTiles.Add(friendly);

            KnightMoveStrategy strategy = new KnightMoveStrategy();
            List<Move> moves = CollectMoves(board, strategy, TeamColor.Light, origin);

            Assert.IsFalse(moves.Exists(m => m.To == friendly),
                "Knight must not land on a friendly piece.");
        }

        [Test]
        public void King_OpenBoard_HasEightMoves()
        {
            MockBoardService board = BoardFactory.Empty();
            KingMoveStrategy strategy = new KingMoveStrategy();
            List<Move> moves = CollectMoves(board, strategy, TeamColor.Light, new TileData(3, 3));

            Assert.AreEqual(8, moves.Count, "King should have 8 moves from an open centre position.");
        }

        [Test]
        public void King_AtCorner_HasOnlyThreeMoves()
        {
            MockBoardService board = BoardFactory.Empty();
            KingMoveStrategy strategy = new KingMoveStrategy();
            List<Move> moves = CollectMoves(board, strategy, TeamColor.Light, new TileData(0, 0));

            Assert.AreEqual(3, moves.Count, "King at corner (0,0) should have exactly 3 moves.");
        }

        [Test]
        public void King_CannotLandOnFriendlyPiece()
        {
            MockBoardService board = BoardFactory.Empty();
            TileData origin = new TileData(3, 3);

            board.FriendlyTiles.Add(new TileData(3, 4));
            board.FriendlyTiles.Add(new TileData(4, 3));
            board.FriendlyTiles.Add(new TileData(4, 4));

            KingMoveStrategy strategy = new KingMoveStrategy();
            List<Move> moves = CollectMoves(board, strategy, TeamColor.Light, origin);

            Assert.AreEqual(5, moves.Count, "King should have 5 moves when 3 adjacent squares are friendly.");
            Assert.IsTrue(moves.TrueForAll(m => !board.FriendlyTiles.Contains(m.To)),
                "King must not move onto a friendly piece.");
        }

        [Test]
        public void King_CanCaptureAdjacentEnemy()
        {
            MockBoardService board = BoardFactory.Empty();
            TileData origin = new TileData(3, 3);
            TileData enemy = new TileData(3, 4);
            board.EnemyTiles[enemy] = null;

            KingMoveStrategy strategy = new KingMoveStrategy();
            List<Move> moves = CollectMoves(board, strategy, TeamColor.Light, origin);

            Assert.IsTrue(moves.Exists(m => m.To == enemy && m.CapturedPiece != null),
                "King should be able to capture an adjacent enemy piece.");
        }

        [Test]
        public void PawnSingleStep_EmptyAhead_GeneratesOneMove()
        {
            MockBoardService board = BoardFactory.Empty();
            PawnSingleStepMove strategy = new PawnSingleStepMove(isUpward: true);
            TileData origin = new TileData(3, 3);

            List<Move> moves = CollectMoves(board, strategy, TeamColor.Light, origin);

            Assert.AreEqual(1, moves.Count);
            Assert.AreEqual(new TileData(3, 4), moves[0].To,
                "Upward pawn should advance one row.");
        }

        [Test]
        public void PawnSingleStep_Downward_AdvancesNegativeRow()
        {
            MockBoardService board = BoardFactory.Empty();
            PawnSingleStepMove strategy = new PawnSingleStepMove(isUpward: false);
            TileData origin = new TileData(3, 4);

            List<Move> moves = CollectMoves(board, strategy, TeamColor.Light, origin);

            Assert.AreEqual(1, moves.Count);
            Assert.AreEqual(new TileData(3, 3), moves[0].To,
                "Downward pawn should advance one row in the negative direction.");
        }

        [Test]
        public void PawnSingleStep_Blocked_ReturnsNoMoves()
        {
            MockBoardService board = BoardFactory.Empty();
            TileData origin = new TileData(3, 3);
            board.FriendlyTiles.Add(new TileData(3, 4));

            PawnSingleStepMove strategy = new PawnSingleStepMove(isUpward: true);
            List<Move> moves = CollectMoves(board, strategy, TeamColor.Light, origin);

            Assert.AreEqual(0, moves.Count, "Pawn must not move forward into a blocked tile.");
        }

        [Test]
        public void PawnSingleStep_AtTopEdge_ReturnsNoMoves()
        {
            MockBoardService board = BoardFactory.Empty();
            PawnSingleStepMove strategy = new PawnSingleStepMove(isUpward: true);
            TileData origin = new TileData(3, 7);

            List<Move> moves = CollectMoves(board, strategy, TeamColor.Light, origin);

            Assert.AreEqual(0, moves.Count, "Pawn at the top edge must not move off the board.");
        }

        [Test]
        public void PawnDoubleStep_EmptyPath_GeneratesOneMove()
        {
            MockBoardService board = BoardFactory.Empty();
            PawnDoubleStepMove strategy = new PawnDoubleStepMove(isUpward: true);
            TileData origin = new TileData(3, 1);

            List<Move> moves = CollectMoves(board, strategy, TeamColor.Light, origin);

            Assert.AreEqual(1, moves.Count);
            Assert.AreEqual(new TileData(3, 3), moves[0].To,
                "Double-step pawn should land two rows ahead.");
        }

        [Test]
        public void PawnDoubleStep_IntermediateBlocked_ReturnsNoMoves()
        {
            MockBoardService board = BoardFactory.Empty();
            TileData origin = new TileData(3, 1);
            board.FriendlyTiles.Add(new TileData(3, 2));

            PawnDoubleStepMove strategy = new PawnDoubleStepMove(isUpward: true);
            List<Move> moves = CollectMoves(board, strategy, TeamColor.Light, origin);

            Assert.AreEqual(0, moves.Count, "Double step must be blocked when the intermediate tile is occupied.");
        }

        [Test]
        public void PawnDoubleStep_TargetBlocked_ReturnsNoMoves()
        {
            MockBoardService board = BoardFactory.Empty();
            TileData origin = new TileData(3, 1);
            board.FriendlyTiles.Add(new TileData(3, 3));

            PawnDoubleStepMove strategy = new PawnDoubleStepMove(isUpward: true);
            List<Move> moves = CollectMoves(board, strategy, TeamColor.Light, origin);

            Assert.AreEqual(0, moves.Count, "Double step must be blocked when the destination tile is occupied.");
        }

        [Test]
        public void PawnDoubleStep_NearEdge_ReturnsNoMoves()
        {
            MockBoardService board = BoardFactory.Empty();
            PawnDoubleStepMove strategy = new PawnDoubleStepMove(isUpward: true);
            TileData origin = new TileData(3, 7);

            List<Move> moves = CollectMoves(board, strategy, TeamColor.Light, origin);

            Assert.AreEqual(0, moves.Count, "Double step from the edge must not leave the board.");
        }

        [Test]
        public void PawnCapture_EnemyOnDiagonal_GeneratesCapture()
        {
            MockBoardService board = BoardFactory.Empty();
            TileData origin = new TileData(3, 3);
            TileData enemy = new TileData(4, 4);

            board.EnemyTiles[enemy] = null;

            PawnCaptureMove strategy = new PawnCaptureMove(isUpward: true);
            List<Move> moves = CollectMoves(board, strategy, TeamColor.Light, origin);

            Assert.IsTrue(moves.Exists(m => m.To == enemy && m.CapturedPiece != null),
                "Pawn should capture diagonally forward.");
        }

        [Test]
        public void PawnCapture_NoEnemyOnDiagonal_ReturnsNoMoves()
        {
            MockBoardService board = BoardFactory.Empty();
            PawnCaptureMove strategy = new PawnCaptureMove(isUpward: true);
            TileData origin = new TileData(3, 3);

            List<Move> moves = CollectMoves(board, strategy, TeamColor.Light, origin);

            Assert.AreEqual(0, moves.Count, "Pawn should not generate capture moves when no enemy is diagonally ahead.");
        }

        [Test]
        public void PawnCapture_CanCaptureInBothDiagonalDirections()
        {
            MockBoardService board = BoardFactory.Empty();
            TileData origin = new TileData(3, 3);
            TileData enemyLeft = new TileData(2, 4);
            TileData enemyRight = new TileData(4, 4);
            board.EnemyTiles[enemyLeft] = null;
            board.EnemyTiles[enemyRight] = null;

            PawnCaptureMove strategy = new PawnCaptureMove(isUpward: true);
            List<Move> moves = CollectMoves(board, strategy, TeamColor.Light, origin);

            Assert.AreEqual(2, moves.Count, "Upward pawn should capture on both left and right diagonals.");
        }

        [Test]
        public void PawnCapture_DownwardPawn_CapturesBelowDiagonals()
        {
            MockBoardService board = BoardFactory.Empty();
            TileData origin = new TileData(3, 4);
            TileData enemy = new TileData(4, 3);
            board.EnemyTiles[enemy] = null;

            PawnCaptureMove strategy = new PawnCaptureMove(isUpward: false);
            List<Move> moves = CollectMoves(board, strategy, TeamColor.Dark, origin);

            Assert.IsTrue(moves.Exists(m => m.To == enemy && m.CapturedPiece != null),
                "Downward pawn should capture diagonally backward (toward lower rows).");
        }

        [Test]
        public void TileData_DirectionConstants_AreCorrect()
        {
            Assert.AreEqual(new TileData(0, 1), TileData.Up, "Up should be row+1.");
            Assert.AreEqual(new TileData(0, -1), TileData.Down, "Down should be row-1.");
            Assert.AreEqual(new TileData(1, 0), TileData.Right, "Right should be col+1.");
            Assert.AreEqual(new TileData(-1, 0), TileData.Left, "Left should be col-1.");
            Assert.AreEqual(new TileData(1, 1), TileData.UpRight, "UpRight should be (col+1, row+1).");
            Assert.AreEqual(new TileData(-1, 1), TileData.UpLeft, "UpLeft should be (col-1, row+1).");
            Assert.AreEqual(new TileData(1, -1), TileData.DownRight, "DownRight should be (col+1, row-1).");
            Assert.AreEqual(new TileData(-1, -1), TileData.DownLeft, "DownLeft should be (col-1, row-1).");
        }

        [Test]
        public void TileData_Addition_IsCorrect()
        {
            TileData a = new TileData(2, 3);
            TileData b = new TileData(1, -1);
            TileData result = a + b;
            Assert.AreEqual(new TileData(3, 2), result);
        }

        [Test]
        public void TileData_Equality_WorksCorrectly()
        {
            Assert.AreEqual(new TileData(2, 3), new TileData(2, 3));
            Assert.AreNotEqual(new TileData(2, 3), new TileData(3, 2));
        }

        private static List<Move> CollectMoves(
            IBoardService board,
            IMoveStrategy strategy,
            TeamColor color,
            TileData origin)
        {
            List<Move> moves = new List<Move>();
            strategy.TryGetMoves(board, color, origin, m => moves.Add(m));
            return moves;
        }
    }
}