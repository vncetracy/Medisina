using UnityEngine;
using UnityEngine.UI;

public class MixingManager : MonoBehaviour
{
    // =========================
    // CONTAINER SLOTS
    // =========================

    [Header("Container Slots")]

    public GameObject[] flaskSlots;
    public GameObject[] dishSlots;

    public Image[] flaskSlotImages;
    public Image[] dishSlotImages;

    private bool[] flaskSlotFilled;
    private bool[] dishSlotFilled;


    // =========================
    // HELD COMPOUND
    // =========================

    [Header("Held Compound")]

    public Image heldCompoundImage;
    public GameObject heldCompoundUI;

    private string heldCompound;
    private bool isHoldingCompound;

    private GameObject heldCompoundObject;


    // =========================
    // CONTAINER SPRITES
    // =========================

    [Header("Flask Sprites")]

    public Sprite emptyFlaskSprite;
    public Sprite filledFlaskSprite;


    [Header("Dish Sprites")]

    public Sprite emptyDishSprite;
    public Sprite filledDishSprite;


    // =========================
    // MIXING
    // =========================

    [Header("Mixing")]

    public GameObject mixButton;
    public GameObject resultMedicine;

    private int compoundsPlaced = 0;


    // =========================
    // COMPOUNDS
    // =========================

    [Header("Compound Objects")]

    public GameObject pAminophenolObject;
    public GameObject aceticAnhydrideObject;

    public Sprite pAminophenolSprite;
    public Sprite aceticAnhydrideSprite;


    // =========================
    // MEDICINE
    // =========================

    private bool isHoldingMedicine = false;
    public string heldMedicineName;


    // =========================
    // START
    // =========================

    void Start()
    {
        // Initialize Flask Slots

        flaskSlotFilled = new bool[flaskSlots.Length];

        for (int i = 0; i < flaskSlots.Length; i++)
        {
            if (flaskSlots[i] != null)
                flaskSlots[i].SetActive(false);

            flaskSlotFilled[i] = false;
        }


        // Initialize Dish Slots

        dishSlotFilled = new bool[dishSlots.Length];

        for (int i = 0; i < dishSlots.Length; i++)
        {
            if (dishSlots[i] != null)
                dishSlots[i].SetActive(false);

            dishSlotFilled[i] = false;
        }


        // Hide Held Compound

        if (heldCompoundUI != null)
            heldCompoundUI.SetActive(false);


        // Hide Mix Button

        if (mixButton != null)
            mixButton.SetActive(false);


        // Hide Result Medicine

        if (resultMedicine != null)
            resultMedicine.SetActive(false);
    }


    // =========================
    // SELECT FLASK
    // =========================

    public void SelectFlask()
    {
        if (isHoldingCompound)
            return;

        int emptySlot = FindEmptyContainerSlot();

        if (emptySlot == -1)
        {
            Debug.Log("No empty slots available.");
            return;
        }

        if (flaskSlots[emptySlot] != null)
        {
            flaskSlots[emptySlot].SetActive(true);

            if (flaskSlotImages[emptySlot] != null)
                flaskSlotImages[emptySlot].sprite =
                    emptyFlaskSprite;
        }
    }


    // =========================
    // SELECT DISH
    // =========================

    public void SelectDish()
    {
        if (isHoldingCompound)
            return;

        int emptySlot = FindEmptyContainerSlot();

        if (emptySlot == -1)
        {
            Debug.Log("No empty slots available.");
            return;
        }

        if (dishSlots[emptySlot] != null)
        {
            dishSlots[emptySlot].SetActive(true);

            if (dishSlotImages[emptySlot] != null)
                dishSlotImages[emptySlot].sprite =
                    emptyDishSprite;
        }
    }


    // =========================
    // FIND EMPTY SLOT
    // =========================

    int FindEmptyContainerSlot()
    {
        for (int i = 0; i < 3; i++)
        {
            bool flaskActive =
                flaskSlots[i] != null &&
                flaskSlots[i].activeSelf;

            bool dishActive =
                dishSlots[i] != null &&
                dishSlots[i].activeSelf;

            if (!flaskActive && !dishActive)
                return i;
        }

        return -1;
    }


    // =========================
    // PICK p-AMINOPHENOL
    // =========================

    public void PickPAminophenol()
    {
        if (isHoldingCompound)
            return;

        heldCompound = "p-Aminophenol";

        heldCompoundObject = pAminophenolObject;

        isHoldingCompound = true;

        if (pAminophenolObject != null)
            pAminophenolObject.SetActive(false);

        if (heldCompoundImage != null)
            heldCompoundImage.sprite =
                pAminophenolSprite;

        if (heldCompoundUI != null)
            heldCompoundUI.SetActive(true);
    }


    // =========================
    // PICK ACETIC ANHYDRIDE
    // =========================

