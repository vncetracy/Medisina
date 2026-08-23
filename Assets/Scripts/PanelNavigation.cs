using UnityEngine;
using System.Collections;

public class PanelNavigation : MonoBehaviour
{
    public RectTransform gameplayContainer;

    public float screenWidth = 1920f;

    public float transitionSpeed = 5f;

    private Coroutine currentTransition;

    public void GoToDoctorDesk()
    {
        MoveToPosition(new Vector2(0f, 0f));
    }

    public void GoToMedicineBook()
    {
        MoveToPosition(new Vector2(screenWidth, 0f));
    }

    public void GoToMixingArea()
    {
        MoveToPosition(new Vector2(-screenWidth, 0f));
    }

    void MoveToPosition(Vector2 targetPosition)
    {
        if (currentTransition != null)
        {
            StopCoroutine(currentTransition);
        }

        currentTransition = StartCoroutine(SmoothMove(targetPosition));
    }

    IEnumerator SmoothMove(Vector2 targetPosition)
    {
        while (Vector2.Distance(
            gameplayContainer.anchoredPosition,
            targetPosition
        ) > 0.1f)
        {
            gameplayContainer.anchoredPosition =
                Vector2.Lerp(
                    gameplayContainer.anchoredPosition,
                    targetPosition,
                    transitionSpeed * Time.deltaTime
                );

            yield return null;
        }

        gameplayContainer.anchoredPosition = targetPosition;
    }
}