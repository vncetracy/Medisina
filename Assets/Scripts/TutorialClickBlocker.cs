using UnityEngine;

// Put this on a full-screen, transparent Image (Raycast Target ON).
// It blocks every click EXCEPT inside the highlighted target's area,
// so the player can only click what the tutorial points at.
//
// IMPORTANT: this must NOT be the parent of the tutorial box/buttons.
// Keep it as a sibling (see setup steps).
public class TutorialClickBlocker : MonoBehaviour, ICanvasRaycastFilter
{
    [HideInInspector] public RectTransform allowedArea;
    [HideInInspector] public bool allowAll;

    // Return true  = this blocker catches the click (blocked).
    // Return false = the click passes through to whatever is underneath.
    public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
    {
        if (allowAll)
            return false;

        if (allowedArea == null)
            return true;

        return !RectTransformUtility.RectangleContainsScreenPoint(
            allowedArea,
            screenPoint,
            eventCamera
        );
    }
}