using System;

public class Pawn : ChessPiece
{
    private bool _hasMoved = false;
    private bool _isUpward = false;

    private IMoveStrategy _doubleStep;
    private IMoveStrategy _singleStep;
    private IMoveStrategy _capture;

    private void Awake()
    {
        _doubleStep = new PawnDoubleStepMove(_isUpward);
        _singleStep = new PawnSingleStepMove(_isUpward);
        _capture = new PawnCaptureMove(_isUpward);
    }

    public override void CalculatePossibleMoves(IBoardService board, TileData currentTile, Action<Move> onPossibleMoveFound)
    {
        if (!_hasMoved)
        {
            _doubleStep.TryGetMoves(board, pieceData.Color, currentTile, onPossibleMoveFound);
        }

        _singleStep.TryGetMoves(board, pieceData.Color, currentTile, onPossibleMoveFound);

        _capture.TryGetMoves(board, pieceData.Color, currentTile, onPossibleMoveFound);
    }

    public void MarkAsMoved()
    {
        _hasMoved = true;
    }
}