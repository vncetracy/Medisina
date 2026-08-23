using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [Header("Screens")]
    public GameObject mainMenuPanel;
    public GameObject gameplayContainer;
    public GameObject dayUI;

    [Header("Managers")]
    public Day1Manager day1Manager;

    void Start()
    {
        mainMenuPanel.SetActive(true);

        gameplayContainer.SetActive(false);

        dayUI.SetActive(false);
    }

    public void PlayGame()
    {
        mainMenuPanel.SetActive(false);

        gameplayContainer.SetActive(true);

        dayUI.SetActive(true);

        day1Manager.StartDay1();
    }
}