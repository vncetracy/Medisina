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

    [Tooltip("Correct decisions needed to pass the day. A correct decision is curing a real patient OR rejecting a fake one.")]
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

    private enum PatientResult
    {
        None,
        Cured,          // real patient, correct medicine           (correct)
        CaughtFake,     // fake patient, rejected                   (correct)
        Dead,           // real patient: wrong medicine, time up, or wrongly rejected (mistake)
        FooledByFake    // fake patient was accepted / given medicine (mistake)
    }

    private List<PatientResult> patientResults = new List<PatientResult>();

    private int curedPatients = 0;      // real patients cured
    private int deadPatients = 0;       // real patients who died (incl. wrongly rejected)
    private int caughtFakes = 0;        // fake patients correctly rejected
    private int fooledByFakes = 0;      // fake patients that tricked you


    // =========================================================
    // PATIENT DISPLAY
    // =========================================================

    [Header("Patient Display")]
    public Image patientImage;
    public GameObject patientObject;
    public GameObject patientDialogueUI;
    public TextMeshProUGUI patientDialogueText;

    [Header("Face Popup")]
    public GameObject facePopup;
    public Image faceImage;


    // =========================================================
    // PATIENT ANIMATION
    // =========================================================

    [Header("Patient Animation")]
    public Animator patientAnimator;
    public float cureAnimationWaitTime = 2f;
    public float deathAnimationWaitTime = 2f;
    public float fooledWaitTime = 3f;
    public float fakeRejectedAnimationWaitTime = 2f;


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
    public TimePenaltyPopup penaltyPopup;
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

    [Header("Tutorial")]
    public TutorialManager tutorialManager;
    public string tutorialMedicineName = "Paracetamol";
    public bool timerPaused = false;


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
        caughtFakes = 0;
        fooledByFakes = 0;

        // Reset per-patient results
        patientResults = new List<PatientResult>();
        for (int i = 0; i < shuffledPatients.Count; i++)
            patientResults.Add(PatientResult.None);

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

        if (patientCounterText != null)
            patientCounterText.text = "PATIENT 1 / " + totalPatients;

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
        if (!timerRunning || timerPaused)
            return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0f)
            currentTime = 0f;

        UpdateTimerText();

        if (currentTime <= 0f)
            HandleTimeUp();
    }


    void UpdateTimerText()
    {
        if (timerText != null)
            timerText.text = Mathf.CeilToInt(currentTime).ToString();
    }


    public void PenalizeTime(float seconds)
    {
        if (!timerRunning || timerPaused || patientIsLeaving || seconds <= 0f)
            return;

        // Show only what was really deducted (never more than is left).
        float deducted = Mathf.Min(seconds, currentTime);

        currentTime = Mathf.Max(0f, currentTime - seconds);

        UpdateTimerText();

        if (penaltyPopup != null)
            penaltyPopup.Show(deducted);

        if (currentTime <= 0f)
            HandleTimeUp();
    }


    void HandleTimeUp()
    {
        if (!timerRunning)
            return;

        timerRunning = false;

        Debug.Log("TIME IS UP");

        if (timerUI != null)
            timerUI.SetActive(false);

        if (patientLifeUI != null)
            patientLifeUI.SetActive(false);

        if (mixingManager != null &&
            mixingManager.IsHoldingMedicine())
        {
            mixingManager.RemoveHeldMedicine();
        }

        // A fake patient isn't actually sick, so running out of time on one
        // means they fooled you, not that they died.
        if (IsCurrentPatientFake())
        {
            StartCoroutine(FooledByFakeSequence("Hehe... you wasted your time, doctor. >:D"));
        }
        else
        {
            StartCoroutine(WrongMedicineSequence());
        }
    }


    // =========================================================
    // HELPERS
    // =========================================================

    bool IsCurrentPatientFake()
    {
        if (shuffledPatients == null)
            return false;

        if (currentPatientIndex < 0 ||
            currentPatientIndex >= shuffledPatients.Count)
            return false;

        PatientData p = shuffledPatients[currentPatientIndex];

        return p != null && p.isFake;
    }


    void SetCurrentResult(PatientResult result)
    {
        if (patientResults != null &&
            currentPatientIndex >= 0 &&
            currentPatientIndex < patientResults.Count)
        {
            patientResults[currentPatientIndex] = result;
        }
    }


    // =========================================================
    // SHUFFLE PATIENTS
    // =========================================================

    void ShufflePatients()
    {
        shuffledPatients = new List<PatientData>(day1Patients);

        for (int i = 0; i < shuffledPatients.Count; i++)
        {
            PatientData temp = shuffledPatients[i];

            int randomIndex = Random.Range(i, shuffledPatients.Count);

            shuffledPatients[i] = shuffledPatients[randomIndex];
            shuffledPatients[randomIndex] = temp;
        }

        if (tutorialManager != null && tutorialManager.IsArmed)
        {
            int idx = shuffledPatients.FindIndex(
                p => p != null && p.correctMedicine == tutorialMedicineName);

            if (idx > 0)
            {
                PatientData first = shuffledPatients[0];
                shuffledPatients[0] = shuffledPatients[idx];
                shuffledPatients[idx] = first;
            }
            else if (idx < 0)
            {
                Debug.LogWarning("Tutorial: no patient needs '" +
                    tutorialMedicineName + "'. Check the name.");
            }
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
        CloseFacePopup();

        PatientData currentPatient = shuffledPatients[currentPatientIndex];

        patientAccepted = false;
        patientIsLeaving = false;
        timerRunning = false;

        if (timerUI != null)
            timerUI.SetActive(false);

        if (patientLifeUI != null)
            patientLifeUI.SetActive(false);

        // Current patient's sick sprite
        if (patientImage != null)
            patientImage.sprite = currentPatient.patientSprite;

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
            patientDialogueText.text = currentPatient.dialogue;

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

        if (tutorialManager != null)
            tutorialManager.OnPatientArrived();
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

        PatientData currentPatient = shuffledPatients[currentPatientIndex];

        if (patientNameText != null)
            patientNameText.text = "NAME: " + currentPatient.patientName;

        if (patientAgeText != null)
            patientAgeText.text = "AGE: " + currentPatient.age;

        if (patientSymptomsText != null)
            patientSymptomsText.text = "SYMPTOMS:\n" + currentPatient.symptoms;

        if (patientAllergyText != null)
            patientAllergyText.text = "ALLERGIES:\n" + currentPatient.allergy;

        if (patientFilePanel != null)
            patientFilePanel.SetActive(true);
    }


    public void ClosePatientFile()
    {
        if (patientFilePanel != null)
            patientFilePanel.SetActive(false);
    }


    // =========================================================
    // FACE POPUP
    // =========================================================

    public void ShowFacePopup()
    {
        if (shuffledPatients == null || patientIsLeaving)
            return;

        if (currentPatientIndex >= shuffledPatients.Count)
            return;

        PatientData currentPatient = shuffledPatients[currentPatientIndex];

        // Faking patients show their fake face; everyone else shows the normal one.
        Sprite spriteToShow = currentPatient.isFake
            ? currentPatient.fakeFaceSprite
            : currentPatient.faceSprite;

        if (spriteToShow == null)
        {
            Debug.LogWarning("No face sprite for " + currentPatient.patientName +
                (currentPatient.isFake ? " (fake face)" : ""));
            return;
        }

        if (faceImage != null)
        {
            faceImage.sprite = spriteToShow;
            faceImage.preserveAspect = true;
        }

        if (facePopup != null)
            facePopup.SetActive(true);
    }

    public void CloseFacePopup()
    {
        if (facePopup != null)
            facePopup.SetActive(false);
    }


    // =========================================================
    // ACCEPT PATIENT
    // =========================================================

    public void AcceptPatient()
    {
        if (patientAccepted || patientIsLeaving)
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
            timerText.text = Mathf.CeilToInt(currentTime).ToString();

        timerRunning = true;

        Debug.Log("Patient accepted. Timer started.");
    }


    // =========================================================
    // REJECT PATIENT
    // =========================================================

    public void RejectPatient()
    {
        if (patientAccepted || patientIsLeaving)
            return;

        StartCoroutine(RejectPatientSequence());
    }


    IEnumerator RejectPatientSequence()
    {
        CloseFacePopup();

        patientIsLeaving = true;

        PatientData currentPatient = shuffledPatients[currentPatientIndex];

        if (patientFilePanel != null)
            patientFilePanel.SetActive(false);

        if (currentPatient.isFake)
        {
            // Correct rejection: the faker is caught.
            caughtFakes++;
            SetCurrentResult(PatientResult.CaughtFake);

            if (patientImage != null && currentPatient.angrySprite != null)
                patientImage.sprite = currentPatient.angrySprite;

            if (patientDialogueText != null)
                patientDialogueText.text = "You caught me";

            if (patientDialogueUI != null)
                patientDialogueUI.SetActive(true);

            // NEW: Play fake patient's angry walking animation.
            if (patientAnimator != null)
                patientAnimator.SetTrigger("FakeRejected");

            yield return new WaitForSeconds(fakeRejectedAnimationWaitTime);
        }
        else
        {
            // Wrong rejection: a genuinely sick patient was turned away.
            deadPatients++;
            SetCurrentResult(PatientResult.Dead);

            if (patientDialogueUI != null)
                patientDialogueUI.SetActive(false);

            if (patientImage != null && currentPatient.deadSprite != null)
                patientImage.sprite = currentPatient.deadSprite;

            if (patientAnimator != null)
                patientAnimator.SetTrigger("Death");

            yield return new WaitForSeconds(deathAnimationWaitTime);
        }

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
            Debug.Log("MixingManager is not assigned.");
            return;
        }

        if (!mixingManager.IsHoldingMedicine())
        {
            Debug.Log("Player is not holding medicine.");
            return;
        }

        if (shuffledPatients == null)
            return;

        if (currentPatientIndex >= shuffledPatients.Count)
            return;

        PatientData currentPatient = shuffledPatients[currentPatientIndex];

        string givenMedicine = mixingManager.GetHeldMedicine();

        Debug.Log("Patient needs: " + currentPatient.correctMedicine);
        Debug.Log("Player gave: " + givenMedicine);

        // ---- FAKE PATIENT: any medicine given is a mistake ----
        if (currentPatient.isFake)
        {
            Debug.Log("FAKE PATIENT FOOLED THE PLAYER!");

            mixingManager.RemoveHeldMedicine();

            timerRunning = false;

            if (timerUI != null)
                timerUI.SetActive(false);

            if (patientLifeUI != null)
                patientLifeUI.SetActive(false);

            StartCoroutine(FooledByFakeSequence(
                "haha I got you! this medicine will go to bad use >:D"));

            return;
        }

        // ---- REAL PATIENT ----
        if (givenMedicine == currentPatient.correctMedicine)
        {
            Debug.Log("CORRECT MEDICINE!");

            mixingManager.RemoveHeldMedicine();

            timerRunning = false;

            if (timerUI != null)
                timerUI.SetActive(false);

            if (patientLifeUI != null)
                patientLifeUI.SetActive(false);

            StartCoroutine(CorrectMedicineSequence());
        }
        else
        {
            Debug.Log("WRONG MEDICINE!");

            mixingManager.RemoveHeldMedicine();

            timerRunning = false;

            if (timerUI != null)
                timerUI.SetActive(false);

            if (patientLifeUI != null)
                patientLifeUI.SetActive(false);

            StartCoroutine(WrongMedicineSequence());
        }
    }


    // =========================================================
    // CORRECT MEDICINE
    // =========================================================

    IEnumerator CorrectMedicineSequence()
    {
        CloseFacePopup();

        patientIsLeaving = true;
        curedPatients++;
        SetCurrentResult(PatientResult.Cured);

        PatientData currentPatient = shuffledPatients[currentPatientIndex];

        if (patientDialogueText != null)
            patientDialogueText.text = "Thank you, doctor! I feel much better.";

        if (patientDialogueUI != null)
            patientDialogueUI.SetActive(true);

        if (patientImage != null && currentPatient.healthySprite != null)
            patientImage.sprite = currentPatient.healthySprite;

        if (patientAnimator != null)
            patientAnimator.SetTrigger("Cure");

        yield return new WaitForSeconds(cureAnimationWaitTime);

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
        CloseFacePopup();

        patientIsLeaving = true;
        deadPatients++;
        SetCurrentResult(PatientResult.Dead);

        PatientData currentPatient = shuffledPatients[currentPatientIndex];

        if (patientDialogueText != null)
            patientDialogueText.text = "Something... is wrong...";

        if (patientDialogueUI != null)
            patientDialogueUI.SetActive(true);

        if (patientImage != null && currentPatient.deadSprite != null)
            patientImage.sprite = currentPatient.deadSprite;

        if (patientAnimator != null)
            patientAnimator.SetTrigger("Death");

        yield return new WaitForSeconds(deathAnimationWaitTime);

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
    // FOOLED BY FAKE PATIENT
    // =========================================================

    IEnumerator FooledByFakeSequence(string line)
    {
        CloseFacePopup();

        patientIsLeaving = true;
        fooledByFakes++;
        SetCurrentResult(PatientResult.FooledByFake);

        PatientData currentPatient = shuffledPatients[currentPatientIndex];

        if (patientDialogueText != null)
            patientDialogueText.text = line;

        if (patientDialogueUI != null)
            patientDialogueUI.SetActive(true);

        if (patientImage != null && currentPatient.grinningSprite != null)
            patientImage.sprite = currentPatient.grinningSprite;

        yield return new WaitForSeconds(fooledWaitTime);

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

        // Correct decisions = cured a real patient OR rejected a fake one.
        int correctDecisions = curedPatients + caughtFakes;

        dayPassed = correctDecisions >= minimumPatientsToCure;

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
            dayResultTitle.text = dayPassed ? "DAY 1 COMPLETE" : "DAY 1 FAILED";
        }

        if (dayResultStats != null)
        {
            dayResultStats.text =
                "PATIENTS EXAMINED: " + totalPatients + "\n" +
                "PATIENTS CURED: " + curedPatients + "\n" +
                "PATIENTS DECEASED: " + deadPatients + "\n" +
                "FAKES REJECTED: " + caughtFakes + "\n" +
                "FOOLED BY FAKES: " + fooledByFakes;
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

        if (currentRecordIndex >= shuffledPatients.Count)
        {
            FinishPatientRecords();
            return;
        }

        PatientData patient = shuffledPatients[currentRecordIndex];

        if (patientRecordTitle != null)
            patientRecordTitle.text = "PATIENT RECORD";

        PatientResult recordResult = PatientResult.None;

        if (patientResults != null && currentRecordIndex < patientResults.Count)
            recordResult = patientResults[currentRecordIndex];

        string result;

        switch (recordResult)
        {
            case PatientResult.Cured:
                result = "RESULT: CURED";
                break;

            case PatientResult.CaughtFake:
                result = "RESULT: FAKE PATIENT - REJECTED";
                break;

            case PatientResult.Dead:
                result = "RESULT: DECEASED";
                break;

            case PatientResult.FooledByFake:
                result = "RESULT: FAKE PATIENT - FOOLED YOU";
                break;

            default:
                result = "RESULT: UNKNOWN";
                break;
        }

        // A fake patient has no real illness to treat, so don't show a "correct medicine".
        string medicineLine = patient.isFake
            ? "CORRECT MEDICINE: NONE (FAKE PATIENT)"
            : "CORRECT MEDICINE: " + patient.correctMedicine;

        if (patientRecordText != null)
        {
            patientRecordText.text =
                "NAME: " + patient.patientName + "\n\n" +
                "AGE: " + patient.age + "\n\n" +
                medicineLine + "\n\n" +
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

    // Describes the mistakes made today, e.g.
    // "1 life was lost and 1 impostor fooled you"
    string BuildMistakeSummary()
    {
        List<string> parts = new List<string>();

        if (deadPatients > 0)
        {
            parts.Add(deadPatients +
                (deadPatients == 1 ? " life was lost" : " lives were lost"));
        }

        if (fooledByFakes > 0)
        {
            parts.Add(fooledByFakes +
                (fooledByFakes == 1 ? " impostor fooled you" : " impostors fooled you"));
        }

        return string.Join(" and ", parts);
    }


    void StartSupervisorCall()
    {
        if (supervisorPanel != null)
            supervisorPanel.SetActive(true);

        if (supervisorTitle != null)
            supervisorTitle.text = "SUPERVISOR";

        if (supervisorDialogue != null)
        {
            int correctDecisions = curedPatients + caughtFakes;
            int mistakes = deadPatients + fooledByFakes;

            if (dayPassed)
            {
                if (mistakes == 0)
                {
                    string text = "I've reviewed today's records, Doctor.\n\n";

                    if (caughtFakes > 0)
                    {
                        text +=
                            "You even caught " + caughtFakes +
                            (caughtFakes == 1 ? " impostor" : " impostors") +
                            " trying to fool us.\n\n";
                    }

                    text +=
                        "All patients were handled correctly.\n\n" +
                        "Excellent work. Continue to Day 2.";

                    supervisorDialogue.text = text;
                }
                else
                {
                    supervisorDialogue.text =
                        "I've reviewed today's records, Doctor.\n\n" +
                        correctDecisions + " of " + totalPatients +
                        " patients were handled correctly, but " +
                        BuildMistakeSummary() + ".\n\n" +
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
                        "Too many mistakes were made today: " +
                        BuildMistakeSummary() + ".\n\n" +
                        "You need to be more careful when examining patients and giving medication.\n\n" +
                        "Day 1 must be repeated.";
                }
            }
        }

        if (supervisorNextButton != null)
            supervisorNextButton.gameObject.SetActive(true);
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
                finalResultTitle.text = "DAY 1 COMPLETE";

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
                    buttonText.text = "CONTINUE TO DAY 2";
            }
        }
        else
        {
            if (finalResultTitle != null)
                finalResultTitle.text = "DAY 1 FAILED";

            if (finalResultMessage != null)
            {
                finalResultMessage.text =
                    "Too many mistakes were made.\n\n" +
                    "You must repeat Day 1.";
            }

            if (finalResultButton != null)
            {
                TextMeshProUGUI buttonText =
                    finalResultButton.GetComponentInChildren<TextMeshProUGUI>();

                if (buttonText != null)
                    buttonText.text = "RESTART DAY 1";
            }
        }
    }


    // =========================================================
    // FINAL RESULT BUTTON
    // =========================================================

    public void FinalResultButtonPressed()
    {
        if (dayPassed)
            ContinueToDay2();
        else
            RestartDay1();
    }


    // =========================================================
    // CONTINUE TO DAY 2
    // =========================================================

    void ContinueToDay2()
    {
        Debug.Log("DAY 1 PASSED - CONTINUE TO DAY 2");

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
        Debug.Log("DAY 1 FAILED - RESTARTING");

        if (finalResultPanel != null)
            finalResultPanel.SetActive(false);

        StartCoroutine(RestartDaySequence());
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