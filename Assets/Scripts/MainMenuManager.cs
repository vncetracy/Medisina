using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [Header("Screens")]
    public GameObject mainMenuPanel;
    public GameObject gameplayContainer;
    public GameObject dayUI;

    [Header("Managers")]
    public Day1Manager day1Manager;

    [Tooltip("Optional. If assigned, the guided tutorial plays with the first patient (first time only).")]
    public TutorialManager tutorialManager;

    void Start()
    {
        // Coming back after the tutorial: skip the title screen
        // and start the real Day 1 right away.
        if (TutorialManager.skipTitleOnLoad)
        {
            TutorialManager.skipTitleOnLoad = false;

            mainMenuPanel.SetActive(false);

            gameplayContainer.SetActive(true);

            dayUI.SetActive(true);

            day1Manager.StartDay1();

            return;
        }

        mainMenuPanel.SetActive(true);

        gameplayContainer.SetActive(false);

        dayUI.SetActive(false);
    }

    public void PlayGame()
    {
        mainMenuPanel.SetActive(false);

        gameplayContainer.SetActive(true);

        dayUI.SetActive(true);

        // The tutorial starts by itself when the first patient arrives.
        if (tutorialManager != null && !tutorialManager.IsCompleted())
            tutorialManager.Arm();

        day1Manager.StartDay1();
    }
}