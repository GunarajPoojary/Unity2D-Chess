using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public static class RaycastTargetToggle
{
    [MenuItem("GameObject/UI/Custom/Disable Raycast Targets", false, 0)]
    private static void DisableRaycastTargets()
    {
        SetRaycastTargetsOnSelection(false);
    }

    [MenuItem("GameObject/UI/Custom/Disable Raycast Targets", true)]
    private static bool ValidateDisable() => Selection.gameObjects.Length > 0;

    [MenuItem("GameObject/UI/Custom/Enable Raycast Targets", false, 1)]
    private static void EnableRaycastTargets()
    {
        SetRaycastTargetsOnSelection(true);
    }

    [MenuItem("GameObject/UI/Custom/Enable Raycast Targets", true)]
    private static bool ValidateEnable() => Selection.gameObjects.Length > 0;

    private static void SetRaycastTargetsOnSelection(bool state)
    {
        foreach (GameObject root in Selection.gameObjects)
        {
            Graphic[] graphics = root.GetComponentsInChildren<Graphic>(includeInactive: true);

            if (graphics.Length == 0) continue;

            Undo.RecordObjects(graphics, state ? "Enable Raycast Targets" : "Disable Raycast Targets");

            foreach (Graphic g in graphics)
            {
                g.raycastTarget = state;
                EditorUtility.SetDirty(g);
            }

            Debug.Log($"[RaycastTargetToggle] {(state ? "Enabled" : "Disabled")} raycastTarget on " +
                      $"{graphics.Length} Graphic(s) under '{root.name}'.");
        }
    }
}