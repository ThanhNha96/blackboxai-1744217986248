using UnityEngine;
using UnityEngine.UI;

public class SettingsSceneController : MonoBehaviour
{
    public Slider bgmVolumeSlider;
    public Slider sfxVolumeSlider;
    public Toggle fullscreenToggle;
    public Button backButton;
    public Button resetButton;
    public Button resetProgressButton;
    public GameObject confirmationDialog;
    public Button confirmResetButton;
    public Button cancelResetButton;
    
    // Language selection UI
    public Dropdown languageDropdown;
    public LocalizedText[] localizedTexts;

    private const string BGM_VOLUME_KEY = "BGMVolume";
    private const string SFX_VOLUME_KEY = "SFXVolume";
    private const string FULLSCREEN_KEY = "Fullscreen";

    private void Start()
    {
        SetupLanguageDropdown();
        LoadSettings();
        SetupUI();
    }

    private void SetupLanguageDropdown()
    {
        if (languageDropdown != null)
        {
            languageDropdown.ClearOptions();
            
            // Add language options
            var options = new List<string>
            {
                "English",
                "Tiếng Việt",
                "中文",
                "한국어",
                "日本語"
            };
            
            languageDropdown.AddOptions(options);
            
            // Set current language
            languageDropdown.value = (int)LocalizationManager.Instance.currentLanguage;
            
            // Add listener for language change
            languageDropdown.onValueChanged.AddListener(OnLanguageChanged);
        }
    }

    private void OnLanguageChanged(int index)
    {
        LocalizationManager.Instance.SetLanguage((LocalizationManager.Language)index);
    }

    private void LoadSettings()
    {
        if (bgmVolumeSlider != null)
            bgmVolumeSlider.value = PlayerPrefs.GetFloat(BGM_VOLUME_KEY, 1f);
        
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.value = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1f);
        
        if (fullscreenToggle != null)
            fullscreenToggle.isOn = PlayerPrefs.GetInt(FULLSCREEN_KEY, 1) == 1;
    }

    private void SetupUI()
    {
        if (bgmVolumeSlider != null)
            bgmVolumeSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        
        if (fullscreenToggle != null)
            fullscreenToggle.onValueChanged.AddListener(OnFullscreenChanged);
        
        if (backButton != null)
            backButton.onClick.AddListener(SaveAndReturn);
        
        if (resetButton != null)
            resetButton.onClick.AddListener(ResetSettings);
        
        if (resetProgressButton != null)
            resetProgressButton.onClick.AddListener(ShowResetConfirmation);
        
        if (confirmResetButton != null)
            confirmResetButton.onClick.AddListener(ConfirmResetProgress);
        
        if (cancelResetButton != null)
            cancelResetButton.onClick.AddListener(HideResetConfirmation);
        
        if (confirmationDialog != null)
            confirmationDialog.SetActive(false);
    }

    private void OnBGMVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat(BGM_VOLUME_KEY, value);
        // Here you would update the actual audio volume
        // AudioManager.Instance.SetBGMVolume(value);
    }

    private void OnSFXVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, value);
        // Here you would update the actual audio volume
        // AudioManager.Instance.SetSFXVolume(value);
    }

    private void OnFullscreenChanged(bool isFullscreen)
    {
        PlayerPrefs.SetInt(FULLSCREEN_KEY, isFullscreen ? 1 : 0);
        Screen.fullScreen = isFullscreen;
    }

    private void SaveAndReturn()
    {
        PlayerPrefs.Save();
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    private void ResetSettings()
    {
        if (bgmVolumeSlider != null)
            bgmVolumeSlider.value = 1f;
        
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.value = 1f;
        
        if (fullscreenToggle != null)
            fullscreenToggle.isOn = true;
        
        PlayerPrefs.SetFloat(BGM_VOLUME_KEY, 1f);
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, 1f);
        PlayerPrefs.SetInt(FULLSCREEN_KEY, 1);
        
        UIManager.Instance.ShowMessage("Settings reset to default!");
    }

    private void ShowResetConfirmation()
    {
        if (confirmationDialog != null)
        {
            confirmationDialog.SetActive(true);
        }
    }

    private void HideResetConfirmation()
    {
        if (confirmationDialog != null)
        {
            confirmationDialog.SetActive(false);
        }
    }

    private void ConfirmResetProgress()
    {
        // Reset game progress
        PlayerPrefs.DeleteKey("GameSave");
        GameManager.Instance.gameState = new GameState();
        
        UIManager.Instance.ShowMessage("Game progress has been reset!");
        HideResetConfirmation();
        
        // Return to main menu after reset
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
