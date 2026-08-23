using UnityEngine;
using UnityEngine.UI;

public class MixingManager : MonoBehaviour
{
    public GameObject placedFlask;
    public GameObject placedDish;

    public Image heldCompoundImage;
    public GameObject heldCompoundUI;

    public Sprite emptyFlaskSprite;
    public Sprite filledFlaskSprite;

    public Sprite emptyDishSprite;
    public Sprite filledDishSprite;

    public Image placedFlaskImage;
    public Image placedDishImage;

    public GameObject mixButton;

    public GameObject resultMedicine;

    public GameObject pAminophenolObject;
    public GameObject aceticAnhydrideObject;

    public Sprite pAminophenolSprite;
    public Sprite aceticAnhydrideSprite;

    private string heldCompound;
    private bool isHoldingCompound;

    private GameObject heldCompoundObject;

    private int compoundsPlaced = 0;

    private bool isHoldingMedicine = false;
    public string heldMedicineName;

    void Start()
    {
        if (placedFlask != null)
            placedFlask.SetActive(false);

        if (placedDish != null)
            placedDish.SetActive(false);

        if (heldCompoundUI != null)
            heldCompoundUI.SetActive(false);

        if (mixButton != null)
            mixButton.SetActive(false);

        if (resultMedicine != null)
            resultMedicine.SetActive(false);
    }

    public void SelectFlask()
    {
        if (placedFlask != null)
            placedFlask.SetActive(true);

        if (placedFlaskImage != null && emptyFlaskSprite != null)
            placedFlaskImage.sprite = emptyFlaskSprite;
    }

    public void SelectDish()
    {
        if (placedDish != null)
            placedDish.SetActive(true);

        if (placedDishImage != null && emptyDishSprite != null)
            placedDishImage.sprite = emptyDishSprite;
    }

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
            heldCompoundImage.sprite = pAminophenolSprite;

        if (heldCompoundUI != null)
            heldCompoundUI.SetActive(true);
    }

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
            heldCompoundImage.sprite = aceticAnhydrideSprite;

        if (heldCompoundUI != null)
            heldCompoundUI.SetActive(true);
    }

    public void PlaceCompoundInFlask()
    {
        if (!isHoldingCompound)
            return;

        if (placedFlask == null || !placedFlask.activeSelf)
            return;

        if (placedFlaskImage != null && filledFlaskSprite != null)
            placedFlaskImage.sprite = filledFlaskSprite;

        FinishPlacingCompound();
    }

    public void PlaceCompoundInDish()
    {
        if (!isHoldingCompound)
            return;

        if (placedDish == null || !placedDish.activeSelf)
            return;

        if (placedDishImage != null && filledDishSprite != null)
            placedDishImage.sprite = filledDishSprite;

        FinishPlacingCompound();
    }

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

    void CheckMixButton()
    {
        if (compoundsPlaced >= 2)
        {
            if (mixButton != null)
                mixButton.SetActive(true);
        }
    }

    public void MixCompounds()
    {
        Debug.Log("MIXING COMPOUNDS");

        if (placedFlask != null)
            placedFlask.SetActive(false);

        if (placedDish != null)
            placedDish.SetActive(false);

        if (mixButton != null)
            mixButton.SetActive(false);

        if (resultMedicine != null)
            resultMedicine.SetActive(true);

        compoundsPlaced = 0;
    }

    public void PickResultMedicine()
    {
        isHoldingMedicine = true;
        heldMedicineName = "Paracetamol";

        if (resultMedicine != null)
            resultMedicine.SetActive(false);

        if (heldCompoundImage != null && resultMedicine != null)
        {
            Image resultImage = resultMedicine.GetComponent<Image>();

            if (resultImage != null)
                heldCompoundImage.sprite = resultImage.sprite;
        }

        if (heldCompoundUI != null)
            heldCompoundUI.SetActive(true);
    }

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