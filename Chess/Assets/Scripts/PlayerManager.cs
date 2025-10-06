using UnityEngine;

public interface IPlayerContext
{
    TeamColor Color { get; }
    MovementDirection MovementDirection { get; }
    bool IsHuman { get; }
}

public class PlayerManager : MonoBehaviour, IPlayerContext
{
    public TeamColor PlayerColor { get; private set; }

    public TeamColor Color { get; private set; } = TeamColor.White;
    public MovementDirection MovementDirection { get; private set; } = MovementDirection.Up;
    public bool IsHuman { get; private set; } = true;

    private void Awake()
    {
        ServiceLocator.Register<IPlayerContext>(this);
    }
}