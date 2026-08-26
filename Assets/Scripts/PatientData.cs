using UnityEngine;

[System.Serializable]
public class PatientData
{
    [Header("Basic Information")]
    public string patientName;

    public int age;

    [Header("Patient Dialogue")]
    [TextArea(2, 4)]
    public string dialogue;

    [Header("Patient Symptoms")]
    [TextArea(3, 6)]
    public string symptoms;

    [Header("Patient Allergy")]
    public string allergy;

    [Header("Patient Appearance")]
    public Sprite patientSprite;

    public Sprite healthySprite;

    public Sprite deadSprite;

    [Header("Patient Type")]
    public bool isFake;

    [Header("Medical Information")]
    public string illness;

    public string correctMedicine;
}