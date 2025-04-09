using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CollectionSceneController : MonoBehaviour
{
    public GameObject dogCardPrefab;
    public Transform contentContainer;
    public Button backButton;
    public Text collectionCountText;

    private List<GameObject> spawnedCards = new List<GameObject>();

    private void Start()
    {
        SetupButtons();
        LoadCollection();
        UpdateCollectionCount();
    }

    private void SetupButtons()
    {
        if (backButton != null)
        {
            backButton.onClick.AddListener(() => UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu"));
        }
    }

    private void LoadCollection()
    {
        ClearExistingCards();

        bool[] unlockedDogs = GameManager.Instance.gameState.unlockedDogs;
        
        for (int i = 0; i < unlockedDogs.Length; i++)
        {
            GameObject cardObject = Instantiate(dogCardPrefab, contentContainer);
            spawnedCards.Add(cardObject);

            // Setup card UI
            DogCardUI cardUI = cardObject.GetComponent<DogCardUI>();
            if (cardUI != null)
            {
                if (unlockedDogs[i])
                {
                    cardUI.SetupUnlockedDog(i);
                }
                else
                {
                    cardUI.SetupLockedDog();
                }
            }
        }
    }

    private void ClearExistingCards()
    {
        foreach (GameObject card in spawnedCards)
        {
            if (card != null)
            {
                Destroy(card);
            }
        }
        spawnedCards.Clear();
    }

    private void UpdateCollectionCount()
    {
        if (collectionCountText != null)
        {
            int unlockedCount = 0;
            foreach (bool unlocked in GameManager.Instance.gameState.unlockedDogs)
            {
                if (unlocked) unlockedCount++;
            }
            
            collectionCountText.text = $"Collection: {unlockedCount}/{GameManager.Instance.gameState.unlockedDogs.Length}";
        }
    }
}

// Helper component for dog collection cards
public class DogCardUI : MonoBehaviour
{
    public Image dogImage;
    public Text dogNumberText;
    public GameObject lockedOverlay;
    public GameObject unlockedOverlay;

    public void SetupUnlockedDog(int dogNumber)
    {
        if (dogNumberText != null)
            dogNumberText.text = $"Dog #{dogNumber + 1}";
        
        if (lockedOverlay != null)
            lockedOverlay.SetActive(false);
        
        if (unlockedOverlay != null)
            unlockedOverlay.SetActive(true);
        
        // Here you would load the specific dog sprite/model
        // dogImage.sprite = Resources.Load<Sprite>($"DogSprites/Dog_{dogNumber}");
    }

    public void SetupLockedDog()
    {
        if (dogNumberText != null)
            dogNumberText.text = "???";
        
        if (lockedOverlay != null)
            lockedOverlay.SetActive(true);
        
        if (unlockedOverlay != null)
            unlockedOverlay.SetActive(false);
        
        if (dogImage != null)
            dogImage.color = Color.black; // Silhouette effect
    }
}
