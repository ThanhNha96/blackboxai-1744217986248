using UnityEngine;
using System.Collections.Generic;

public class DogTrader : MonoBehaviour
{
    [System.Serializable]
    public class DogForSale
    {
        public string id;
        public string nameKey;
        public string breedKey;
        public int price;
        public DogRarity rarity;
        public DogSkill[] skills;
        public Sprite preview;
        public bool isAvailable = true;
    }

    [System.Serializable]
    public class DogSkill
    {
        public string nameKey;
        public SkillType type;
        public int level;
    }

    public enum DogRarity
    {
        Common,     // 60% chance
        Uncommon,   // 25% chance
        Rare,       // 10% chance
        Legendary   // 5% chance
    }

    public enum SkillType
    {
        Speed,
        Strength,
        Agility,
        Intelligence,
        Stealth,
        Charm
    }

    [Header("Shop Settings")]
    public float restockTime = 3600f; // 1 hour in seconds
    public int maxDogsInShop = 5;
    public Transform displayArea;

    [Header("Available Dogs")]
    public DogForSale[] availableDogs = new DogForSale[]
    {
        // Common Dogs
        new DogForSale { 
            id = "dog_labrador", 
            nameKey = "DOG_LABRADOR", 
            breedKey = "BREED_LABRADOR",
            price = 2000,
            rarity = DogRarity.Common,
            skills = new DogSkill[] {
                new DogSkill { nameKey = "SKILL_SPEED", type = SkillType.Speed, level = 3 },
                new DogSkill { nameKey = "SKILL_STRENGTH", type = SkillType.Strength, level = 4 }
            }
        },
        
        // Uncommon Dogs
        new DogForSale {
            id = "dog_husky",
            nameKey = "DOG_HUSKY",
            breedKey = "BREED_HUSKY",
            price = 5000,
            rarity = DogRarity.Uncommon,
            skills = new DogSkill[] {
                new DogSkill { nameKey = "SKILL_SPEED", type = SkillType.Speed, level = 5 },
                new DogSkill { nameKey = "SKILL_AGILITY", type = SkillType.Agility, level = 4 }
            }
        },
        
        // Rare Dogs
        new DogForSale {
            id = "dog_german_shepherd",
            nameKey = "DOG_GERMAN_SHEPHERD",
            breedKey = "BREED_GERMAN_SHEPHERD",
            price = 10000,
            rarity = DogRarity.Rare,
            skills = new DogSkill[] {
                new DogSkill { nameKey = "SKILL_INTELLIGENCE", type = SkillType.Intelligence, level = 5 },
                new DogSkill { nameKey = "SKILL_STRENGTH", type = SkillType.Strength, level = 5 }
            }
        },
        
        // Legendary Dogs
        new DogForSale {
            id = "dog_golden_special",
            nameKey = "DOG_GOLDEN_SPECIAL",
            breedKey = "BREED_GOLDEN_SPECIAL",
            price = 25000,
            rarity = DogRarity.Legendary,
            skills = new DogSkill[] {
                new DogSkill { nameKey = "SKILL_CHARM", type = SkillType.Charm, level = 5 },
                new DogSkill { nameKey = "SKILL_INTELLIGENCE", type = SkillType.Intelligence, level = 5 },
                new DogSkill { nameKey = "SKILL_AGILITY", type = SkillType.Agility, level = 5 }
            }
        }
    };

    private List<DogForSale> currentStock = new List<DogForSale>();
    private float nextRestockTime;

    void Start()
    {
        RestockShop();
    }

    void Update()
    {
        if (Time.time >= nextRestockTime)
        {
            RestockShop();
        }
    }

    void RestockShop()
    {
        currentStock.Clear();
        
        // Add random dogs based on rarity
        while (currentStock.Count < maxDogsInShop)
        {
            float random = Random.value;
            DogRarity rarity;
            
            if (random < 0.05f) rarity = DogRarity.Legendary;
            else if (random < 0.15f) rarity = DogRarity.Rare;
            else if (random < 0.40f) rarity = DogRarity.Uncommon;
            else rarity = DogRarity.Common;

            // Find available dog of selected rarity
            DogForSale[] dogsOfRarity = System.Array.FindAll(availableDogs, 
                dog => dog.rarity == rarity && dog.isAvailable);

            if (dogsOfRarity.Length > 0)
            {
                DogForSale selectedDog = dogsOfRarity[Random.Range(0, dogsOfRarity.Length)];
                currentStock.Add(selectedDog);
            }
        }

        nextRestockTime = Time.time + restockTime;
        UpdateShopDisplay();
    }

    void UpdateShopDisplay()
    {
        // Clear existing display
        foreach (Transform child in displayArea)
        {
            Destroy(child.gameObject);
        }

        // Create display for each dog
        foreach (DogForSale dog in currentStock)
        {
            CreateDogDisplay(dog);
        }
    }

    void CreateDogDisplay(DogForSale dog)
    {
        // Create UI elements for dog display
        GameObject displayObject = new GameObject(dog.id + "_display");
        displayObject.transform.SetParent(displayArea);
        
        // Add preview image
        if (dog.preview != null)
        {
            SpriteRenderer preview = displayObject.AddComponent<SpriteRenderer>();
            preview.sprite = dog.preview;
        }

        // Add interaction trigger
        BoxCollider2D trigger = displayObject.AddComponent<BoxCollider2D>();
        trigger.isTrigger = true;
    }

    public void ShowDogInfo(DogForSale dog)
    {
        string name = LocalizationManager.Instance.GetLocalizedText(dog.nameKey);
        string breed = LocalizationManager.Instance.GetLocalizedText(dog.breedKey);
        
        string skillsText = "";
        foreach (DogSkill skill in dog.skills)
        {
            string skillName = LocalizationManager.Instance.GetLocalizedText(skill.nameKey);
            skillsText += $"\n- {skillName} Lv.{skill.level}";
        }

        string info = $"{name}\n{breed}\n{LocalizationManager.Instance.GetLocalizedText("PRICE")}: {dog.price}{skillsText}";
        
        UIManager.Instance.ShowDogInfo(info, () => PurchaseDog(dog), null);
    }

    public bool PurchaseDog(DogForSale dog)
    {
        if (GameManager.Instance.SpendCoins(dog.price))
        {
            dog.isAvailable = false;
            currentStock.Remove(dog);
            GameManager.Instance.AddDogToCollection(dog.id);
            UpdateShopDisplay();
            UIManager.Instance.ShowMessage("DOG_PURCHASED");
            return true;
        }
        else
        {
            UIManager.Instance.ShowMessage("NOT_ENOUGH_MONEY");
            return false;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            UIManager.Instance.ShowMessage("PRESS_E_TO_SHOP");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            UIManager.Instance.HideMessage();
        }
    }
}
