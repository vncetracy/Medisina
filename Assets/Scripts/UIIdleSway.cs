
using UnityEngine;

public class UIIdleSway : MonoBehaviour
{
    [Header("Position")]
    [SerializeField] private float positionAmount = 2f;
    [SerializeField] private float positionSpeed = 0.4f;

    [Header("Rotation")]
    [SerializeField] private float rotationAmount = 0.12f;
    [SerializeField] private float rotationSpeed = 0.3f;

    [Header("Natural Noise")]
    [SerializeField] private float noiseAmount = 1.5f;
    [SerializeField] private float noiseSpeed = 0.4f;

    private RectTransform rectTransform;
    private Vector2 startPosition;
    private Quaternion startRotation;
    private float randomOffset;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        startPosition = rectTransform.anchoredPosition;
        startRotation = rectTransform.localRotation;

        randomOffset = Random.Range(0f, 100f);
    }

    private void LateUpdate()
    {
        float time = Time.time;

        float swayX = Mathf.Sin(
            time * positionSpeed + randomOffset
        );

        float swayY = Mathf.Sin(
            time * positionSpeed * 0.7f + randomOffset
        );

        float noiseX = (
            Mathf.PerlinNoise(time * noiseSpeed, randomOffset) - 0.5f
        ) * 2f;

        float noiseY = (
            Mathf.PerlinNoise(randomOffset, time * noiseSpeed) - 0.5f
        ) * 2f;

        Vector2 movement = new Vector2(
            swayX * positionAmount + noiseX * noiseAmount,
            swayY * positionAmount + noiseY * noiseAmount
        );

        rectTransform.anchoredPosition =
            startPosition + movement;

        float rotation = Mathf.Sin(
            time * rotationSpeed + randomOffset
        ) * rotationAmount;

        rectTransform.localRotation =
            startRotation * Quaternion.Euler(0f, 0f, rotation);
    }
}
