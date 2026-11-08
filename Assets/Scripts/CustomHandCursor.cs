
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CustomHandCursor : MonoBehaviour
{
    [Header("Cursor Sprites")]
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite clickSprite;
    [SerializeField] private Sprite holdSprite;
    [SerializeField] private Sprite hoverSprite;

    [Header("Cursor Settings")]
    [SerializeField] private Vector2 cursorOffset = Vector2.zero;
    [SerializeField] private float clickDuration = 0.08f;
    [SerializeField] private float holdDelay = 0.08f;

    private Image cursorImage;
    private RectTransform cursorRect;

    private float clickTimer;
    private float holdTimer;

    private void Awake()
    {
        cursorImage = GetComponent<Image>();
        cursorRect = GetComponent<RectTransform>();
    }

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None;

        SetCursor(idleSprite);
    }

    private void Update()
    {
        // Follow the actual mouse position
        cursorRect.position = (Vector2)Input.mousePosition
                              + cursorOffset;

        bool mouseDown = Input.GetMouseButtonDown(0);
        bool mouseHeld = Input.GetMouseButton(0);

        // Initial click
        if (mouseDown)
        {
            clickTimer = clickDuration;
            holdTimer = 0f;
        }

        // Count how long the button is held
        if (mouseHeld)
        {
            holdTimer += Time.unscaledDeltaTime;
        }

        // Count down click animation
        if (clickTimer > 0f)
        {
            clickTimer -= Time.unscaledDeltaTime;
        }

        // Determine cursor state
        if (mouseHeld && holdTimer >= holdDelay)
        {
            SetCursor(holdSprite);
        }
        else if (clickTimer > 0f)
        {
            SetCursor(clickSprite);
        }
        else if (IsHoveringClickableUI())
        {
            SetCursor(hoverSprite);
        }
        else
        {
            SetCursor(idleSprite);
        }
    }

    private bool IsHoveringClickableUI()
    {
        if (EventSystem.current == null)
            return false;

        PointerEventData pointerData =
            new PointerEventData(EventSystem.current);

        pointerData.position = Input.mousePosition;

        var results =
            new System.Collections.Generic.List<RaycastResult>();

        EventSystem.current.RaycastAll(pointerData, results);

        foreach (RaycastResult result in results)
        {
            GameObject target = result.gameObject;

            // Check for standard Unity UI buttons,
            // sliders, toggles, and other Selectables.
            Selectable selectable =
                target.GetComponentInParent<Selectable>();

            if (selectable != null && selectable.IsInteractable())
                return true;

            // Also support custom click and drag handlers.
            if (ExecuteEvents.GetEventHandler<IPointerClickHandler>(
                target) != null)
                return true;

            if (ExecuteEvents.GetEventHandler<IDragHandler>(
                target) != null)
                return true;
        }

        return false;
    }

    private void SetCursor(Sprite newSprite)
    {
        if (newSprite != null && cursorImage.sprite != newSprite)
        {
            cursorImage.sprite = newSprite;
        }
    }

    private void OnDisable()
    {
        Cursor.visible = true;
    }
}
