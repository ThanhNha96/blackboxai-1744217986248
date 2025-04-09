cxzusing UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

/*
This is a helper script to verify all required components are set up correctly.
Attach this to a GameObject in each scene to check for missing requirements.
*/

public class SetupGuide : MonoBehaviour
{
    private List<string> missingItems = new List<string>();

    void Start()
    {
        VerifySetup();
    }

    void VerifySetup()
    {
        // Check for required managers
        if (FindObjectOfType<GameManager>() == null)
            missingItems.Add("GameManager prefab is missing");
        
        if (FindObjectOfType<UIManager>() == null)
            missingItems.Add("UIManager prefab is missing");
            
        if (FindObjectOfType<LocalizationManager>() == null)
            missingItems.Add("LocalizationManager prefab is missing");

        // Check current scene requirements
        string currentScene = SceneManager.GetActiveScene().name;
        switch (currentScene)
        {
            case "MainMenu":
                VerifyMainMenuScene();
                break;
            case "GameScene":
                VerifyGameScene();
                break;
            case "CustomizationScene":
                VerifyCustomizationScene();
                break;
            case "CollectionScene":
                VerifyCollectionScene();
                break;
            case "SettingsScene":
                VerifySettingsScene();
                break;
        }

        // Display results
        if (missingItems.Count > 0)
        {
            Debug.LogError("Missing required setup items:");
            foreach (string item in missingItems)
            {
                Debug.LogError("- " + item);
            }
        }
    }

    void VerifyMainMenuScene()
    {
        // Check for required UI elements
        if (!FindObjectWithTag("StartButton"))
            missingItems.Add("Start Button is missing");
        if (!FindObjectWithTag("ContinueButton"))
            missingItems.Add("Continue Button is missing");
        if (!FindObjectWithTag("CustomizeButton"))
            missingItems.Add("Customize Button is missing");
        if (!FindObjectWithTag("CollectionButton"))
            missingItems.Add("Collection Button is missing");
        if (!FindObjectWithTag("SettingsButton"))
            missingItems.Add("Settings Button is missing");
    }

    void VerifyGameScene()
    {
        // Check for required gameplay elements
        if (!FindObjectWithTag("Player"))
            missingItems.Add("Player prefab is missing");
        if (!FindObjectWithTag("MainCamera"))
            missingItems.Add("Main Camera is missing");
        if (FindObjectOfType<LevelGenerator>() == null)
            missingItems.Add("LevelGenerator is missing");
        if (FindObjectOfType<GameSceneController>() == null)
            missingItems.Add("GameSceneController is missing");
        if (FindObjectOfType<CameraFollow>() == null)
            missingItems.Add("CameraFollow script is missing");
    }

    void VerifyCustomizationScene()
    {
        if (FindObjectOfType<CustomizationSceneController>() == null)
            missingItems.Add("CustomizationSceneController is missing");
        // Check for required customization elements
        if (!FindObjectWithTag("DogModel"))
            missingItems.Add("Dog Model is missing");
    }

    void VerifyCollectionScene()
    {
        if (FindObjectOfType<CollectionSceneController>() == null)
            missingItems.Add("CollectionSceneController is missing");
        if (!FindObjectWithTag("CollectionGrid"))
            missingItems.Add("Collection Grid is missing");
    }

    void VerifySettingsScene()
    {
        if (FindObjectOfType<SettingsSceneController>() == null)
            missingItems.Add("SettingsSceneController is missing");
        
        // Check for required settings UI elements
        var settings = FindObjectOfType<SettingsSceneController>();
        if (settings != null)
        {
            if (settings.languageDropdown == null)
                missingItems.Add("Language Dropdown is missing");
            if (settings.bgmVolumeSlider == null)
                missingItems.Add("BGM Volume Slider is missing");
            if (settings.sfxVolumeSlider == null)
                missingItems.Add("SFX Volume Slider is missing");
        }
    }

    bool FindObjectWithTag(string tag)
    {
        return GameObject.FindGameObjectWithTag(tag) != null;
    }
}
