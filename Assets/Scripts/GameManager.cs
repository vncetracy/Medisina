using UnityEngine;

public class GameManager : MonoBehaviour
{
    public TutorialManager tutorialManager;

    void Start()
    {
        if (PlayerPrefs.GetInt("TutorialCompleted", 0) == 0)
        {
            tutorialManager.StartTutorial();
        }
        else
        {
            StartDay1();
        }
    }

    public void CompleteTutorial()
    {
        PlayerPrefs.SetInt("TutorialCompleted", 1);

        PlayerPrefs.Save();

        StartDay1();
    }

    public void StartDay1()
    {
        Debug.Log("DAY 1 STARTED");
    }
}