using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public void StartGame()
    {
        // Load the game scene
        SceneManager.LoadScene("GameScene");
    }

    public void OpenMap()
    {
        // Load the game map scene
        SceneManager.LoadScene("GameMap");
    }

    public void OpenSettings()
    {
        // Load the settings scene
        SceneManager.LoadScene("SettingsScene");
    }

    public void OpenCredits()
    {
        // Load the credits scene
        SceneManager.LoadScene("CreditsScene");
    }

    public void QuitGame()
    {
        // Quit the application
        Application.Quit();
    }
}
