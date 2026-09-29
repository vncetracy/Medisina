using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Day1Manager : MonoBehaviour
{
// =========================================================
// DAY SETTINGS
// =========================================================

[Header("Day Settings")]
public int totalPatients = 3;

[Header("Patient Data")]
public List<PatientData> day1Patients;

private List<PatientData> shuffledPatients;

private int currentPatientIndex = 0;

private bool patientAccepted = false;
private bool patientIsLeaving = false;


// =========================================================
// DAY RESULT
// =========================================================

[Header("Day Result")]

public GameObject dayResultPanel;

public TextMeshProUGUI dayResultTitle;

public TextMeshProUGUI dayResultStats;

public int minimumPatientsToCure = 2;


// =========================================================
// PATIENT RECORDS
// =========================================================

[Header("Patient Records")]

public GameObject patientRecordsPanel;

public TextMeshProUGUI patientRecordTitle;

public TextMeshProUGUI patientRecordText;

public TextMeshProUGUI patientRecordCounter;

public Button patientRecordNextButton;

private int currentRecordIndex = 0;


// =========================================================
// SUPERVISOR CALL
// =========================================================

[Header("Supervisor Call")]

public GameObject supervisorPanel;

public TextMeshProUGUI supervisorTitle;

public TextMeshProUGUI supervisorDialogue;

public Button supervisorNextButton;


// =========================================================
// FINAL RESULT
// =========================================================

[Header("Final Result")]

public GameObject finalResultPanel;

public TextMeshProUGUI finalResultTitle;

public TextMeshProUGUI finalResultMessage;

public Button finalResultButton;

private bool dayPassed = false;


// =========================================================
// PATIENT RESULT COUNTERS
// =========================================================

private int curedPatients = 0;
private int deadPatients = 0;
private int rejectedPatients = 0;


// =========================================================
// PATIENT DISPLAY
// =========================================================

[Header("Patient Display")]

public Image patientImage;

public GameObject patientObject;

public GameObject patientDialogueUI;

public TextMeshProUGUI patientDialogueText;


// =========================================================
// PATIENT ANIMATION
// =========================================================

[Header("Patient Animation")]

public Animator patientAnimator;

public float cureAnimationWaitTime = 2f;

public float deathAnimationWaitTime = 2f;


// =========================================================
// DAY UI
// =========================================================

[Header("Day UI")]

public GameObject dayUI;

public TextMeshProUGUI dayText;

public TextMeshProUGUI patientCounterText;


// =========================================================
// TIMER
// =========================================================

[Header("Timer and Patient Life")]

public GameObject timerUI;

public GameObject patientLifeUI;

public TextMeshProUGUI timerText;

public float startingTime = 60f;

private float currentTime;

private bool timerRunning;


// =========================================================
// PATIENT FILE
// =========================================================

[Header("Patient File")]

public GameObject patientFilePanel;

public TextMeshProUGUI patientNameText;

public TextMeshProUGUI patientAgeText;

public TextMeshProUGUI patientSymptomsText;

public TextMeshProUGUI patientAllergyText;


// =========================================================
// MIXING
// =========================================================

[Header("Mixing")]

public MixingManager mixingManager;


// =========================================================
// START
// =========================================================

void Start()
{
    HideEndingUI();
}


// =========================================================
// START DAY 1
// =========================================================

public void StartDay1()
{
    StopAllCoroutines();

    StartCoroutine(StartDaySequence());
}


IEnumerator StartDaySequence()
{
    ShufflePatients();

    currentPatientIndex = 0;

    patientAccepted = false;

    patientIsLeaving = false;

    timerRunning = false;


    // Reset counters

    curedPatients = 0;

    deadPatients = 0;

    rejectedPatients = 0;


    // Reset record index

    currentRecordIndex = 0;

    dayPassed = false;


    // Hide ending UI

    HideEndingUI();


    // Hide patient-related UI

    if (patientObject != null)
        patientObject.SetActive(false);

    if (patientDialogueUI != null)
        patientDialogueUI.SetActive(false);

    if (timerUI != null)
        timerUI.SetActive(false);

    if (patientLifeUI != null)
        patientLifeUI.SetActive(false);

    if (patientFilePanel != null)
        patientFilePanel.SetActive(false);


    // Show day intro

    if (dayUI != null)
        dayUI.SetActive(true);

    if (dayText != null)
        dayText.text = "DAY 1";


    yield return new WaitForSeconds(2f);


    if (dayUI != null)
        dayUI.SetActive(false);


    yield return new WaitForSeconds(0.5f);


    ShowCurrentPatient();
}


// =========================================================
// UPDATE
// =========================================================

void Update()
{
    if (!timerRunning)
        return;


    currentTime -= Time.deltaTime;


    if (currentTime <= 0)
    {
        currentTime = 0;

        timerRunning = false;

        Debug.Log("TIME IS UP");
    }


    if (timerText != null)
    {
        timerText.text =
            Mathf.CeilToInt(currentTime).ToString();
    }
}


// =========================================================
// SHUFFLE PATIENTS
// =========================================================

void ShufflePatients()
{
    shuffledPatients =
        new List<PatientData>(day1Patients);


    for (int i = 0;
         i < shuffledPatients.Count;
         i++)
    {
        PatientData temp =
            shuffledPatients[i];


        int randomIndex =
            Random.Range(
                i,
                shuffledPatients.Count
            );


        shuffledPatients[i] =
            shuffledPatients[randomIndex];


        shuffledPatients[randomIndex] =
            temp;
    }
}


// =========================================================
// SHOW CURRENT PATIENT
// =========================================================

void ShowCurrentPatient()
{
    if (shuffledPatients == null)
        return;


    if (currentPatientIndex >= shuffledPatients.Count)
    {
        EndDay();

        return;
    }


    StartCoroutine(BringPatient());
}


// =========================================================
// BRING PATIENT
// =========================================================

IEnumerator BringPatient()
{
    PatientData currentPatient =
        shuffledPatients[currentPatientIndex];


    patientAccepted = false;

    patientIsLeaving = false;

    timerRunning = false;


    if (timerUI != null)
        timerUI.SetActive(false);


    if (patientLifeUI != null)
        patientLifeUI.SetActive(false);


    // Current patient's sick sprite

    if (patientImage != null)
        patientImage.sprite =
            currentPatient.patientSprite;


    if (patientObject != null)
        patientObject.SetActive(false);


    if (patientDialogueUI != null)
        patientDialogueUI.SetActive(false);


    if (patientFilePanel != null)
        patientFilePanel.SetActive(false);


    yield return new WaitForSeconds(1f);


    if (patientObject != null)
        patientObject.SetActive(true);


    // Reset Animator

    if (patientAnimator != null)
    {
        patientAnimator.Rebind();

        patientAnimator.Update(0f);
    }


    yield return new WaitForSeconds(1f);


    if (patientDialogueText != null)
        patientDialogueText.text =
            currentPatient.dialogue;


    if (patientDialogueUI != null)
        patientDialogueUI.SetActive(true);


    if (patientCounterText != null)
    {
        patientCounterText.text =
            "PATIENT " +
            (currentPatientIndex + 1) +
            " / " +
            totalPatients;
    }
}


// =========================================================
// PATIENT FILE
// =========================================================

public void OpenPatientFile()
{
    if (shuffledPatients == null)
        return;


    if (currentPatientIndex >= shuffledPatients.Count)
        return;


    PatientData currentPatient =
        shuffledPatients[currentPatientIndex];


    if (patientNameText != null)
        patientNameText.text =
            "NAME: " +
            currentPatient.patientName;


    if (patientAgeText != null)
        patientAgeText.text =
            "AGE: " +
            currentPatient.age;


    if (patientSymptomsText != null)
        patientSymptomsText.text =
            "SYMPTOMS:\n" +
            currentPatient.symptoms;


    if (patientAllergyText != null)
        patientAllergyText.text =
            "ALLERGIES:\n" +
            currentPatient.allergy;


    if (patientFilePanel != null)
        patientFilePanel.SetActive(true);
}


public void ClosePatientFile()
{
    if (patientFilePanel != null)
        patientFilePanel.SetActive(false);
}


// =========================================================
// ACCEPT PATIENT
// =========================================================

public void AcceptPatient()
{
    if (patientAccepted ||
        patientIsLeaving)
        return;


    patientAccepted = true;


    if (patientFilePanel != null)
        patientFilePanel.SetActive(false);


    if (patientDialogueUI != null)
        patientDialogueUI.SetActive(false);


    if (timerUI != null)
        timerUI.SetActive(true);


    if (patientLifeUI != null)
        patientLifeUI.SetActive(true);


    currentTime = startingTime;


    if (timerText != null)
        timerText.text =
            Mathf.CeilToInt(currentTime).ToString();


    timerRunning = true;


    Debug.Log(
        "Patient accepted. Timer started."
    );
}


// =========================================================
// REJECT PATIENT
// =========================================================

public void RejectPatient()
{
    if (patientAccepted ||
        patientIsLeaving)
        return;


    StartCoroutine(
        RejectPatientSequence()
    );
}


IEnumerator RejectPatientSequence()
{
    patientIsLeaving = true;

    rejectedPatients++;


    if (patientFilePanel != null)
        patientFilePanel.SetActive(false);


    if (patientDialogueText != null)
        patientDialogueText.text =
            "BAKLA BAKLA BAKLA!!!.";


    if (patientDialogueUI != null)
        patientDialogueUI.SetActive(true);


    yield return new WaitForSeconds(2f);


    if (patientObject != null)
        patientObject.SetActive(false);


    if (patientDialogueUI != null)
        patientDialogueUI.SetActive(false);


    currentPatientIndex++;


    patientAccepted = false;

    patientIsLeaving = false;


    yield return new WaitForSeconds(1f);


    ShowCurrentPatient();
}


// =========================================================
// GIVE MEDICINE
// =========================================================

public void GiveMedicineToPatient()
{
    if (!patientAccepted)
        return;


    if (patientIsLeaving)
        return;


    if (mixingManager == null)
    {
        Debug.Log(
            "MixingManager is not assigned."
        );

        return;
    }


    if (!mixingManager.IsHoldingMedicine())
    {
        Debug.Log(
            "Player is not holding medicine."
        );

        return;
    }


    if (shuffledPatients == null)
        return;


    if (currentPatientIndex >=
        shuffledPatients.Count)
        return;


    PatientData currentPatient =
        shuffledPatients[currentPatientIndex];


    string givenMedicine =
        mixingManager.GetHeldMedicine();


    Debug.Log(
        "Patient needs: " +
        currentPatient.correctMedicine
    );


    Debug.Log(
        "Player gave: " +
        givenMedicine
    );


    if (givenMedicine ==
        currentPatient.correctMedicine)
    {
        Debug.Log(
            "CORRECT MEDICINE!"
        );


        mixingManager.RemoveHeldMedicine();


        timerRunning = false;


        if (timerUI != null)
            timerUI.SetActive(false);


        if (patientLifeUI != null)
            patientLifeUI.SetActive(false);


        StartCoroutine(
            CorrectMedicineSequence()
        );
    }
    else
    {
        Debug.Log(
            "WRONG MEDICINE!"
        );


        mixingManager.RemoveHeldMedicine();


        timerRunning = false;


        if (timerUI != null)
            timerUI.SetActive(false);


        if (patientLifeUI != null)
            patientLifeUI.SetActive(false);


        StartCoroutine(
            WrongMedicineSequence()
        );
    }
}


// =========================================================
// CORRECT MEDICINE
// =========================================================

IEnumerator CorrectMedicineSequence()
{
    patientIsLeaving = true;

    curedPatients++;


    PatientData currentPatient =
        shuffledPatients[currentPatientIndex];


    if (patientDialogueText != null)
        patientDialogueText.text =
            "Thank you, doctor! I feel much better.";


    if (patientDialogueUI != null)
        patientDialogueUI.SetActive(true);


    if (patientImage != null &&
        currentPatient.healthySprite != null)
    {
        patientImage.sprite =
            currentPatient.healthySprite;
    }


    if (patientAnimator != null)
        patientAnimator.SetTrigger("Cure");


    yield return new WaitForSeconds(
        cureAnimationWaitTime
    );


    if (patientObject != null)
        patientObject.SetActive(false);


    if (patientDialogueUI != null)
        patientDialogueUI.SetActive(false);


    currentPatientIndex++;


    patientAccepted = false;

    patientIsLeaving = false;


    yield return new WaitForSeconds(1f);


    ShowCurrentPatient();
}


// =========================================================
// WRONG MEDICINE
// =========================================================

IEnumerator WrongMedicineSequence()
{
    patientIsLeaving = true;

    deadPatients++;


    PatientData currentPatient =
        shuffledPatients[currentPatientIndex];


    if (patientDialogueText != null)
        patientDialogueText.text =
            "Something... is wrong...";


    if (patientDialogueUI != null)
        patientDialogueUI.SetActive(true);


    if (patientImage != null &&
        currentPatient.deadSprite != null)
    {
        patientImage.sprite =
            currentPatient.deadSprite;
    }


    if (patientAnimator != null)
        patientAnimator.SetTrigger("Death");


    yield return new WaitForSeconds(
        deathAnimationWaitTime
    );


    if (patientObject != null)
        patientObject.SetActive(false);


    if (patientDialogueUI != null)
        patientDialogueUI.SetActive(false);


    currentPatientIndex++;


    patientAccepted = false;

    patientIsLeaving = false;


    yield return new WaitForSeconds(1f);


    ShowCurrentPatient();
}


// =========================================================
// END DAY
// =========================================================

void EndDay()
{
    timerRunning = false;


    if (timerUI != null)
        timerUI.SetActive(false);


    if (patientLifeUI != null)
        patientLifeUI.SetActive(false);


    if (patientDialogueUI != null)
        patientDialogueUI.SetActive(false);


    if (patientFilePanel != null)
        patientFilePanel.SetActive(false);


    if (patientObject != null)
        patientObject.SetActive(false);


    dayPassed =
        curedPatients >= minimumPatientsToCure;


    ShowDailyReport();
}


// =========================================================
// DAILY REPORT
// =========================================================

void ShowDailyReport()
{
    if (dayResultPanel != null)
        dayResultPanel.SetActive(true);


    if (dayResultTitle != null)
    {
        if (dayPassed)
        {
            dayResultTitle.text =
                "DAY 1 COMPLETE";
        }
        else
        {
            dayResultTitle.text =
                "DAY 1 FAILED";
        }
    }


    if (dayResultStats != null)
    {
        dayResultStats.text =
            "PATIENTS EXAMINED: " +
            totalPatients +
            "\n\n" +

            "PATIENTS CURED: " +
            curedPatients +
            "\n" +

            "PATIENTS DECEASED: " +
            deadPatients +
            "\n" +

            "PATIENTS REJECTED: " +
            rejectedPatients;
    }
}


// =========================================================
// OPEN PATIENT RECORDS
// =========================================================

public void OpenPatientRecords()
{
    if (dayResultPanel != null)
        dayResultPanel.SetActive(false);


    currentRecordIndex = 0;


    if (patientRecordsPanel != null)
        patientRecordsPanel.SetActive(true);


    ShowPatientRecord();
}


// =========================================================
// SHOW PATIENT RECORD
// =========================================================

void ShowPatientRecord()
{
    if (shuffledPatients == null)
        return;


    if (currentRecordIndex >=
        shuffledPatients.Count)
    {
        FinishPatientRecords();

        return;
    }


    PatientData patient =
        shuffledPatients[currentRecordIndex];


    if (patientRecordTitle != null)
    {
        patientRecordTitle.text =
            "PATIENT RECORD";
    }


    string result;


    if (currentRecordIndex <
        curedPatients)
    {
        result = "RESULT: CURED";
    }
    else
    {
        result = "RESULT: DECEASED / REJECTED";
    }


    if (patientRecordText != null)
    {
        patientRecordText.text =
            "NAME: " +
            patient.patientName +
            "\n\n" +

            "AGE: " +
            patient.age +
            "\n\n" +

            "CORRECT MEDICINE: " +
            patient.correctMedicine +
            "\n\n" +

            result;
    }


    if (patientRecordCounter != null)
    {
        patientRecordCounter.text =
            "RECORD " +
            (currentRecordIndex + 1) +
            " / " +
            shuffledPatients.Count;
    }
}


// =========================================================
// NEXT PATIENT RECORD
// =========================================================

public void NextPatientRecord()
{
    currentRecordIndex++;

    ShowPatientRecord();
}


// =========================================================
// FINISH PATIENT RECORDS
// =========================================================

void FinishPatientRecords()
{
    if (patientRecordsPanel != null)
        patientRecordsPanel.SetActive(false);


    StartSupervisorCall();
}


// =========================================================
// SUPERVISOR CALL
// =========================================================

void StartSupervisorCall()
{
    if (supervisorPanel != null)
        supervisorPanel.SetActive(true);


    if (supervisorTitle != null)
        supervisorTitle.text =
            "SUPERVISOR";


    if (supervisorDialogue != null)
    {
        if (dayPassed)
        {
            if (deadPatients == 0)
            {
                supervisorDialogue.text =
                    "I've reviewed today's records, Doctor.\n\n" +
                    "All patients survived.\n\n" +
                    "Excellent work. Continue to Day 2.";
            }
            else
            {
                supervisorDialogue.text =
                    "I've reviewed today's records, Doctor.\n\n" +
                    curedPatients +
                    " patients survived, but " +
                    deadPatients +
                    " life was lost.\n\n" +
                    "Be more careful tomorrow.\n\n" +
                    "You may continue to Day 2.";
            }
        }
        else
        {
            if (deadPatients >= 2)
            {
                supervisorDialogue.text =
                    "I've reviewed today's records.\n\n" +
                    "You killed " +
                    deadPatients +
                    " patients today.\n\n" +
                    "You were entrusted with their lives, Doctor.\n\n" +
                    "This cannot happen again.\n\n" +
                    "Day 1 must be repeated.";
            }
            else
            {
                supervisorDialogue.text =
                    "I've reviewed today's records.\n\n" +
                    "Too many patients failed to survive today's treatment.\n\n" +
                    "You need to be more careful with your medications.\n\n" +
                    "Day 1 must be repeated.";
            }
        }
    }


    if (supervisorNextButton != null)
    {
        supervisorNextButton.gameObject.SetActive(true);
    }
}


// =========================================================
// SUPERVISOR NEXT
// =========================================================

public void SupervisorNext()
{
    if (supervisorPanel != null)
        supervisorPanel.SetActive(false);


    ShowFinalResult();
}


// =========================================================
// FINAL RESULT
// =========================================================

void ShowFinalResult()
{
    if (finalResultPanel != null)
        finalResultPanel.SetActive(true);


    if (dayPassed)
    {
        if (finalResultTitle != null)
            finalResultTitle.text =
                "DAY 1 COMPLETE";


        if (finalResultMessage != null)
        {
            finalResultMessage.text =
                "You have successfully completed Day 1.\n\n" +
                "Prepare for Day 2.";
        }


        if (finalResultButton != null)
        {
            TextMeshProUGUI buttonText =
                finalResultButton.GetComponentInChildren<TextMeshProUGUI>();

            if (buttonText != null)
                buttonText.text =
                    "CONTINUE TO DAY 2";
        }
    }
    else
    {
        if (finalResultTitle != null)
            finalResultTitle.text =
                "DAY 1 FAILED";


        if (finalResultMessage != null)
        {
            finalResultMessage.text =
                "Too many patients were lost.\n\n" +
                "You must repeat Day 1.";
        }


        if (finalResultButton != null)
        {
            TextMeshProUGUI buttonText =
                finalResultButton.GetComponentInChildren<TextMeshProUGUI>();

            if (buttonText != null)
                buttonText.text =
                    "RESTART DAY 1";
        }
    }
}


// =========================================================
// FINAL RESULT BUTTON
// =========================================================

public void FinalResultButtonPressed()
{
    if (dayPassed)
    {
        ContinueToDay2();
    }
    else
    {
        RestartDay1();
    }
}


// =========================================================
// CONTINUE TO DAY 2
// =========================================================

void ContinueToDay2()
{
    Debug.Log(
        "DAY 1 PASSED - CONTINUE TO DAY 2"
    );

    /*
     * DAY 2 WILL BE CONNECTED HERE LATER.
     *
     * For now this only closes the result panel.
     */

    if (finalResultPanel != null)
        finalResultPanel.SetActive(false);

    Debug.Log("DAY 2 NOT IMPLEMENTED YET.");
}


// =========================================================
// RESTART DAY 1
// =========================================================

void RestartDay1()
{
    Debug.Log(
        "DAY 1 FAILED - RESTARTING"
    );


    if (finalResultPanel != null)
        finalResultPanel.SetActive(false);


    StartCoroutine(
        RestartDaySequence()
    );
}


IEnumerator RestartDaySequence()
{
    yield return new WaitForSeconds(0.5f);


    StartDay1();
}


// =========================================================
// HIDE ENDING UI
// =========================================================

void HideEndingUI()
{
    if (dayResultPanel != null)
        dayResultPanel.SetActive(false);


    if (patientRecordsPanel != null)
        patientRecordsPanel.SetActive(false);


    if (supervisorPanel != null)
        supervisorPanel.SetActive(false);


    if (finalResultPanel != null)
        finalResultPanel.SetActive(false);
}

}
