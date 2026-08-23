using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Day1Manager : MonoBehaviour
{
    [Header("Day Settings")]
    public int totalPatients = 3;

    [Header("Patient Data")]
    public List<PatientData> day1Patients;

    private List<PatientData> shuffledPatients;

    private int currentPatientIndex = 0;

    private bool patientAccepted = false;
    private bool patientIsLeaving = false;

    [Header("Patient Display")]
    public Image patientImage;
    public GameObject patientObject;
    public GameObject patientDialogueUI;
    public TextMeshProUGUI patientDialogueText;

    [Header("Day UI")]
    public GameObject dayUI;
    public TextMeshProUGUI dayText;
    public TextMeshProUGUI patientCounterText;

    [Header("Timer and Patient Life")]
    public GameObject timerUI;
    public GameObject patientLifeUI;
    public TextMeshProUGUI timerText;
    public float startingTime = 60f;

    private float currentTime;
    private bool timerRunning;

    [Header("Patient File")]
    public GameObject patientFilePanel;
    public TextMeshProUGUI patientNameText;
    public TextMeshProUGUI patientAgeText;
    public TextMeshProUGUI patientSymptomsText;
    public TextMeshProUGUI patientAllergyText;

    [Header("Mixing")]
    public MixingManager mixingManager;

    public void StartDay1()
    {
        StartCoroutine(StartDaySequence());
    }

    IEnumerator StartDaySequence()
    {
        ShufflePatients();

        currentPatientIndex = 0;
        patientAccepted = false;
        patientIsLeaving = false;
        timerRunning = false;

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

    void ShufflePatients()
    {
        shuffledPatients = new List<PatientData>(day1Patients);

        for (int i = 0; i < shuffledPatients.Count; i++)
        {
            PatientData temp = shuffledPatients[i];

            int randomIndex =
                Random.Range(i, shuffledPatients.Count);

            shuffledPatients[i] =
                shuffledPatients[randomIndex];

            shuffledPatients[randomIndex] = temp;
        }
    }

    void ShowCurrentPatient()
    {
        if (currentPatientIndex >= shuffledPatients.Count)
        {
            EndDay();
            return;
        }

        StartCoroutine(BringPatient());
    }

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
    }

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
                "NAME: " + currentPatient.patientName;

        if (patientAgeText != null)
            patientAgeText.text =
                "AGE: " + currentPatient.age;

        if (patientSymptomsText != null)
            patientSymptomsText.text =
                "SYMPTOMS:\n" + currentPatient.symptoms;

        if (patientAllergyText != null)
            patientAllergyText.text =
                "ALLERGIES:\n" + currentPatient.allergy;

        if (patientFilePanel != null)
            patientFilePanel.SetActive(true);
    }

    public void ClosePatientFile()
    {
        if (patientFilePanel != null)
            patientFilePanel.SetActive(false);
    }

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
            timerText.text =
                Mathf.CeilToInt(currentTime).ToString();

        timerRunning = true;

        Debug.Log("Patient accepted. Timer started.");
    }

    public void RejectPatient()
    {
        if (patientAccepted || patientIsLeaving)
            return;

        StartCoroutine(RejectPatientSequence());
    }

    IEnumerator RejectPatientSequence()
    {
        patientIsLeaving = true;

        if (patientFilePanel != null)
            patientFilePanel.SetActive(false);

        if (patientDialogueText != null)
            patientDialogueText.text = "BAKLA BAKLA BAKLA!!!.";

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

    public void GiveMedicineToPatient()
    {
        if (!patientAccepted)
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
        }
    }

    IEnumerator CorrectMedicineSequence()
    {
        if (patientDialogueText != null)
            patientDialogueText.text =
                "Thank you, doctor! I feel much better.";

        if (patientDialogueUI != null)
            patientDialogueUI.SetActive(true);

        yield return new WaitForSeconds(2f);

        if (patientObject != null)
            patientObject.SetActive(false);

        if (patientDialogueUI != null)
            patientDialogueUI.SetActive(false);

        currentPatientIndex++;

        patientAccepted = false;

        yield return new WaitForSeconds(1f);

        ShowCurrentPatient();
    }

    void EndDay()
    {
        timerRunning = false;

        Debug.Log("DAY 1 COMPLETE");
    }
}