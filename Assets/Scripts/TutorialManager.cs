using UnityEngine;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public GameObject tutorialOverlay;

    public TextMeshProUGUI tutorialText;

    public GameObject tutorialArrow;

    private int tutorialStep = 0;

    void Start()
    {
        StartTutorial();
    }

    public void StartTutorial()
    {
        tutorialOverlay.SetActive(true);

        tutorialStep = 0;

        ShowStep();
    }

    void ShowStep()
    {
        switch (tutorialStep)
        {
            case 0:

                tutorialText.text =
                    "Welcome to Medisina! Let's learn how to process a patient.";

                break;

            case 1:

                tutorialText.text =
                    "First, click the Patient File to check the patient's information.";

                break;

            case 2:

                tutorialText.text =
                    "Now observe the patient and compare their appearance with their information.";

                break;

            case 3:

                tutorialText.text =
                    "This patient appears legitimate. Click ACCEPT.";

                break;

            case 4:

                tutorialText.text =
                    "Open the Medicine Book to identify the patient's illness.";

                break;

            case 5:

                tutorialText.text =
                    "Select the symptoms you observed.";

                break;

            case 6:

                tutorialText.text =
                    "The Medicine Book has identified the possible illness.";

                break;

            case 7:

                tutorialText.text =
                    "Now open the Medicine Cabinet and prepare the medicine.";

                break;

            case 8:

                tutorialText.text =
                    "Select the correct compounds and mix them.";

                break;

            case 9:

                tutorialText.text =
                    "Return to the patient and give the prepared medicine.";

                break;

            case 10:

                tutorialText.text =
                    "Great! You have completed the tutorial.";

                break;
        }
    }

    public void NextStep()
    {
        tutorialStep++;

        ShowStep();
    }
}