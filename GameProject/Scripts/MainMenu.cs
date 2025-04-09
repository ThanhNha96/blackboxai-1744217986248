using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button startButton;
    public Button continueButton;
    public Button customizeButton;
    public Button collectionButton;
    public Button settingsButton;

    void Start()
    {
        startButton.onClick.AddListener(StartGame);
        continueButton.onClick.AddListener(ContinueGame);
        customizeButton.onClick.AddListener(CustomizeCharacter);
        collectionButton.onClick.AddListener(ViewCollection);
        settingsButton.onClick.AddListener(OpenSettings);

        // Enable/disable continue button based on save existence
        continueButton.interactable = PlayerPrefs.HasKey("GameSave");
    }

    void StartGame()
    {
        UIManager.Instance.AnimateButtonClick(startButton);
        UIManager.Instance.ShowLoadingScreen();
        GameManager.Instance.StartNewGame();
    }

    void ContinueGame()
    {
        if (PlayerPrefs.HasKey("GameSave"))
        {
            UIManager.Instance.AnimateButtonClick(continueButton);
            UIManager.Instance.ShowLoadingScreen();
            GameManager.Instance.LoadGame();
        }
        else
        {
            UIManager.Instance.ShowMessage("No saved game found!");
        }
    }

    void CustomizeCharacter()
    {
        UIManager.Instance.AnimateButtonClick(customizeButton);
        GameManager.Instance.OpenCustomization();
    }

    void ViewCollection()
    {
        UIManager.Instance.AnimateButtonClick(collectionButton);
        GameManager.Instance.OpenCollection();
    }

    void OpenSettings()
    {
        UIManager.Instance.AnimateButtonClick(settingsButton);
        GameManager.Instance.OpenSettings();
    }
}
