using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

// Interactive "play along" tutorial.
// - Starts when the FIRST patient arrives in Day 1 (first time only).
// - Each step points an arrow at a button; the player must click it.
// - Info steps show a NEXT button instead.
// - SKIP ends it at any time. The patient's timer is paused during it.
public class TutorialManager : MonoBehaviour
{
    // Same key your GameManager already uses.
    public const string SavedKey = "TutorialCompleted";

    public enum ArrowPoints { Down, Up, Left, Right }   // direction the arrow POINTS
    public enum BoxPosition { Top, Center, Bottom }

    [System.Serializable]
    public class TutorialStep
    {
        [TextArea(2, 4)]
        public string text;

        [Tooltip("The player must click this button to continue. Leave EMPTY for an info step (shows NEXT).")]
        public Button targetButton;

        [Tooltip("Only used when Target Button is empty: the arrow points at this.")]
        public RectTransform pointAt;

        [Tooltip("Which way the arrow points. Down = arrow sits ABOVE the target pointing down.")]
        public ArrowPoints arrowPoints = ArrowPoints.Down;

        public BoxPosition boxPosition = BoxPosition.Top;

        [Tooltip("Info steps only: let the player click anything (for example to explore the book) until they press NEXT.")]
        public bool allowAnyClick;
    }

    // =========================================================
    // REFERENCES
    // =========================================================

    [Header("References")]
    public Day1Manager day1Manager;
    public GameObject tutorialOverlay;
    public TutorialClickBlocker clickBlocker;
    public RectTransform box;
    public TextMeshProUGUI tutorialText;

    [Tooltip("Optional. Shows e.g. 3 / 21")]
    public TextMeshProUGUI stepCounterText;

    public Button nextButton;

    [Tooltip("Optional. The text inside the NEXT button.")]
    public TextMeshProUGUI nextButtonLabel;

    public Button skipButton;
    public RectTransform arrow;

    [Header("Arrow")]
    public float arrowGap = 10f;
    public float bobAmount = 20f;
    public float bobSpeed = 6f;

    [Header("Testing")]
    [Tooltip("Turn ON to see the tutorial every time. Turn OFF for the real game.")]
    public bool alwaysShowTutorial = false;

    // =========================================================
    // STEPS (the arrow/target buttons are assigned in the Inspector)
    // =========================================================

    [Header("Steps")]
    public TutorialStep[] steps = DefaultSteps();

    static TutorialStep S(string text, ArrowPoints arrow, BoxPosition pos, bool anyClick = false)
    {
        TutorialStep s = new TutorialStep();
        s.text = text;
        s.arrowPoints = arrow;
        s.boxPosition = pos;
        s.allowAnyClick = anyClick;
        return s;
    }

    static TutorialStep[] DefaultSteps()
    {
        return new TutorialStep[]
        {
            // 0
            S("Welcome to Medisina, Doctor! I'll guide you through your first patient. You can SKIP anytime.",
              ArrowPoints.Down, BoxPosition.Center),
            // 1
            S("Click the PATIENT FILE to read the patient's information.",
              ArrowPoints.Down, BoxPosition.Top),
            // 2
            S("Read the file and compare it with how the patient looks. Press NEXT when you're done.",
              ArrowPoints.Down, BoxPosition.Top),
            // 3
            S("This patient looks legitimate. Click ACCEPT.",
              ArrowPoints.Down, BoxPosition.Top),
            // 4
            S("Now open the MEDICINE BOOK to find out the patient's illness.",
              ArrowPoints.Left, BoxPosition.Top),
            // 5
            S("Turn the pages to the SYMPTOMS page and tap the symptoms the patient has. The book highlights the possible illness. Press NEXT when done.",
              ArrowPoints.Down, BoxPosition.Bottom, true),
            // 6
            S("Check the COMPOUNDS and MEDICINES pages. Paracetamol is made by mixing p-Aminophenol (dish) and Acetic Anhydride (flask). Press NEXT.",
              ArrowPoints.Down, BoxPosition.Bottom, true),
            // 7
            S("Close the book.",
              ArrowPoints.Down, BoxPosition.Top),
            // 8
            S("Go to the MIXING AREA.",
              ArrowPoints.Down, BoxPosition.Top),
            // 9
            S("Click the DISH to put it on your table.",
              ArrowPoints.Down, BoxPosition.Top),
            // 10
            S("Pick up p-AMINOPHENOL from the cabinet.",
              ArrowPoints.Left, BoxPosition.Top),
            // 11
            S("Click the dish to put the p-Aminophenol in it.",
              ArrowPoints.Down, BoxPosition.Top),
            // 12
            S("Now click the FLASK.",
              ArrowPoints.Down, BoxPosition.Top),
            // 13
            S("Pick up ACETIC ANHYDRIDE from the cabinet.",
              ArrowPoints.Left, BoxPosition.Top),
            // 14
            S("Click the flask to pour it in.",
              ArrowPoints.Down, BoxPosition.Top),
            // 15
            S("Click MIX!",
              ArrowPoints.Down, BoxPosition.Top),
            // 16
            S("Pick up the mixed medicine.",
              ArrowPoints.Down, BoxPosition.Top),
            // 17
            S("Made a mistake? Pick up a compound and click the TRASH CAN to throw it away. But it costs the patient's time! Press NEXT.",
              ArrowPoints.Down, BoxPosition.Top),
            // 18
            S("Go back to the patient.",
              ArrowPoints.Down, BoxPosition.Top),
            // 19
            S("Give the medicine to the patient.",
              ArrowPoints.Down, BoxPosition.Top),
            // 20
            S("Well done, Doctor! You're on your own now. Good luck!",
              ArrowPoints.Down, BoxPosition.Center),
        };
    }

