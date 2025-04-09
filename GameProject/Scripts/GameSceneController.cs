using UnityEngine;
using UnityEngine.UI;

public class GameSceneController : MonoBehaviour
{
    public Text levelText;
    public Text scoreText;
    public Button pauseButton;
    public GameObject pauseMenu;
    public Button resumeButton;
    public Button saveAndQuitButton;

    private void Start()
    {
        UpdateUI();
        SetupButtons();
        Time.timeScale = 1f;
    }

    private void UpdateUI()
    {
        if (levelText != null)
        {
            levelText.text = "Level: " + GameManager.Instance.gameState.level;
        }
        if (scoreText != null)
        {
            scoreText.text = "Score: " + GameManager.Instance.gameState.score;
        }
    }

    private void SetupButtons()
    {
        if (pauseButton != null)
        {
            pauseButton.onClick.AddListener(PauseGame);
        }
        if (resumeButton != null)
        {
            resumeButton.onClick.AddListener(ResumeGame);
        }
        if (saveAndQuitButton != null)
        {
            saveAndQuitButton.onClick.AddListener(SaveAndQuit);
        }
    }

    public void AddScore(int points)
    {
        GameManager.Instance.gameState.score += points;
        UpdateUI();
    }

    public void CompleteLevel()
    {
        GameManager.Instance.gameState.level++;
        UIManager.Instance.ShowMessage("Level Complete!");
        GameManager.Instance.SaveGame();
        UpdateUI();
        // Add level transition logic here
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(true);
        }
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f;
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }
    }

    private void SaveAndQuit()
    {
        GameManager.Instance.SaveGame();
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
