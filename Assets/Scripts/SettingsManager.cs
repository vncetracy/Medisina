using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class SettingsManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject settingsPanel;
    public GameObject mainMenuPanel;
    public GameObject confirmMainMenuPanel;

    [Header("Volume Slider (min 0, max 1)")]
    public Slider masterSlider;

    [Header("Buttons")]
    public GameObject resumeButton;
    public GameObject mainMenuButton;

    [Header("Pull-up Animation")]
    [Tooltip("Seconds to let the open animation play before pausing. Frames / Samples (e.g. 15 / 60 = 0.25)")]
    public float pauseAfterSeconds = 0.25f;

    bool isOpen;
    Coroutine pauseRoutine;

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
        if (isOpen) return;

        isOpen = true;
        settingsPanel.SetActive(true);

        masterSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("MasterVol", 1f));

        bool onTitle = mainMenuPanel != null && mainMenuPanel.activeInHierarchy;
        resumeButton.SetActive(true);
        mainMenuButton.SetActive(!onTitle);

        // Let the animation finish first, then freeze the game.
        if (pauseRoutine != null) StopCoroutine(pauseRoutine);
        pauseRoutine = StartCoroutine(PauseAfterAnimation());
    }

    IEnumerator PauseAfterAnimation()
    {
        yield return new WaitForSecondsRealtime(pauseAfterSeconds);
        Time.timeScale = 0f;
        pauseRoutine = null;
    }

    public void Resume()
    {
        isOpen = false;

        // Cancel a pending pause if the panel is closed mid-animation.
        if (pauseRoutine != null)
        {
            StopCoroutine(pauseRoutine);
            pauseRoutine = null;
        }

        settingsPanel.SetActive(false);
        confirmMainMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        PlayerPrefs.Save();
    }

    public void AskMainMenu() { confirmMainMenuPanel.SetActive(true); }
    public void CancelMainMenu() { confirmMainMenuPanel.SetActive(false); }

    public void ConfirmMainMenu()
    {
        if (pauseRoutine != null) StopCoroutine(pauseRoutine);
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