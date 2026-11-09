using System.Collections;
using System.Globalization;
using UnityEngine;
using TMPro;

// Put this on a TextMeshPro text object placed next to the timer.
// Day1Manager calls Show(seconds) whenever time is deducted.
[RequireComponent(typeof(CanvasGroup))]
public class TimePenaltyPopup : MonoBehaviour
{
    [Header("Animation")]
    [Tooltip("How far the text floats upward (in UI units).")]
    public float floatDistance = 40f;

    [Tooltip("Total time the popup is on screen.")]
    public float duration = 1f;

    [Tooltip("Fraction of the duration the text stays fully visible before fading (0 to 1).")]
    [Range(0f, 0.9f)]
    public float holdFraction = 0.4f;

    private TextMeshProUGUI label;
    private CanvasGroup group;
    private RectTransform rect;
    private Vector2 basePosition;
    private Coroutine running;

    void Awake()
    {
        label = GetComponent<TextMeshProUGUI>();
        group = GetComponent<CanvasGroup>();
        rect = GetComponent<RectTransform>();

        basePosition = rect.anchoredPosition;

        // Never block clicks.
        group.alpha = 0f;
        group.blocksRaycasts = false;
        group.interactable = false;

        if (label != null)
            label.raycastTarget = false;
    }

    public void Show(float seconds)
    {
        if (label == null)
            return;

        label.text = "-" + seconds.ToString("0.##", CultureInfo.InvariantCulture);

        // Restart cleanly if a previous popup is still playing.
        if (running != null)
            StopCoroutine(running);

        running = StartCoroutine(Animate());
    }

    IEnumerator Animate()
    {
        float t = 0f;

        rect.anchoredPosition = basePosition;
        group.alpha = 1f;

        while (t < duration)
        {
            t += Time.deltaTime;

            float p = Mathf.Clamp01(t / duration);

            rect.anchoredPosition = basePosition + Vector2.up * (floatDistance * p);

            float fade = Mathf.Clamp01((p - holdFraction) / (1f - holdFraction));
            group.alpha = 1f - fade;

            yield return null;
        }

        group.alpha = 0f;
        rect.anchoredPosition = basePosition;
        running = null;
    }
}