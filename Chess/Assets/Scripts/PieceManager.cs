using UnityEngine;

public class PieceManager : MonoBehaviour
{
    private void Awake()
    {
        ServiceLocator.Register<IMoveStrategyFactory>(new MoveStrategyFactory());
    }
}