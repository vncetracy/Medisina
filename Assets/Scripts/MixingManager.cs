using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

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
    // ADDITIONAL COMPOUNDS
    // =========================
    // Every compound other than the two above.
    // Fill this list in the Inspector. Add more
    // entries later for new compounds.

    [System.Serializable]
    public class CompoundEntry
    {
        public string compoundName;
        public GameObject cabinetObject;
        public Sprite sprite;
    }

    [Header("Additional Compounds")]

    public CompoundEntry[] additionalCompounds;


    // =========================
    // RECIPES
    // =========================
    // Uses the existing MedicineRecipe ScriptableObject.
    //
    // A recipe matches only when the filled DISHES contain
    // exactly the recipe's Dish ingredients AND the filled
    // FLASKS contain exactly its Flask ingredients.
    // Reversed or extra ingredients do not match.
    //
    // To add a recipe: create a MedicineRecipe asset
    // (Create > Medicine Game > Medicine Recipe) and add it
    // to this list. No code change needed.

    [Header("Recipes")]

    public MedicineRecipe[] recipes;

    // When true, built-in recipes (Paracetamol, Aspirin,
    // Benzocaine, Amoxicillin, Clarithromycin) are added for
    // any medicine name NOT already covered by an asset above.
    public bool useBuiltInRecipes = true;

    // The liquid used as the vehicle in the built-in
    // Amoxicillin and Clarithromycin recipes.
    public string liquidVehicleName = "Water";

    // Optional sprite for the result of a wrong mixture.
    public Sprite wrongMixtureSprite;

    private List<MedicineRecipe> runtimeRecipes =
        new List<MedicineRecipe>();

    private Image resultMedicineImage;
    private Sprite defaultResultSprite;


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
        // SAFE IMAGE ARRAYS
        // =========================

        flaskSlotImages =
            EnsureImageArrayLength(
                flaskSlotImages,
                flaskSlots.Length
            );

        dishSlotImages =
            EnsureImageArrayLength(
                dishSlotImages,
                dishSlots.Length
            );


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
        // CHECK IMAGE REFERENCES
        // =========================

        ValidateSlotImages(
            flaskSlotImages,
            flaskSlots,
            "Flask"
        );

        ValidateSlotImages(
            dishSlotImages,
            dishSlots,
            "Dish"
        );


        // =========================
        // BUILD RECIPE LIST
        // =========================

        BuildRuntimeRecipes();


        // =========================
        // REMEMBER RESULT SPRITE
        // =========================

        if (resultMedicine != null)
        {
            resultMedicineImage =
                resultMedicine.GetComponent<Image>();

            if (resultMedicineImage != null)
            {
                defaultResultSprite =
                    resultMedicineImage.sprite;
            }
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
    // BUILD RUNTIME RECIPES
    // =========================
    // 1) Recipe assets assigned in the Inspector.
    // 2) Built-in recipes for medicines not covered by (1).

    void BuildRuntimeRecipes()
    {
        runtimeRecipes.Clear();


        if (recipes != null)
        {
            for (int i = 0; i < recipes.Length; i++)
            {
                if (recipes[i] != null)
                    runtimeRecipes.Add(recipes[i]);
            }
        }


        if (!useBuiltInRecipes)
            return;


        AddBuiltInRecipe(
            "Paracetamol",
            "p-Aminophenol",
            "Acetic Anhydride"
        );

        AddBuiltInRecipe(
            "Aspirin",
            "Salicylic Acid",
            "Acetic Anhydride"
        );

        AddBuiltInRecipe(
            "Benzocaine",
            "p-Aminobenzoic Acid",
            "Ethanol"
        );

        AddBuiltInRecipe(
            "Amoxicillin",
            "Amoxicillin powder",
            liquidVehicleName
        );

        AddBuiltInRecipe(
            "Clarithromycin",
            "Clarithromycin powder",
            liquidVehicleName
        );
    }


    void AddBuiltInRecipe(
        string medicineName,
        string dishCompound,
        string flaskCompound)
    {
        // Skip if an Inspector asset already
        // defines this medicine.

        for (int i = 0; i < runtimeRecipes.Count; i++)
        {
            if (NamesMatch(
                    runtimeRecipes[i].medicineName,
                    medicineName))
            {
                return;
            }
        }


        MedicineRecipe recipe =
            ScriptableObject.CreateInstance<MedicineRecipe>();

        recipe.name = medicineName + " (built-in)";

        recipe.medicineName = medicineName;

        recipe.ingredients =
            new List<RecipeIngredient>
            {
                new RecipeIngredient
                {
                    compoundName = dishCompound,
                    requiredContainer = ContainerType.Dish
                },

                new RecipeIngredient
                {
                    compoundName = flaskCompound,
                    requiredContainer = ContainerType.Flask
                }
            };

        runtimeRecipes.Add(recipe);
    }


    // =========================
    // IMAGE HELPERS
    // =========================

    Image[] EnsureImageArrayLength(
        Image[] source,
        int length)
    {
        if (source != null &&
            source.Length >= length)
        {
            return source;
        }

        Image[] result = new Image[length];

        if (source != null)
        {
            for (int i = 0; i < source.Length; i++)
                result[i] = source[i];
        }

        return result;
    }


    // Returns the Image for a slot.
    // Valid Inspector assignments are never overwritten.
    // If the reference is missing, falls back to the
    // Image on that SAME slot GameObject (or its children).

    Image ResolveSlotImage(
        Image[] images,
        GameObject[] slots,
        int index,
        string label)
    {
        if (images == null ||
            slots == null ||
            index < 0 ||
            index >= images.Length ||
            index >= slots.Length)
        {
            return null;
        }

        if (images[index] != null)
            return images[index];

        if (slots[index] == null)
            return null;

        Image found =
            slots[index].GetComponent<Image>();

        if (found == null)
        {
            found =
                slots[index]
                    .GetComponentInChildren<Image>(true);
        }

        if (found != null)
        {
            images[index] = found;

            Debug.LogWarning(
                label + "SlotImages[" + index +
                "] was not assigned. Using the Image " +
                "found on " + slots[index].name +
                ". Please assign it in the Inspector."
            );
        }
        else
        {
            Debug.LogWarning(
                label + "SlotImages[" + index +
                "] is missing and " + slots[index].name +
                " has no Image component."
            );
        }

        return found;
    }


    // Warns (does not modify) if an assigned image
    // is shared between slots or is not part of its slot.

    void ValidateSlotImages(
        Image[] images,
        GameObject[] slots,
        string label)
    {
        if (images == null || slots == null)
            return;

        for (int i = 0; i < images.Length; i++)
        {
            if (images[i] == null)
                continue;

            if (i < slots.Length &&
                slots[i] != null &&
                !images[i].transform.IsChildOf(
                    slots[i].transform))
            {
                Debug.LogWarning(
                    label + "SlotImages[" + i +
                    "] is not on or under " +
                    slots[i].name +
                    ". Check the Inspector assignment."
                );
            }

            for (int j = i + 1; j < images.Length; j++)
            {
                if (images[j] == images[i])
                {
                    Debug.LogWarning(
                        label + "SlotImages[" + i +
                        "] and [" + j +
                        "] reference the same Image."
                    );
                }
            }
        }
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
            // Make sure the new container
            // starts empty.

            flaskSlotFilled[emptySlot] = false;

            flaskSlotCompounds[emptySlot] = "";


            flaskSlots[emptySlot].SetActive(true);


            Image flaskImage =
                ResolveSlotImage(
                    flaskSlotImages,
                    flaskSlots,
                    emptySlot,
                    "Flask"
                );


            if (flaskImage != null)
            {
                flaskImage.sprite =
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
            // Make sure the new container
            // starts empty.

            dishSlotFilled[emptySlot] = false;

            dishSlotCompounds[emptySlot] = "";


            dishSlots[emptySlot].SetActive(true);


            Image dishImage =
                ResolveSlotImage(
                    dishSlotImages,
                    dishSlots,
                    emptySlot,
                    "Dish"
                );


            if (dishImage != null)
            {
                dishImage.sprite =
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
    // PICK ANY OTHER COMPOUND
    // =========================
    // Button OnClick can call either of these:
    //   PickCompoundByIndex(int)   -> index in additionalCompounds
    //   PickCompoundByName(string) -> compoundName

    public void PickCompoundByIndex(int index)
    {
        if (isHoldingCompound)
            return;

        if (additionalCompounds == null ||
            index < 0 ||
            index >= additionalCompounds.Length)
        {
            Debug.LogWarning(
                "PickCompoundByIndex: invalid index " +
                index
            );

            return;
        }

        CompoundEntry entry =
            additionalCompounds[index];

        if (entry == null ||
            string.IsNullOrEmpty(entry.compoundName))
        {
            Debug.LogWarning(
                "PickCompoundByIndex: entry " + index +
                " has no compoundName."
            );

            return;
        }

        PickCompoundInternal(
            entry.compoundName,
            entry.cabinetObject,
            entry.sprite
        );
    }


    public void PickCompoundByName(string compoundName)
    {
        if (isHoldingCompound)
            return;

        // Built-in compounds keep their original methods.

        if (NamesMatch(compoundName, "p-Aminophenol"))
        {
            PickPAminophenol();
            return;
        }

        if (NamesMatch(compoundName, "Acetic Anhydride"))
        {
            PickAceticAnhydride();
            return;
        }

        if (additionalCompounds != null)
        {
            for (int i = 0;
                 i < additionalCompounds.Length;
                 i++)
            {
                if (additionalCompounds[i] != null &&
                    NamesMatch(
                        additionalCompounds[i].compoundName,
                        compoundName))
                {
                    PickCompoundByIndex(i);
                    return;
                }
            }
        }

        Debug.LogWarning(
            "PickCompoundByName: unknown compound '" +
            compoundName + "'."
        );
    }


    void PickCompoundInternal(
        string compoundName,
        GameObject cabinetObject,
        Sprite compoundSprite)
    {
        heldCompound =
            compoundName;

        heldCompoundObject =
            cabinetObject;

        isHoldingCompound = true;


        if (cabinetObject != null)
            cabinetObject.SetActive(false);


        if (heldCompoundImage != null &&
            compoundSprite != null)
        {
            heldCompoundImage.sprite =
                compoundSprite;
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


        // Safe image lookup for the container
        // that was actually clicked.

        Image flaskImage =
            ResolveSlotImage(
                flaskSlotImages,
                flaskSlots,
                slotIndex,
                "Flask"
            );


        if (flaskImage != null)
        {
            flaskImage.sprite =
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


        // Safe image lookup for the container
        // that was actually clicked.

        Image dishImage =
            ResolveSlotImage(
                dishSlotImages,
                dishSlots,
                slotIndex,
                "Dish"
            );


        if (dishImage != null)
        {
            dishImage.sprite =
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
        // CHECK RECIPES FIRST
        // =========================
        // CHANGED: uses the MedicineRecipe list
        // instead of the Paracetamol-only check.

        MedicineRecipe matchedRecipe =
            FindMatchingRecipe();

        Sprite resultSpriteToUse =
            defaultResultSprite;


        if (matchedRecipe != null)
        {
            heldMedicineName =
                matchedRecipe.medicineName;


            if (matchedRecipe.resultSprite != null)
            {
                resultSpriteToUse =
                    matchedRecipe.resultSprite;
            }


            Debug.Log(
                "CORRECT MIXTURE! " +
                heldMedicineName
            );
        }
        else
        {
            heldMedicineName =
                "Wrong Mixture";


            if (wrongMixtureSprite != null)
            {
                resultSpriteToUse =
                    wrongMixtureSprite;
            }


            Debug.Log(
                "WRONG MIXTURE!"
            );
        }


        // Update the result sprite (only if
        // one is available).

        if (resultMedicineImage != null &&
            resultSpriteToUse != null)
        {
            resultMedicineImage.sprite =
                resultSpriteToUse;
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
    // FIND MATCHING RECIPE
    // =========================

    MedicineRecipe FindMatchingRecipe()
    {
        List<string> dishContents =
            CollectFilledCompounds(
                dishSlotFilled,
                dishSlotCompounds
            );

        List<string> flaskContents =
            CollectFilledCompounds(
                flaskSlotFilled,
                flaskSlotCompounds
            );


        for (int r = 0; r < runtimeRecipes.Count; r++)
        {
            MedicineRecipe recipe = runtimeRecipes[r];

            if (recipe == null ||
                string.IsNullOrEmpty(recipe.medicineName) ||
                recipe.ingredients == null ||
                recipe.ingredients.Count == 0)
            {
                continue;
            }


            // Split the recipe by required container type.

            List<string> requiredDish =
                new List<string>();

            List<string> requiredFlask =
                new List<string>();


            for (int i = 0;
                 i < recipe.ingredients.Count;
                 i++)
            {
                RecipeIngredient ingredient =
                    recipe.ingredients[i];

                if (ingredient == null ||
                    string.IsNullOrEmpty(
                        ingredient.compoundName))
                {
                    continue;
                }

                if (ingredient.requiredContainer ==
                    ContainerType.Dish)
                {
                    requiredDish.Add(
                        ingredient.compoundName);
                }
                else
                {
                    requiredFlask.Add(
                        ingredient.compoundName);
                }
            }


            if (requiredDish.Count == 0 &&
                requiredFlask.Count == 0)
            {
                continue;
            }


            // Dish compounds must match dishes only,
            // flask compounds must match flasks only.

            if (SameCompounds(
                    dishContents,
                    requiredDish) &&
                SameCompounds(
                    flaskContents,
                    requiredFlask))
            {
                return recipe;
            }
        }


        return null;
    }


    List<string> CollectFilledCompounds(
        bool[] filled,
        string[] compounds)
    {
        List<string> result =
            new List<string>();

        if (filled == null || compounds == null)
            return result;

        for (int i = 0; i < filled.Length; i++)
        {
            if (filled[i] &&
                !string.IsNullOrEmpty(compounds[i]))
            {
                result.Add(compounds[i]);
            }
        }

        return result;
    }


    // True when both lists contain exactly the same
    // compounds (any order, case-insensitive).

    bool SameCompounds(
        List<string> placed,
        List<string> required)
    {
        if (placed.Count != required.Count)
            return false;

        List<string> remaining =
            new List<string>(placed);

        for (int i = 0; i < required.Count; i++)
        {
            int found = -1;

            for (int j = 0; j < remaining.Count; j++)
            {
                if (NamesMatch(remaining[j], required[i]))
                {
                    found = j;
                    break;
                }
            }

            if (found == -1)
                return false;

            remaining.RemoveAt(found);
        }

        return remaining.Count == 0;
    }


    bool NamesMatch(string a, string b)
    {
        if (a == null || b == null)
            return false;

        return string.Equals(
            a.Trim(),
            b.Trim(),
            System.StringComparison.OrdinalIgnoreCase
        );
    }


    // =========================
    // CHECK PARACETAMOL RECIPE
    // =========================
    // LEGACY: kept from the original script.
    // MixCompounds() no longer calls it; the
    // recipe list handles Paracetamol now.

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
        // whether the result is a real medicine
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
    // TRASH CAN SUPPORT
    // =========================

    public bool IsHoldingCompound()
    {
        return isHoldingCompound;
    }

    public bool DiscardHeldCompound()
    {
        if (!isHoldingCompound)
            return false;

        Debug.Log("Discarded compound: " + heldCompound);

        ReturnHeldCompound();

        return true;
    }

    public bool DiscardHeldMedicine()
    {
        if (!isHoldingMedicine)
            return false;

        Debug.Log("Discarded: " + heldMedicineName);

        RemoveHeldMedicine();

        return true;
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