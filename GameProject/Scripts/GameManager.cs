using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    public GameState gameState = new GameState();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadGame()
    {
        if (PlayerPrefs.HasKey("GameSave"))
        {
            string saveData = PlayerPrefs.GetString("GameSave");
            // Implement save data deserialization
            SceneManager.LoadScene("GameScene");
        }
    }

    public void SaveGame()
    {
        string saveData = JsonUtility.ToJson(gameState);
        PlayerPrefs.SetString("GameSave", saveData);
        PlayerPrefs.Save();
    }

    public void StartNewGame()
    {
        gameState = new GameState();
        SceneManager.LoadScene("GameScene");
    }

    public void OpenCustomization()
    {
        SceneManager.LoadScene("CustomizationScene");
    }

    public void OpenCollection()
    {
        SceneManager.LoadScene("CollectionScene");
    }

    public void OpenSettings()
    {
        SceneManager.LoadScene("SettingsScene");
    }
}

[System.Serializable]
public class GameState
{
    public int level = 1;
    public int score = 0;
    public bool[] unlockedDogs = new bool[10];
    public DogCustomization currentDog = new DogCustomization();
}

[System.Serializable]
public class DogCustomization
{
    public int colorIndex = 0;
    public int accessoryIndex = 0;
    public string dogName = "Dog";
}
