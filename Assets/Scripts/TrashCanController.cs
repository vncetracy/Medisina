using UnityEngine;

// Put this on the Trash Can object (or any object) and hook the
// Trash Can's Button OnClick to TrashCanController.DiscardHeldItem().
public class TrashCanController : MonoBehaviour
{
    [Header("References")]
    public MixingManager mixingManager;
    public Day1Manager day1Manager;

    [Header("Rules")]
    [Tooltip("Seconds removed from the patient's timer after each successful discard.")]
    public float timePenaltySeconds = 5f;

    [Tooltip("Also allow throwing away the mixed result (including Wrong Mixture) once it has been picked up.")]
    public bool allowDiscardResultMedicine = true;

    [Header("Feedback (optional - leave blank to skip)")]
    [Tooltip("A soundName from the AudioManager's Sounds list.")]
    public string discardSoundName = "";
    public string invalidSoundName = "";

    public Animator trashAnimator;
    public string discardTrigger = "";
    public string invalidTrigger = "";

    void Start()
    {
        if (mixingManager == null)
            mixingManager = FindFirstObjectByType<MixingManager>();

        if (day1Manager == null)
            day1Manager = FindFirstObjectByType<Day1Manager>();

        if (mixingManager == null)
            Debug.LogWarning("TrashCanController: MixingManager is not assigned.");

        if (day1Manager == null)
            Debug.LogWarning("TrashCanController: Day1Manager is not assigned.");
    }

    // Hook this to the Trash Can's Button OnClick.
    public void DiscardHeldItem()
    {
        // Settings panel open -> game is paused, ignore.
        if (Time.timeScale == 0f)
            return;

        if (mixingManager == null)
            return;

        bool discarded = false;

        if (mixingManager.IsHoldingCompound())
        {
            discarded = mixingManager.DiscardHeldCompound();
        }
        else if (allowDiscardResultMedicine &&
                 mixingManager.IsHoldingMedicine())
        {
            discarded = mixingManager.DiscardHeldMedicine();
        }

        if (!discarded)
        {
            Debug.Log("Trash Can: nothing to discard.");
            PlayFeedback(invalidSoundName, invalidTrigger);
            return;
        }

        PlayFeedback(discardSoundName, discardTrigger);

        // Exactly one deduction per successful discard.
        if (day1Manager != null)
            day1Manager.PenalizeTime(timePenaltySeconds);
    }

    void PlayFeedback(string soundName, string trigger)
    {
        if (!string.IsNullOrEmpty(soundName) &&
            AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySound(soundName);
        }

        if (trashAnimator != null &&
            !string.IsNullOrEmpty(trigger))
        {
            trashAnimator.SetTrigger(trigger);
        }
    }
}