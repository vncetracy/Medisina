using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PanelNavigation : MonoBehaviour
{
    [Header("Gameplay")]
    public RectTransform gameplayContainer;

    public float screenWidth = 1920f;

    [Header("Panel Movement")]
    public float movementDuration = 0.15f;

    [Header("Fade Transition")]
    public Image fadePanel;

    // How fast the fade happens
    public float fadeDuration = 0.05f;

    // How long the screen stays completely black
    public float blackScreenDuration = 0.05f;

    private bool isTransitioning = false;

    void Start()
    {
        // Fade panel is disabled when the game starts
        fadePanel.gameObject.SetActive(false);
    }

    public void GoToDoctorDesk()
    {
        StartPanelTransition(new Vector2(0f, 0f));
    }

    public void GoToMedicineBook()
    {
        StartPanelTransition(new Vector2(screenWidth, 0f));
    }

    public void GoToMixingArea()
    {
        StartPanelTransition(new Vector2(-screenWidth, 0f));
    }

    void StartPanelTransition(Vector2 targetPosition)
    {
        // Prevent multiple transitions at the same time
        if (isTransitioning)
            return;

        // Don't transition if already at the target
        if (Vector2.Distance(
            gameplayContainer.anchoredPosition,
            targetPosition
        ) < 0.1f)
        {
            return;
        }

        StartCoroutine(FadeAndMove(targetPosition));
    }

    IEnumerator FadeAndMove(Vector2 targetPosition)
    {
        isTransitioning = true;

        // Enable fade panel
        fadePanel.gameObject.SetActive(true);

        // Start completely transparent
        Color color = fadePanel.color;
        color.a = 0f;
        fadePanel.color = color;

        // --------------------------------
        // FADE TO BLACK
        // --------------------------------

        yield return StartCoroutine(Fade(1f));

        // --------------------------------
        // START PANEL MOVEMENT
        // --------------------------------

        StartCoroutine(MoveToPosition(targetPosition));

        // --------------------------------
        // SHORT BLACK DELAY
        // --------------------------------

        yield return new WaitForSeconds(blackScreenDuration);

        // --------------------------------
        // FADE BACK IN
        // --------------------------------

        yield return StartCoroutine(Fade(0f));

        // Make sure movement is finished
        gameplayContainer.anchoredPosition = targetPosition;

        // Disable fade panel
        fadePanel.gameObject.SetActive(false);

        isTransitioning = false;
    }

    IEnumerator MoveToPosition(Vector2 targetPosition)
    {
        Vector2 startPosition = gameplayContainer.anchoredPosition;

        float elapsedTime = 0f;

        while (elapsedTime < movementDuration)
        {
            elapsedTime += Time.deltaTime;

            float progress = elapsedTime / movementDuration;

            // Smooth movement
            progress = Mathf.SmoothStep(0f, 1f, progress);

            gameplayContainer.anchoredPosition =
                Vector2.Lerp(
                    startPosition,
                    targetPosition,
                    progress
                );

            yield return null;
        }

        gameplayContainer.anchoredPosition = targetPosition;
    }

    IEnumerator Fade(float targetAlpha)
    {
        Color color = fadePanel.color;

        float startAlpha = color.a;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;

            float progress = elapsedTime / fadeDuration;

            color.a = Mathf.Lerp(
                startAlpha,
                targetAlpha,
                progress
            );

            fadePanel.color = color;

            yield return null;
        }

        color.a = targetAlpha;
        fadePanel.color = color;
    }
}