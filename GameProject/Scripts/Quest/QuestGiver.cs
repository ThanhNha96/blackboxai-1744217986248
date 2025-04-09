using UnityEngine;
using System.Collections.Generic;

public class QuestGiver : MonoBehaviour
{
    [System.Serializable]
    public class QuestDialogue
    {
        public string questId;
        public string introKey;
        public string acceptKey;
        public string declineKey;
        public string activeKey;
        public string completeKey;
        public string rewardKey;
    }

    public string npcId;
    public QuestDialogue[] availableQuests;
    public NPCType specialization;
    
    public enum NPCType
    {
        Trainer,        // Gives trick and training quests
        Mailman,        // Gives delivery quests
        RaceOrganizer,  // Gives racing quests
        Explorer,       // Gives exploration quests
        Socialite,      // Gives social quests
        Collector,      // Gives collection quests
        VIP            // Gives special high-reward quests
    }

    private Dictionary<string, QuestDialogue> questDialogues;

    void Start()
    {
        InitializeQuests();
    }

    void InitializeQuests()
    {
        questDialogues = new Dictionary<string, QuestDialogue>();
        foreach (QuestDialogue dialogue in availableQuests)
        {
            questDialogues.Add(dialogue.questId, dialogue);
        }

        // Add specialized quests based on NPC type
        AddSpecializedQuests();
    }

    void AddSpecializedQuests()
    {
        switch (specialization)
        {
            case NPCType.Trainer:
                AddTrainerQuests();
                break;
            case NPCType.Mailman:
                AddMailmanQuests();
                break;
            case NPCType.RaceOrganizer:
                AddRaceQuests();
                break;
            case NPCType.Explorer:
                AddExplorationQuests();
                break;
            case NPCType.Socialite:
                AddSocialQuests();
                break;
            case NPCType.Collector:
                AddCollectionQuests();
                break;
            case NPCType.VIP:
                AddVIPQuests();
                break;
        }
    }

    void AddTrainerQuests()
    {
        // Basic tricks
        AddQuestDialogue("QUEST_BASIC_TRICKS", 
            "DIALOGUE_TRICKS_INTRO",
            "DIALOGUE_TRICKS_ACCEPT",
            "DIALOGUE_TRICKS_DECLINE",
            "DIALOGUE_TRICKS_ACTIVE",
            "DIALOGUE_TRICKS_COMPLETE",
            "DIALOGUE_TRICKS_REWARD");

        // Advanced tricks
        AddQuestDialogue("QUEST_ADVANCED_TRICKS",
            "DIALOGUE_ADV_TRICKS_INTRO",
            "DIALOGUE_ADV_TRICKS_ACCEPT",
            "DIALOGUE_ADV_TRICKS_DECLINE",
            "DIALOGUE_ADV_TRICKS_ACTIVE",
            "DIALOGUE_ADV_TRICKS_COMPLETE",
            "DIALOGUE_ADV_TRICKS_REWARD");
    }

    void AddMailmanQuests()
    {
        // Simple delivery
        AddQuestDialogue("QUEST_DELIVER_MAIL",
            "DIALOGUE_MAIL_INTRO",
            "DIALOGUE_MAIL_ACCEPT",
            "DIALOGUE_MAIL_DECLINE",
            "DIALOGUE_MAIL_ACTIVE",
            "DIALOGUE_MAIL_COMPLETE",
            "DIALOGUE_MAIL_REWARD");

        // Express delivery
        AddQuestDialogue("QUEST_EXPRESS_DELIVERY",
            "DIALOGUE_EXPRESS_INTRO",
            "DIALOGUE_EXPRESS_ACCEPT",
            "DIALOGUE_EXPRESS_DECLINE",
            "DIALOGUE_EXPRESS_ACTIVE",
            "DIALOGUE_EXPRESS_COMPLETE",
            "DIALOGUE_EXPRESS_REWARD");
    }

    void AddRaceQuests()
    {
        // Time trial
        AddQuestDialogue("QUEST_TIME_TRIAL",
            "DIALOGUE_RACE_INTRO",
            "DIALOGUE_RACE_ACCEPT",
            "DIALOGUE_RACE_DECLINE",
            "DIALOGUE_RACE_ACTIVE",
            "DIALOGUE_RACE_COMPLETE",
            "DIALOGUE_RACE_REWARD");

        // Championship
        AddQuestDialogue("QUEST_CHAMPIONSHIP",
            "DIALOGUE_CHAMP_INTRO",
            "DIALOGUE_CHAMP_ACCEPT",
            "DIALOGUE_CHAMP_DECLINE",
            "DIALOGUE_CHAMP_ACTIVE",
            "DIALOGUE_CHAMP_COMPLETE",
            "DIALOGUE_CHAMP_REWARD");
    }

    void AddExplorationQuests()
    {
        // Find landmarks
        AddQuestDialogue("QUEST_FIND_LANDMARKS",
            "DIALOGUE_EXPLORE_INTRO",
            "DIALOGUE_EXPLORE_ACCEPT",
            "DIALOGUE_EXPLORE_DECLINE",
            "DIALOGUE_EXPLORE_ACTIVE",
            "DIALOGUE_EXPLORE_COMPLETE",
            "DIALOGUE_EXPLORE_REWARD");

        // Secret locations
        AddQuestDialogue("QUEST_SECRET_SPOTS",
            "DIALOGUE_SECRET_INTRO",
            "DIALOGUE_SECRET_ACCEPT",
            "DIALOGUE_SECRET_DECLINE",
            "DIALOGUE_SECRET_ACTIVE",
            "DIALOGUE_SECRET_COMPLETE",
            "DIALOGUE_SECRET_REWARD");
    }

