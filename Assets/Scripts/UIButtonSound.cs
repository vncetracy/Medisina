
using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonSound : MonoBehaviour,
    IPointerEnterHandler, IPointerClickHandler
{
    [Header("Sound Settings")]
    public string clickSound = "ButtonClick";
    public string hoverSound = "ButtonHover";

    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.Instance.PlaySound(hoverSound);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        AudioManager.Instance.PlaySound(clickSound);
    }
}
