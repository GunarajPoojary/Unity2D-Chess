using UnityEngine;

public interface IPlayerContext
{
    TeamColor Color { get; }
    BoardSide Side { get; }
    bool IsHuman { get; }
}

public class PlayerManager : MonoBehaviour, IPlayerContext
{
    public TeamColor PlayerColor { get; private set; }

    public TeamColor Color { get; private set; } = TeamColor.White;
    public BoardSide Side { get; private set; } = BoardSide.Down;
    public bool IsHuman { get; private set; } = true;

    private void Awake()
    {
        ServiceLocator.Register<IPlayerContext>(this);
    }
}