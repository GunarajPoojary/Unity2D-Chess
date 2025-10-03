using System;
using UnityEngine;

public static class GameEvents
{
    public static event Action<Vector2Int> OnUnHighlightEventRaised = delegate { };
    public static event Action<Vector2Int, HighlightType> OnHighlightEventRaised = delegate { };

    public static void RaiseUnHighlightEvent(Vector2Int position) => OnUnHighlightEventRaised?.Invoke(position);
    public static void RaiseHighlightEvent(Vector2Int position, HighlightType type) => OnHighlightEventRaised?.Invoke(position, type);
}