using UnityEngine;
using System.Collections.Generic;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    [System.Serializable]
    public class QuestReward
    {
        public int coins;
        public List<ShopItem> items;
        public int experiencePoints;
    }

    [System.Serializable]
    public class Quest
    {
        public string questId;
        public string titleKey;
        public string descriptionKey;
        public QuestType type;
        public int difficulty; // 1-5, affects rewards
        public QuestReward reward;
        public bool isCompleted;
        public bool isActive;
        
        // Quest requirements
        public int targetAmount; // Number of items to collect, distance to travel, etc.
        public float timeLimit; // For time-based quests
        public string targetItem; // Item to collect/deliver
        public Vector2 targetLocation; // Location to reach
        public string targetNPCId; // NPC to interact with
        
        // Quest progress
        public int currentAmount;
        public float currentTime;
    }

    public enum QuestType
    {
        Collect,        // Collect specific items
        Delivery,       // Deliver items to NPCs
        Race,          // Complete a race within time limit
        Exploration,   // Find specific locations
        Training,      // Complete training exercises
        Transport,     // Transport NPCs to locations
        Tricks,        // Perform specific tricks
        Social         // Interact with multiple NPCs
    }

    private Dictionary<string, Quest> allQuests;
    private List<Quest> activeQuests;
    private List<Quest> completedQuests;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeQuests();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializeQuests()
    {
        allQuests = new Dictionary<string, Quest>();
        activeQuests = new List<Quest>();
        completedQuests = new List<Quest>();

        // Initialize beginner quests
        CreateBeginnerQuests();
        // Initialize intermediate quests
        CreateIntermediateQuests();
        // Initialize advanced quests
        CreateAdvancedQuests();
    }

    void CreateBeginnerQuests()
    {
        // Simple collection quest
        CreateQuest(new Quest
        {
            questId = "QUEST_COLLECT_BONES",
            titleKey = "QUEST_TITLE_COLLECT_BONES",
            descriptionKey = "QUEST_DESC_COLLECT_BONES",
            type = QuestType.Collect,
            difficulty = 1,
            targetAmount = 5,
            reward = new QuestReward { coins = 100, experiencePoints = 50 }
        });

        // Basic delivery quest
        CreateQuest(new Quest
        {
            questId = "QUEST_DELIVER_MAIL",
            titleKey = "QUEST_TITLE_DELIVER_MAIL",
            descriptionKey = "QUEST_DESC_DELIVER_MAIL",
            type = QuestType.Delivery,
            difficulty = 1,
            targetAmount = 3,
            reward = new QuestReward { coins = 150, experiencePoints = 75 }
        });

        // Simple exploration quest
        CreateQuest(new Quest
        {
            questId = "QUEST_EXPLORE_PARK",
            titleKey = "QUEST_TITLE_EXPLORE_PARK",
            descriptionKey = "QUEST_DESC_EXPLORE_PARK",
            type = QuestType.Exploration,
            difficulty = 1,
            targetAmount = 3,
            reward = new QuestReward { coins = 120, experiencePoints = 60 }
        });
    }

    void CreateIntermediateQuests()
    {
        // Racing quest
        CreateQuest(new Quest
        {
            questId = "QUEST_RACE_TIME",
            titleKey = "QUEST_TITLE_RACE_TIME",
            descriptionKey = "QUEST_DESC_RACE_TIME",
            type = QuestType.Race,
            difficulty = 3,
            timeLimit = 120f,
            reward = new QuestReward { coins = 300, experiencePoints = 150 }
        });

        // Transport quest
        CreateQuest(new Quest
        {
            questId = "QUEST_TRANSPORT_VIP",
            titleKey = "QUEST_TITLE_TRANSPORT_VIP",
            descriptionKey = "QUEST_DESC_TRANSPORT_VIP",
            type = QuestType.Transport,
            difficulty = 3,
            targetAmount = 1,
            reward = new QuestReward { coins = 400, experiencePoints = 200 }
        });

        // Tricks training quest
        CreateQuest(new Quest
        {
            questId = "QUEST_LEARN_TRICKS",
            titleKey = "QUEST_TITLE_LEARN_TRICKS",
            descriptionKey = "QUEST_DESC_LEARN_TRICKS",
            type = QuestType.Tricks,
            difficulty = 3,
            targetAmount = 5,
            reward = new QuestReward { coins = 350, experiencePoints = 175 }
        });
    }

    void CreateAdvancedQuests()
    {
        // Complex delivery chain
        CreateQuest(new Quest
        {
            questId = "QUEST_DELIVERY_CHAIN",
            titleKey = "QUEST_TITLE_DELIVERY_CHAIN",
            descriptionKey = "QUEST_DESC_DELIVERY_CHAIN",
            type = QuestType.Delivery,
            difficulty = 5,
            targetAmount = 10,
            reward = new QuestReward { coins = 1000, experiencePoints = 500 }
        });

        // Championship race
        CreateQuest(new Quest
        {
            questId = "QUEST_CHAMPIONSHIP",
            titleKey = "QUEST_TITLE_CHAMPIONSHIP",
            descriptionKey = "QUEST_DESC_CHAMPIONSHIP",
            type = QuestType.Race,
            difficulty = 5,
            timeLimit = 300f,
            reward = new QuestReward { coins = 1500, experiencePoints = 750 }
        });

        // City-wide social quest
        CreateQuest(new Quest
        {
            questId = "QUEST_SOCIAL_NETWORK",
            titleKey = "QUEST_TITLE_SOCIAL_NETWORK",
            descriptionKey = "QUEST_DESC_SOCIAL_NETWORK",
            type = QuestType.Social,
            difficulty = 5,
            targetAmount = 15,
            reward = new QuestReward { coins = 1200, experiencePoints = 600 }
        });
    }

    void CreateQuest(Quest quest)
    {
        allQuests.Add(quest.questId, quest);
    }

    public void ActivateQuest(string questId)
    {
        if (allQuests.ContainsKey(questId) && !allQuests[questId].isActive && !allQuests[questId].isCompleted)
        {
            Quest quest = allQuests[questId];
            quest.isActive = true;
            activeQuests.Add(quest);
            UIManager.Instance.ShowMessage("QUEST_STARTED");
        }
    }

    public void UpdateQuestProgress(string questId, int progress)
    {
        if (allQuests.ContainsKey(questId) && allQuests[questId].isActive)
        {
            Quest quest = allQuests[questId];
            quest.currentAmount += progress;
            
            if (quest.currentAmount >= quest.targetAmount)
            {
                CompleteQuest(questId);
            }
            else
            {
                UIManager.Instance.UpdateQuestProgress(quest);
            }
        }
    }

    public void CompleteQuest(string questId)
    {
        if (allQuests.ContainsKey(questId))
        {
            Quest quest = allQuests[questId];
            quest.isCompleted = true;
            quest.isActive = false;
            activeQuests.Remove(quest);
            completedQuests.Add(quest);
            
            // Award rewards
            GameManager.Instance.AddCoins(quest.reward.coins);
            GameManager.Instance.AddExperience(quest.reward.experiencePoints);
            
            if (quest.reward.items != null)
            {
                foreach (ShopItem item in quest.reward.items)
                {
                    // Add items to inventory
                    GameManager.Instance.AddInventoryItem(item);
                }
            }
            
            UIManager.Instance.ShowQuestComplete(quest);
            AudioManager.Instance.PlayUISound("complete");
        }
    }

    public List<Quest> GetAvailableQuests()
    {
        List<Quest> available = new List<Quest>();
        foreach (var quest in allQuests.Values)
        {
            if (!quest.isActive && !quest.isCompleted)
            {
                available.Add(quest);
            }
        }
        return available;
    }

    public List<Quest> GetActiveQuests()
    {
        return activeQuests;
    }

    public List<Quest> GetCompletedQuests()
    {
        return completedQuests;
    }
}