    public void PickAceticAnhydride()
    {
        if (isHoldingCompound)
            return;

        heldCompound = "Acetic Anhydride";

        heldCompoundObject = aceticAnhydrideObject;

        isHoldingCompound = true;

        if (aceticAnhydrideObject != null)
            aceticAnhydrideObject.SetActive(false);

        if (heldCompoundImage != null)
            heldCompoundImage.sprite =
                aceticAnhydrideSprite;

        if (heldCompoundUI != null)
            heldCompoundUI.SetActive(true);
    }


    // =========================
    // PLACE IN FLASK
    // =========================

    public void PlaceCompoundInFlask(int slotIndex)
    {
        if (!isHoldingCompound)
            return;

        if (slotIndex < 0 ||
            slotIndex >= flaskSlots.Length)
            return;

        if (flaskSlots[slotIndex] == null)
            return;

        if (!flaskSlots[slotIndex].activeSelf)
            return;


        // Don't allow placing another compound
        // inside an already filled Flask

        if (flaskSlotFilled[slotIndex])
            return;


        if (flaskSlotImages[slotIndex] != null)
            flaskSlotImages[slotIndex].sprite =
                filledFlaskSprite;


        flaskSlotFilled[slotIndex] = true;

        FinishPlacingCompound();
    }


    // =========================
    // PLACE IN DISH
    // =========================

    public void PlaceCompoundInDish(int slotIndex)
    {
        if (!isHoldingCompound)
            return;

        if (slotIndex < 0 ||
            slotIndex >= dishSlots.Length)
            return;

        if (dishSlots[slotIndex] == null)
            return;

        if (!dishSlots[slotIndex].activeSelf)
            return;


        // Don't allow placing another compound
        // inside an already filled Dish

        if (dishSlotFilled[slotIndex])
            return;


        if (dishSlotImages[slotIndex] != null)
            dishSlotImages[slotIndex].sprite =
                filledDishSprite;


        dishSlotFilled[slotIndex] = true;

        FinishPlacingCompound();
    }


    // =========================
    // FINISH PLACING COMPOUND
    // =========================

    void FinishPlacingCompound()
    {
        if (heldCompoundObject != null)
            heldCompoundObject.SetActive(true);

        heldCompoundObject = null;

        isHoldingCompound = false;

        heldCompound = "";

        compoundsPlaced++;


        if (heldCompoundUI != null)
            heldCompoundUI.SetActive(false);


        CheckMixButton();
    }

    public void ReturnHeldCompound()
    {
        if (!isHoldingCompound)
            return;

        if (heldCompoundObject != null)
            heldCompoundObject.SetActive(true);

        heldCompoundObject = null;

        heldCompound = "";

        isHoldingCompound = false;

        if (heldCompoundUI != null)
            heldCompoundUI.SetActive(false);

        Debug.Log("Compound returned to cabinet.");
    }

    // =========================
    // CHECK MIX BUTTON
    // =========================

    void CheckMixButton()
    {
        if (compoundsPlaced >= 2)
        {
            if (mixButton != null)
                mixButton.SetActive(true);
        }
    }


    // =========================
    // MIX COMPOUNDS
    // =========================

    public void MixCompounds()
    {
        Debug.Log("MIXING COMPOUNDS");


        // Remove all Flask slots

        for (int i = 0; i < flaskSlots.Length; i++)
        {
            if (flaskSlots[i] != null)
                flaskSlots[i].SetActive(false);

            flaskSlotFilled[i] = false;
        }


        // Remove all Dish slots

        for (int i = 0; i < dishSlots.Length; i++)
        {
            if (dishSlots[i] != null)
                dishSlots[i].SetActive(false);

            dishSlotFilled[i] = false;
        }


        if (mixButton != null)
            mixButton.SetActive(false);


        if (resultMedicine != null)
            resultMedicine.SetActive(true);


        compoundsPlaced = 0;
    }


    // =========================
    // PICK RESULT MEDICINE
    // =========================

    public void PickResultMedicine()
    {
        isHoldingMedicine = true;

        heldMedicineName = "Paracetamol";


        if (resultMedicine != null)
            resultMedicine.SetActive(false);


        if (heldCompoundImage != null &&
            resultMedicine != null)
        {
            Image resultImage =
                resultMedicine.GetComponent<Image>();

            if (resultImage != null)
                heldCompoundImage.sprite =
                    resultImage.sprite;
        }


        if (heldCompoundUI != null)
            heldCompoundUI.SetActive(true);
    }


    // =========================
    // MEDICINE FUNCTIONS
    // =========================

    public bool IsHoldingMedicine()
    {
        return isHoldingMedicine;
    }


    public string GetHeldMedicine()
    {
        return heldMedicineName;
    }


    public void RemoveHeldMedicine()
    {
        isHoldingMedicine = false;

        heldMedicineName = "";


        if (heldCompoundUI != null)
            heldCompoundUI.SetActive(false);
    }
}