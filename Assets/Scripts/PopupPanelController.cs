using UnityEngine;

public class BookPopupController : MonoBehaviour
{
    [Header("Book Content Panels")]
    public GameObject bookPanel1;
    public GameObject bookPanel2;
    public GameObject bookPanel3;

    [Header("Background")]
    public GameObject blurPanel;

    void Start()
    {
        // Hide everything when the game starts
        CloseAllBooks();
    }

    public void OpenBook1()
    {
        CloseAllBooks();

        bookPanel1.SetActive(true);
        blurPanel.SetActive(true);
    }

    public void OpenBook2()
    {
        CloseAllBooks();

        bookPanel2.SetActive(true);
        blurPanel.SetActive(true);
    }

    public void OpenBook3()
    {
        CloseAllBooks();

        bookPanel3.SetActive(true);
        blurPanel.SetActive(true);
    }

    public void CloseBooks()
    {
        CloseAllBooks();
    }

    void CloseAllBooks()
    {
        bookPanel1.SetActive(false);
        bookPanel2.SetActive(false);
        bookPanel3.SetActive(false);

        blurPanel.SetActive(false);
    }
}