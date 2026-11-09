using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class SettingsManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject settingsPanel;
    public GameObject mainMenuPanel;        // your existing MainMenuPanel
    public GameObject confirmMainMenuPanel; // small "are you sure?" popup

    [Header("Volume Slider (min 0, max 1)")]
    public Slider masterSlider;             // the ONE slider for all sound

    [Header("Buttons")]
    public GameObject resumeButton;
    public GameObject mainMenuButton;       // hidden on the title screen

    bool isOpen;

    void Start()
    {
        settingsPanel.SetActive(false);
        confirmMainMenuPanel.SetActive(false);

        masterSlider.onValueChanged.AddListener(v => AudioManager.Instance.SetMasterVolume(v));
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isOpen) Resume(); else Open();
        }
    }

    // Hook this to the gear button
    public void Open()
    {
        isOpen = true;
        settingsPanel.SetActive(true);

        masterSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("MasterVol", 1f));

        bool onTitle = mainMenuPanel != null && mainMenuPanel.activeInHierarchy;
        resumeButton.SetActive(true);
        mainMenuButton.SetActive(!onTitle);

        Time.timeScale = 0f;
    }

    public void Resume()
    {
        isOpen = false;
        settingsPanel.SetActive(false);
        confirmMainMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        PlayerPrefs.Save();
    }

    public void AskMainMenu() { confirmMainMenuPanel.SetActive(true); }
    public void CancelMainMenu() { confirmMainMenuPanel.SetActive(false); }

    public void ConfirmMainMenu()
    {
        Time.timeScale = 1f; // never leave the game frozen
        PlayerPrefs.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        PlayerPrefs.Save();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}