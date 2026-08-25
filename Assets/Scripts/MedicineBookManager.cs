using System.Collections;
using UnityEngine;
using TMPro;

public class MedicineBookManager : MonoBehaviour
{
    // =========================
    // PAGES
    // =========================

    [Header("Pages")]
    public GameObject guidePage;
    public GameObject symptomsPage;
    public GameObject conditionsPage;
    public GameObject compoundsPage;
    public GameObject medicinesPage;

    private int currentPage = 0;


    // =========================
    // PAGE ANIMATION
    // =========================

    [Header("Page Animation")]
    public CanvasGroup bookPagesCanvasGroup;
    public float pageFlipSpeed = 0.2f;

    private bool isChangingPage = false;


    // =========================
    // MEDICINE INFORMATION
    // =========================

    [Header("Medicine Information")]
    public TextMeshProUGUI medicineInfoText;


    // =========================
    // COMPOUND INFORMATION
    // =========================

    [Header("Compound Information")]
    public TextMeshProUGUI compoundInfoText;


    // =========================
    // SYMPTOM CIRCLES
    // =========================

    [Header("Symptom Circles")]

    public GameObject feverCircle;
    public GameObject headacheCircle;
    public GameObject muscleAchesCircle;
    public GameObject bodyPainCircle;
    public GameObject coldCircle;
    public GameObject runnyNoseCircle;
    public GameObject coughCircle;
    public GameObject soreThroatCircle;


    // =========================
    // SYMPTOM SELECTION
    // =========================

    private bool feverSelected;
    private bool headacheSelected;
    private bool muscleAchesSelected;
    private bool bodyPainSelected;
    private bool coldSelected;
    private bool runnyNoseSelected;
    private bool coughSelected;
    private bool soreThroatSelected;


    // =========================
    // CONDITIONS
    // =========================

    [Header("Condition Texts")]

    public TextMeshProUGUI feverMildPainConditionText;
    public TextMeshProUGUI coldConditionText;
    public TextMeshProUGUI fluConditionText;
    public TextMeshProUGUI migraineConditionText;
    public TextMeshProUGUI infectionConditionText;


    [Header("Condition Colors")]

    public Color normalConditionColor = Color.white;

    public Color highlightedConditionColor =
        new Color(1f, 0.85f, 0.2f);


    // =========================
    // START
    // =========================

    void Start()
    {
        if (bookPagesCanvasGroup != null)
            bookPagesCanvasGroup.alpha = 1f;

        // Hide all symptom circles at the start

        if (feverCircle != null)
            feverCircle.SetActive(false);

        if (headacheCircle != null)
            headacheCircle.SetActive(false);

        if (muscleAchesCircle != null)
            muscleAchesCircle.SetActive(false);

        if (bodyPainCircle != null)
            bodyPainCircle.SetActive(false);

        if (coldCircle != null)
            coldCircle.SetActive(false);

        if (runnyNoseCircle != null)
            runnyNoseCircle.SetActive(false);

        if (coughCircle != null)
            coughCircle.SetActive(false);

        if (soreThroatCircle != null)
            soreThroatCircle.SetActive(false);


        ShowPage(0);

        UpdateConditionHighlights();
    }


    // =========================
    // PAGE SYSTEM
    // =========================

    public void NextPage()
    {
        if (isChangingPage)
            return;

        if (currentPage < 4)
        {
            StartCoroutine(
                ChangePage(currentPage + 1)
            );
        }
    }


    public void PreviousPage()
    {
        if (isChangingPage)
            return;

        if (currentPage > 0)
        {
            StartCoroutine(
                ChangePage(currentPage - 1)
            );
        }
    }


    IEnumerator ChangePage(int newPage)
    {
        isChangingPage = true;


        // FADE OUT

        if (bookPagesCanvasGroup != null)
        {
            float timer = 0f;

            while (timer < pageFlipSpeed)
            {
                timer += Time.deltaTime;

                bookPagesCanvasGroup.alpha =
                    Mathf.Lerp(
                        1f,
                        0f,
                        timer / pageFlipSpeed
                    );

                yield return null;
            }

            bookPagesCanvasGroup.alpha = 0f;
        }


        // CHANGE PAGE

        currentPage = newPage;

        ShowPage(currentPage);


        // FADE IN

        if (bookPagesCanvasGroup != null)
        {
            float timer = 0f;

            while (timer < pageFlipSpeed)
            {
                timer += Time.deltaTime;

                bookPagesCanvasGroup.alpha =
                    Mathf.Lerp(
                        0f,
                        1f,
                        timer / pageFlipSpeed
                    );

                yield return null;
            }

            bookPagesCanvasGroup.alpha = 1f;
        }


        isChangingPage = false;
    }


    void ShowPage(int pageNumber)
    {
        if (guidePage != null)
            guidePage.SetActive(pageNumber == 0);

        if (symptomsPage != null)
            symptomsPage.SetActive(pageNumber == 1);

        if (conditionsPage != null)
            conditionsPage.SetActive(pageNumber == 2);

        if (compoundsPage != null)
            compoundsPage.SetActive(pageNumber == 3);

        if (medicinesPage != null)
            medicinesPage.SetActive(pageNumber == 4);
    }


    // =========================
    // SYMPTOM BUTTONS
    // CLICK ONCE = SELECT
    // CLICK AGAIN = UNSELECT
    // =========================

    public void ToggleFever()
    {
        feverSelected = !feverSelected;

        if (feverCircle != null)
            feverCircle.SetActive(feverSelected);

        UpdateConditionHighlights();
    }


    public void ToggleHeadache()
    {
        headacheSelected = !headacheSelected;

        if (headacheCircle != null)
            headacheCircle.SetActive(headacheSelected);

        UpdateConditionHighlights();
    }