    void AddSocialQuests()
    {
        // Meet neighbors
        AddQuestDialogue("QUEST_MEET_NEIGHBORS",
            "DIALOGUE_SOCIAL_INTRO",
            "DIALOGUE_SOCIAL_ACCEPT",
            "DIALOGUE_SOCIAL_DECLINE",
            "DIALOGUE_SOCIAL_ACTIVE",
            "DIALOGUE_SOCIAL_COMPLETE",
            "DIALOGUE_SOCIAL_REWARD");

        // Party organization
        AddQuestDialogue("QUEST_ORGANIZE_PARTY",
            "DIALOGUE_PARTY_INTRO",
            "DIALOGUE_PARTY_ACCEPT",
            "DIALOGUE_PARTY_DECLINE",
            "DIALOGUE_PARTY_ACTIVE",
            "DIALOGUE_PARTY_COMPLETE",
            "DIALOGUE_PARTY_REWARD");
    }

    void AddCollectionQuests()
    {
        // Collect bones
        AddQuestDialogue("QUEST_COLLECT_BONES",
            "DIALOGUE_BONES_INTRO",
            "DIALOGUE_BONES_ACCEPT",
            "DIALOGUE_BONES_DECLINE",
            "DIALOGUE_BONES_ACTIVE",
            "DIALOGUE_BONES_COMPLETE",
            "DIALOGUE_BONES_REWARD");

        // Rare items
        AddQuestDialogue("QUEST_RARE_ITEMS",
            "DIALOGUE_RARE_INTRO",
            "DIALOGUE_RARE_ACCEPT",
            "DIALOGUE_RARE_DECLINE",
            "DIALOGUE_RARE_ACTIVE",
            "DIALOGUE_RARE_COMPLETE",
            "DIALOGUE_RARE_REWARD");
    }

    void AddVIPQuests()
    {
        // Special delivery
        AddQuestDialogue("QUEST_VIP_DELIVERY",
            "DIALOGUE_VIP_INTRO",
            "DIALOGUE_VIP_ACCEPT",
            "DIALOGUE_VIP_DECLINE",
            "DIALOGUE_VIP_ACTIVE",
            "DIALOGUE_VIP_COMPLETE",
            "DIALOGUE_VIP_REWARD");

        // City tour
        AddQuestDialogue("QUEST_CITY_TOUR",
            "DIALOGUE_TOUR_INTRO",
            "DIALOGUE_TOUR_ACCEPT",
            "DIALOGUE_TOUR_DECLINE",
            "DIALOGUE_TOUR_ACTIVE",
            "DIALOGUE_TOUR_COMPLETE",
            "DIALOGUE_TOUR_REWARD");
    }

    void AddQuestDialogue(string questId, string intro, string accept, string decline, string active, string complete, string reward)
    {
        QuestDialogue dialogue = new QuestDialogue
        {
            questId = questId,
            introKey = intro,
            acceptKey = accept,
            declineKey = decline,
            activeKey = active,
            completeKey = complete,
            rewardKey = reward
        };
        
        if (!questDialogues.ContainsKey(questId))
        {
            questDialogues.Add(questId, dialogue);
        }
    }

    public void InteractWithPlayer()
    {
        // Check for completable quests first
        foreach (var quest in QuestManager.Instance.GetActiveQuests())
        {
            if (questDialogues.ContainsKey(quest.questId) && quest.currentAmount >= quest.targetAmount)
            {
                CompleteQuest(quest.questId);
                return;
            }
        }

        // Offer new quest if available
        OfferQuest();
    }

    void OfferQuest()
    {
        foreach (var questId in questDialogues.Keys)
        {
            if (!QuestManager.Instance.GetCompletedQuests().Exists(q => q.questId == questId) &&
                !QuestManager.Instance.GetActiveQuests().Exists(q => q.questId == questId))
            {
                ShowQuestDialog(questId);
                return;
            }
        }

        // No quests available
        UIManager.Instance.ShowMessage("NO_QUESTS_AVAILABLE");
    }

    void ShowQuestDialog(string questId)
    {
        QuestDialogue dialogue = questDialogues[questId];
        string intro = LocalizationManager.Instance.GetLocalizedText(dialogue.introKey);
        
        UIManager.Instance.ShowQuestDialog(
            intro,
            () => AcceptQuest(questId),
            () => DeclineQuest(questId)
        );
    }

    void AcceptQuest(string questId)
    {
        QuestManager.Instance.ActivateQuest(questId);
        string acceptText = LocalizationManager.Instance.GetLocalizedText(questDialogues[questId].acceptKey);
        UIManager.Instance.ShowMessage(acceptText);
    }

    void DeclineQuest(string questId)
    {
        string declineText = LocalizationManager.Instance.GetLocalizedText(questDialogues[questId].declineKey);
        UIManager.Instance.ShowMessage(declineText);
    }

    void CompleteQuest(string questId)
    {
        QuestDialogue dialogue = questDialogues[questId];
        string completeText = LocalizationManager.Instance.GetLocalizedText(dialogue.completeKey);
        string rewardText = LocalizationManager.Instance.GetLocalizedText(dialogue.rewardKey);
        
        UIManager.Instance.ShowQuestComplete(completeText, rewardText);
        QuestManager.Instance.CompleteQuest(questId);
    }
}