    // =========================================================
    // RUNTIME
    // =========================================================

    private bool armed = false;
    private bool isRunning = false;
    private int stepIndex = 0;

    private RectTransform arrowTarget;
    private ArrowPoints arrowDir;

    private readonly List<Button> hookedButtons = new List<Button>();
    private readonly List<UnityAction> hookedActions = new List<UnityAction>();
    private readonly Vector3[] corners = new Vector3[4];

    public bool IsArmed { get { return armed; } }

    void Start()
    {
        if (day1Manager == null)
            day1Manager = FindFirstObjectByType<Day1Manager>();

        if (clickBlocker == null && tutorialOverlay != null)
            clickBlocker = tutorialOverlay.GetComponentInChildren<TutorialClickBlocker>(true);

        if (nextButton != null)
            nextButton.onClick.AddListener(NextStep);

        if (skipButton != null)
            skipButton.onClick.AddListener(SkipTutorial);

        if (tutorialOverlay != null)
            tutorialOverlay.SetActive(false);
    }

    // True if the player already finished or skipped the tutorial before.
    public bool IsCompleted()
    {
        if (alwaysShowTutorial)
            return false;

        return PlayerPrefs.GetInt(SavedKey, 0) == 1;
    }

    // Called by MainMenuManager.PlayGame(). The tutorial then
    // begins by itself when the first patient arrives.
    public void Arm()
    {
        armed = true;
    }

    // Called by Day1Manager when a patient has arrived.
    public void OnPatientArrived()
    {
        if (!armed || isRunning)
            return;

        armed = false;

        StartTutorial();
    }

    // Kept so your GameManager still compiles.
    public void StartTutorial()
    {
        if (steps == null || steps.Length == 0)
            return;

        isRunning = true;
        stepIndex = 0;

        if (day1Manager != null)
            day1Manager.timerPaused = true;

        HookTargetButtons();

        if (tutorialOverlay != null)
            tutorialOverlay.SetActive(true);

        ShowStep();
    }

    // =========================================================
    // STEPS
    // =========================================================

    void HookTargetButtons()
    {
        UnhookTargetButtons();

        for (int i = 0; i < steps.Length; i++)
        {
            Button b = steps[i].targetButton;

            if (b == null)
                continue;

            int index = i;
            UnityAction action = () => OnTargetClicked(index);

            b.onClick.AddListener(action);

            hookedButtons.Add(b);
            hookedActions.Add(action);
        }
    }

    void UnhookTargetButtons()
    {
        for (int i = 0; i < hookedButtons.Count; i++)
        {
            if (hookedButtons[i] != null)
                hookedButtons[i].onClick.RemoveListener(hookedActions[i]);
        }

        hookedButtons.Clear();
        hookedActions.Clear();
    }

    void ShowStep()
    {
        TutorialStep step = steps[stepIndex];

        bool needsClick = step.targetButton != null;

        if (tutorialText != null)
            tutorialText.text = step.text;

        if (stepCounterText != null)
            stepCounterText.text = (stepIndex + 1) + " / " + steps.Length;

        // Next button only on info steps.
        if (nextButton != null)
            nextButton.gameObject.SetActive(!needsClick);

        if (nextButtonLabel != null)
        {
            nextButtonLabel.text =
                (stepIndex >= steps.Length - 1) ? "START" : "NEXT";
        }

        // Arrow target.
        arrowTarget = needsClick
            ? step.targetButton.GetComponent<RectTransform>()
            : step.pointAt;

        arrowDir = step.arrowPoints;

        // Click blocking.
        if (clickBlocker != null)
        {
            clickBlocker.allowedArea = needsClick ? arrowTarget : null;
            clickBlocker.allowAll = !needsClick && step.allowAnyClick;
        }

        PlaceBox(step.boxPosition);
    }

