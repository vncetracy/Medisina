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

    public Sprite angrySprite;      // shown when a fake patient is caught (rejected)

    public Sprite grinningSprite;   // NEW: shown when a fake patient fools you (you gave them medicine)

    public Sprite faceSprite;       // close-up face shown in the popup

    public Sprite fakeFaceSprite;   // face popup version for fake patients

    [Header("Patient Type")]
    public bool isFake;

    [Header("Medical Information")]
    public string illness;

    public string correctMedicine;
}