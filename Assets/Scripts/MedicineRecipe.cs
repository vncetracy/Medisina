using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RecipeIngredient
{
    public string compoundName;

    public ContainerType requiredContainer;
}

public enum ContainerType
{
    Flask,
    Dish
}

[CreateAssetMenu(
    fileName = "NewMedicineRecipe",
    menuName = "Medicine Game/Medicine Recipe"
)]
public class MedicineRecipe : ScriptableObject
{
    [Header("Medicine")]
    public string medicineName;

    [Header("Required Ingredients")]
    public List<RecipeIngredient> ingredients;
}