    public void ToggleMuscleAches()
    {
        muscleAchesSelected = !muscleAchesSelected;

        if (muscleAchesCircle != null)
            muscleAchesCircle.SetActive(muscleAchesSelected);

        UpdateConditionHighlights();
    }


    public void ToggleBodyPain()
    {
        bodyPainSelected = !bodyPainSelected;

        if (bodyPainCircle != null)
            bodyPainCircle.SetActive(bodyPainSelected);

        UpdateConditionHighlights();
    }


    public void ToggleCold()
    {
        coldSelected = !coldSelected;

        if (coldCircle != null)
            coldCircle.SetActive(coldSelected);

        UpdateConditionHighlights();
    }


    public void ToggleRunnyNose()
    {
        runnyNoseSelected = !runnyNoseSelected;

        if (runnyNoseCircle != null)
            runnyNoseCircle.SetActive(runnyNoseSelected);

        UpdateConditionHighlights();
    }


    public void ToggleCough()
    {
        coughSelected = !coughSelected;

        if (coughCircle != null)
            coughCircle.SetActive(coughSelected);

        UpdateConditionHighlights();
    }


    public void ToggleSoreThroat()
    {
        soreThroatSelected = !soreThroatSelected;

        if (soreThroatCircle != null)
            soreThroatCircle.SetActive(soreThroatSelected);

        UpdateConditionHighlights();
    }


    // =========================
    // CONDITION LOGIC
    // =========================

    void UpdateConditionHighlights()
    {
        // RESET ALL CONDITIONS

        if (feverMildPainConditionText != null)
            feverMildPainConditionText.color =
                normalConditionColor;

        if (coldConditionText != null)
            coldConditionText.color =
                normalConditionColor;

        if (fluConditionText != null)
            fluConditionText.color =
                normalConditionColor;

        if (migraineConditionText != null)
            migraineConditionText.color =
                normalConditionColor;

        if (infectionConditionText != null)
            infectionConditionText.color =
                normalConditionColor;


        // FEVER / MILD PAIN

        if (feverSelected &&
            (bodyPainSelected || headacheSelected))
        {
            if (feverMildPainConditionText != null)
                feverMildPainConditionText.color =
                    highlightedConditionColor;
        }


        // COLD

        if (coldSelected ||
            (runnyNoseSelected &&
            (coughSelected || soreThroatSelected)))
        {
            if (coldConditionText != null)
                coldConditionText.color =
                    highlightedConditionColor;
        }


        // FLU

        int fluSymptoms = 0;

        if (feverSelected)
            fluSymptoms++;

        if (headacheSelected)
            fluSymptoms++;

        if (muscleAchesSelected)
            fluSymptoms++;

        if (runnyNoseSelected)
            fluSymptoms++;

        if (coughSelected)
            fluSymptoms++;

        if (soreThroatSelected)
            fluSymptoms++;

        if (fluSymptoms >= 2)
        {
            if (fluConditionText != null)
                fluConditionText.color =
                    highlightedConditionColor;
        }


        // MIGRAINE

        if (headacheSelected &&
            !feverSelected &&
            !coughSelected &&
            !runnyNoseSelected)
        {
            if (migraineConditionText != null)
                migraineConditionText.color =
                    highlightedConditionColor;
        }


        // INFECTION

        int infectionSymptoms = 0;

        if (feverSelected)
            infectionSymptoms++;

        if (bodyPainSelected)
            infectionSymptoms++;

        if (soreThroatSelected)
            infectionSymptoms++;

        if (infectionSymptoms >= 2)
        {
            if (infectionConditionText != null)
                infectionConditionText.color =
                    highlightedConditionColor;
        }
    }


    // =========================
    // COMPOUNDS
    // =========================

    public void ShowPAminophenolInfo()
    {
        if (compoundInfoText != null)
        {
            compoundInfoText.text =
                "p-AMINOPHENOL\n\n" +
                "Type: Solid\n\n" +
                "Description:\n" +
                "A chemical compound used as one of the ingredients for creating Paracetamol.\n\n" +
                "Can be combined with:\n" +
                "Acetic Anhydride";
        }
    }


    public void ShowAceticAnhydrideInfo()
    {
        if (compoundInfoText != null)
        {
            compoundInfoText.text =
                "ACETIC ANHYDRIDE\n\n" +
                "Type: Liquid\n\n" +
                "Description:\n" +
                "A chemical compound used in the production of Paracetamol.\n\n" +
                "Can be combined with:\n" +
                "p-Aminophenol";
        }
    }


    // =========================
    // MEDICINES
    // =========================

    public void ShowParacetamol()
    {
        if (medicineInfoText != null)
        {
            medicineInfoText.text =
                "PARACETAMOL\n\n" +
                "Used for:\n" +
                "• Fever\n" +
                "• Headache\n" +
                "• Muscle aches\n" +
                "• Mild pain\n\n" +
                "Can be made by mixing:\n\n" +
                "p-Aminophenol\n" +
                "+\n" +
                "Acetic Anhydride";
        }
    }


    public void ShowAspirin()
    {
        if (medicineInfoText != null)
        {
            medicineInfoText.text =
                "ASPIRIN\n\n" +
                "Used for:\n" +
                "• Pain relief\n" +
                "• Fever\n" +
                "• Inflammation\n\n" +
                "Recipe information coming soon.";
        }
    }


    public void ShowIbuprofen()
    {
        if (medicineInfoText != null)
        {
            medicineInfoText.text =
                "IBUPROFEN\n\n" +
                "Used for:\n" +
                "• Pain relief\n" +
                "• Fever\n" +
                "• Inflammation\n\n" +
                "Recipe information coming soon.";
        }
    }
}