    void PlaceBox(BoxPosition position)
    {
        if (box == null)
            return;

        switch (position)
        {
            case BoxPosition.Top:
                box.anchorMin = box.anchorMax = new Vector2(0.5f, 1f);
                box.pivot = new Vector2(0.5f, 1f);
                box.anchoredPosition = new Vector2(0f, -40f);
                break;

            case BoxPosition.Bottom:
                box.anchorMin = box.anchorMax = new Vector2(0.5f, 0f);
                box.pivot = new Vector2(0.5f, 0f);
                box.anchoredPosition = new Vector2(0f, 40f);
                break;

            default:
                box.anchorMin = box.anchorMax = new Vector2(0.5f, 0.5f);
                box.pivot = new Vector2(0.5f, 0.5f);
                box.anchoredPosition = Vector2.zero;
                break;
        }
    }

    void OnTargetClicked(int index)
    {
        // Only the CURRENT step reacts to its button.
        if (!isRunning || index != stepIndex)
            return;

        Advance();
    }

    // Hooked to the NEXT button automatically in Start.
    public void NextStep()
    {
        if (!isRunning)
            return;

        // Click-steps can't be skipped with NEXT.
        if (steps[stepIndex].targetButton != null)
            return;

        Advance();
    }

    void Advance()
    {
        stepIndex++;

        if (stepIndex >= steps.Length)
        {
            FinishTutorial();
            return;
        }

        ShowStep();
    }

    // Hooked to the SKIP button automatically in Start.
    public void SkipTutorial()
    {
        if (!isRunning)
            return;

        FinishTutorial();
    }

    void FinishTutorial()
    {
        isRunning = false;

        UnhookTargetButtons();

        if (clickBlocker != null)
        {
            clickBlocker.allowedArea = null;
            clickBlocker.allowAll = true;
        }

        if (tutorialOverlay != null)
            tutorialOverlay.SetActive(false);

        if (day1Manager != null)
            day1Manager.timerPaused = false;

        PlayerPrefs.SetInt(SavedKey, 1);
        PlayerPrefs.Save();
    }

    // =========================================================
    // ARROW (follows the target every frame, even if it sways)
    // =========================================================

    void LateUpdate()
    {
        if (!isRunning || arrow == null)
            return;

        bool visible =
            arrowTarget != null &&
            arrowTarget.gameObject.activeInHierarchy;

        if (arrow.gameObject.activeSelf != visible)
            arrow.gameObject.SetActive(visible);

        if (!visible)
            return;

        arrowTarget.GetWorldCorners(corners);
        // corners: 0 = bottom-left, 1 = top-left, 2 = top-right, 3 = bottom-right

        Vector3 center = (corners[0] + corners[2]) * 0.5f;

        float scale = arrow.lossyScale.x;
        float half = arrow.rect.height * 0.5f * scale;
        float gap = arrowGap * scale;
        float bob = (Mathf.Sin(Time.unscaledTime * bobSpeed) * 0.5f + 0.5f) * bobAmount * scale;

        Vector3 pos = center;
        float rotZ = 0f;

        switch (arrowDir)
        {
            case ArrowPoints.Down:   // sits above the target, points down
                pos.y = corners[1].y + half + gap + bob;
                rotZ = 0f;
                break;

            case ArrowPoints.Up:     // sits below the target, points up
                pos.y = corners[0].y - half - gap - bob;
                rotZ = 180f;
                break;

            case ArrowPoints.Right:  // sits left of the target, points right
                pos.x = corners[0].x - half - gap - bob;
                rotZ = 90f;
                break;

            case ArrowPoints.Left:   // sits right of the target, points left
                pos.x = corners[2].x + half + gap + bob;
                rotZ = -90f;
                break;
        }

        arrow.position = pos;
        arrow.rotation = Quaternion.Euler(0f, 0f, rotZ);
    }

    // Right-click this component's header in the Inspector to use it.
    [ContextMenu("Reset Tutorial Saved Flag")]
    void ResetSavedFlag()
    {
        PlayerPrefs.DeleteKey(SavedKey);
        PlayerPrefs.Save();
        Debug.Log("Tutorial flag cleared. The tutorial will show on the next PLAY.");
    }
}