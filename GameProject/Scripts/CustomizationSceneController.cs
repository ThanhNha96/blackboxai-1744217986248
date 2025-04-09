using UnityEngine;
using UnityEngine.UI;

public class CustomizationSceneController : MonoBehaviour
{
    public InputField nameInput;
    public Button nextColorButton;
    public Button prevColorButton;
    public Button nextAccessoryButton;
    public Button prevAccessoryButton;
    public Button saveButton;
    public Button backButton;
    
    public Color[] availableColors;
    public GameObject[] availableAccessories;
    public GameObject dogModel;

    private void Start()
    {
        LoadCurrentCustomization();
        SetupButtons();
    }

    private void LoadCurrentCustomization()
    {
        DogCustomization currentDog = GameManager.Instance.gameState.currentDog;
        
        if (nameInput != null)
        {
            nameInput.text = currentDog.dogName;
        }
        
        UpdateDogAppearance();
    }

    private void SetupButtons()
    {
        if (nextColorButton != null)
            nextColorButton.onClick.AddListener(() => ChangeColor(1));
        
        if (prevColorButton != null)
            prevColorButton.onClick.AddListener(() => ChangeColor(-1));
        
        if (nextAccessoryButton != null)
            nextAccessoryButton.onClick.AddListener(() => ChangeAccessory(1));
        
        if (prevAccessoryButton != null)
            prevAccessoryButton.onClick.AddListener(() => ChangeAccessory(-1));
        
        if (saveButton != null)
            saveButton.onClick.AddListener(SaveCustomization);
        
        if (backButton != null)
            backButton.onClick.AddListener(() => UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu"));
        
        if (nameInput != null)
            nameInput.onEndEdit.AddListener(UpdateDogName);
    }

    private void ChangeColor(int direction)
    {
        DogCustomization currentDog = GameManager.Instance.gameState.currentDog;
        currentDog.colorIndex = (currentDog.colorIndex + direction + availableColors.Length) % availableColors.Length;
        UpdateDogAppearance();
    }

    private void ChangeAccessory(int direction)
    {
        DogCustomization currentDog = GameManager.Instance.gameState.currentDog;
        currentDog.accessoryIndex = (currentDog.accessoryIndex + direction + availableAccessories.Length) % availableAccessories.Length;
        UpdateDogAppearance();
    }

    private void UpdateDogName(string newName)
    {
        if (!string.IsNullOrEmpty(newName))
        {
            GameManager.Instance.gameState.currentDog.dogName = newName;
        }
    }

    private void UpdateDogAppearance()
    {
        if (dogModel != null)
        {
            DogCustomization currentDog = GameManager.Instance.gameState.currentDog;
            
            // Update color
            Renderer dogRenderer = dogModel.GetComponent<Renderer>();
            if (dogRenderer != null && currentDog.colorIndex < availableColors.Length)
            {
                dogRenderer.material.color = availableColors[currentDog.colorIndex];
            }
            
            // Update accessory
            foreach (GameObject accessory in availableAccessories)
            {
                if (accessory != null)
                {
                    accessory.SetActive(false);
                }
            }
            
            if (currentDog.accessoryIndex < availableAccessories.Length)
            {
                availableAccessories[currentDog.accessoryIndex].SetActive(true);
            }
        }
    }

    private void SaveCustomization()
    {
        GameManager.Instance.SaveGame();
        UIManager.Instance.ShowMessage("Customization saved!");
    }
}
