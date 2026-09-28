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

    // NEW:
    // Remembers which compound is inside each container.
    private string[] flaskSlotCompounds;
    private string[] dishSlotCompounds;


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
        // =========================
        // INITIALIZE FLASK SLOTS
        // =========================

        flaskSlotFilled =
            new bool[flaskSlots.Length];

        flaskSlotCompounds =
            new string[flaskSlots.Length];


        for (int i = 0;
             i < flaskSlots.Length;
             i++)
        {
            if (flaskSlots[i] != null)
                flaskSlots[i].SetActive(false);

            flaskSlotFilled[i] = false;

            flaskSlotCompounds[i] = "";
        }


        // =========================
        // INITIALIZE DISH SLOTS
        // =========================

        dishSlotFilled =
            new bool[dishSlots.Length];

        dishSlotCompounds =
            new string[dishSlots.Length];


        for (int i = 0;
             i < dishSlots.Length;
             i++)
        {
            if (dishSlots[i] != null)
                dishSlots[i].SetActive(false);

            dishSlotFilled[i] = false;

            dishSlotCompounds[i] = "";
        }


        // =========================
        // HIDE HELD COMPOUND
        // =========================

        if (heldCompoundUI != null)
            heldCompoundUI.SetActive(false);


        // =========================
        // HIDE MIX BUTTON
        // =========================

        if (mixButton != null)
            mixButton.SetActive(false);


        // =========================
        // HIDE RESULT MEDICINE
        // =========================

        if (resultMedicine != null)
            resultMedicine.SetActive(false);


        // =========================
        // RESET MEDICINE
        // =========================

        isHoldingMedicine = false;

        heldMedicineName = "";

        compoundsPlaced = 0;
    }


    // =========================
    // SELECT FLASK
    // =========================

    public void SelectFlask()
    {
        if (isHoldingCompound)
            return;


        int emptySlot =
            FindEmptyContainerSlot();


        if (emptySlot == -1)
        {
            Debug.Log(
                "No empty slots available."
            );

            return;
        }


        if (flaskSlots[emptySlot] != null)
        {
            flaskSlots[emptySlot].SetActive(true);


            if (flaskSlotImages[emptySlot] != null)
            {
                flaskSlotImages[emptySlot].sprite =
                    emptyFlaskSprite;
            }
        }
    }


    // =========================
    // SELECT DISH
    // =========================

    public void SelectDish()
    {
        if (isHoldingCompound)
            return;


        int emptySlot =
            FindEmptyContainerSlot();


        if (emptySlot == -1)
        {
            Debug.Log(
                "No empty slots available."
            );

            return;
        }


        if (dishSlots[emptySlot] != null)
        {
            dishSlots[emptySlot].SetActive(true);


            if (dishSlotImages[emptySlot] != null)
            {
                dishSlotImages[emptySlot].sprite =
                    emptyDishSprite;
            }
        }
    }


    // =========================
    // FIND EMPTY SLOT
    // =========================

    int FindEmptyContainerSlot()
    {
        int slotCount =
            Mathf.Min(
                3,
                Mathf.Min(
                    flaskSlots.Length,
                    dishSlots.Length
                )
            );


        for (int i = 0;
             i < slotCount;
             i++)
        {
            bool flaskActive =
                flaskSlots[i] != null &&
                flaskSlots[i].activeSelf;


            bool dishActive =
                dishSlots[i] != null &&
                dishSlots[i].activeSelf;


            if (!flaskActive &&
                !dishActive)
            {
                return i;
            }
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


        heldCompound =
            "p-Aminophenol";


        heldCompoundObject =
            pAminophenolObject;


        isHoldingCompound = true;


        if (pAminophenolObject != null)
            pAminophenolObject.SetActive(false);


        if (heldCompoundImage != null)
        {
            heldCompoundImage.sprite =
                pAminophenolSprite;
        }


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


        heldCompound =
            "Acetic Anhydride";


        heldCompoundObject =
            aceticAnhydrideObject;


        isHoldingCompound = true;


        if (aceticAnhydrideObject != null)
            aceticAnhydrideObject.SetActive(false);


        if (heldCompoundImage != null)
        {
            heldCompoundImage.sprite =
                aceticAnhydrideSprite;
        }


        if (heldCompoundUI != null)
            heldCompoundUI.SetActive(true);
    }


    // =========================
    // PLACE IN FLASK
    // =========================

    public void PlaceCompoundInFlask(
        int slotIndex)
    {
        if (!isHoldingCompound)
            return;


        if (slotIndex < 0 ||
            slotIndex >= flaskSlots.Length)
        {
            return;
        }


        if (flaskSlots[slotIndex] == null)
            return;


        if (!flaskSlots[slotIndex].activeSelf)
            return;


        // Don't allow another compound
        // inside an already filled Flask.

        if (flaskSlotFilled[slotIndex])
            return;


        if (flaskSlotImages[slotIndex] != null)
        {
            flaskSlotImages[slotIndex].sprite =
                filledFlaskSprite;
        }


        // IMPORTANT:
        // Remember which compound was placed
        // inside this Flask.

        flaskSlotFilled[slotIndex] = true;

        flaskSlotCompounds[slotIndex] =
            heldCompound;


        Debug.Log(
            heldCompound +
            " placed in FLASK slot " +
            slotIndex
        );


        FinishPlacingCompound();
    }


    // =========================
    // PLACE IN DISH
    // =========================

    public void PlaceCompoundInDish(
        int slotIndex)
    {
        if (!isHoldingCompound)
            return;


        if (slotIndex < 0 ||
            slotIndex >= dishSlots.Length)
        {
            return;
        }


        if (dishSlots[slotIndex] == null)
            return;


        if (!dishSlots[slotIndex].activeSelf)
            return;


        // Don't allow another compound
        // inside an already filled Dish.

        if (dishSlotFilled[slotIndex])
            return;


        if (dishSlotImages[slotIndex] != null)
        {
            dishSlotImages[slotIndex].sprite =
                filledDishSprite;
        }


        // IMPORTANT:
        // Remember which compound was placed
        // inside this Dish.

        dishSlotFilled[slotIndex] = true;

        dishSlotCompounds[slotIndex] =
            heldCompound;


        Debug.Log(
            heldCompound +
            " placed in DISH slot " +
            slotIndex
        );


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


    // =========================
    // RETURN HELD COMPOUND
    // =========================

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


        Debug.Log(
            "Compound returned to cabinet."
        );
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
        else
        {
            if (mixButton != null)
                mixButton.SetActive(false);
        }
    }


    // =========================
    // MIX COMPOUNDS
    // =========================

    public void MixCompounds()
    {
        Debug.Log(
            "MIXING COMPOUNDS"
        );


        // =========================
        // CHECK RECIPE FIRST
        // =========================

        bool correctRecipe =
            IsCorrectParacetamolRecipe();


        if (correctRecipe)
        {
            heldMedicineName =
                "Paracetamol";


            Debug.Log(
                "CORRECT MIXTURE! " +
                "PARACETAMOL"
            );
        }
        else
        {
            heldMedicineName =
                "Wrong Mixture";


            Debug.Log(
                "WRONG MIXTURE!"
            );
        }


        // =========================
        // REMOVE FLASKS
        // =========================

        for (int i = 0;
             i < flaskSlots.Length;
             i++)
        {
            if (flaskSlots[i] != null)
                flaskSlots[i].SetActive(false);


            flaskSlotFilled[i] = false;

            flaskSlotCompounds[i] = "";


            if (flaskSlotImages != null &&
                i < flaskSlotImages.Length &&
                flaskSlotImages[i] != null)
            {
                flaskSlotImages[i].sprite =
                    emptyFlaskSprite;
            }
        }


        // =========================
        // REMOVE DISHES
        // =========================

        for (int i = 0;
             i < dishSlots.Length;
             i++)
        {
            if (dishSlots[i] != null)
                dishSlots[i].SetActive(false);


            dishSlotFilled[i] = false;

            dishSlotCompounds[i] = "";


            if (dishSlotImages != null &&
                i < dishSlotImages.Length &&
                dishSlotImages[i] != null)
            {
                dishSlotImages[i].sprite =
                    emptyDishSprite;
            }
        }


        // =========================
        // HIDE MIX BUTTON
        // =========================

        if (mixButton != null)
            mixButton.SetActive(false);


        // =========================
        // SHOW RESULT
        // =========================

        if (resultMedicine != null)
            resultMedicine.SetActive(true);


        compoundsPlaced = 0;
    }


    // =========================
    // CHECK PARACETAMOL RECIPE
    // =========================

    bool IsCorrectParacetamolRecipe()
    {
        bool hasPAminophenolInDish =
            false;

        bool hasAceticAnhydrideInFlask =
            false;


        // =========================
        // CHECK FLASK
        // =========================

        for (int i = 0;
             i < flaskSlotCompounds.Length;
             i++)
        {
            if (
                flaskSlotCompounds[i] ==
                "Acetic Anhydride"
            )
            {
                hasAceticAnhydrideInFlask =
                    true;

                break;
            }
        }


        // =========================
        // CHECK DISH
        // =========================

        for (int i = 0;
             i < dishSlotCompounds.Length;
             i++)
        {
            if (
                dishSlotCompounds[i] ==
                "p-Aminophenol"
            )
            {
                hasPAminophenolInDish =
                    true;

                break;
            }
        }


        // =========================
        // BOTH MUST BE TRUE
        // =========================

        return
            hasPAminophenolInDish &&
            hasAceticAnhydrideInFlask;
    }


    // =========================
    // PICK RESULT MEDICINE
    // =========================

    public void PickResultMedicine()
    {
        isHoldingMedicine = true;


        // IMPORTANT:
        // DO NOT set this to Paracetamol here.
        //
        // MixCompounds() already decided
        // whether the result is:
        //
        // Paracetamol
        // OR
        // Wrong Mixture


        Debug.Log(
            "Picked up: " +
            heldMedicineName
        );


        if (resultMedicine != null)
            resultMedicine.SetActive(false);


        if (heldCompoundImage != null &&
            resultMedicine != null)
        {
            Image resultImage =
                resultMedicine.GetComponent<Image>();


            if (resultImage != null)
            {
                heldCompoundImage.sprite =
                    resultImage.sprite;
            }
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