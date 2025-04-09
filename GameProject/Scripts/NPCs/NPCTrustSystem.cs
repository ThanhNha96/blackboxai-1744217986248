using UnityEngine;
using System.Collections.Generic;

public class NPCTrustSystem : MonoBehaviour
{
    [System.Serializable]
    public class TrustData
    {
        public float trustLevel = 50f; // 0-100
        public float suspicion = 0f; // 0-100
        public List<string> completedFavors = new List<string>();
        public bool hasReportedCrime = false;
    }

    [Header("Trust Settings")]
    public float baseDetectionRadius = 10f;
    public float trustDecayRate = 0.1f; // Per minute
    public float suspicionIncreaseRate = 5f;
    public float trustIncreaseFromFavor = 10f;
    public float trustDecreaseFromCrime = 20f;

    [Header("Favor System")]
    public string[] availableFavors = {
        "FAVOR_WALK_DOG",
        "FAVOR_FIND_TOY",
        "FAVOR_DELIVER_FOOD",
        "FAVOR_GARDEN_HELP",
        "FAVOR_HOME_REPAIR"
    };

    private Dictionary<string, TrustData> npcTrustLevels = new Dictionary<string, TrustData>();
    private string currentFavor;
    private bool isFavorActive;

    void Start()
    {
        InvokeRepeating("UpdateTrustLevels", 60f, 60f); // Update trust every minute
    }

    void UpdateTrustLevels()
    {
        foreach (var trustData in npcTrustLevels.Values)
        {
            // Natural trust decay over time
            trustData.trustLevel = Mathf.Max(0f, trustData.trustLevel - trustDecayRate);
            
            // Decrease suspicion over time
            trustData.suspicion = Mathf.Max(0f, trustData.suspicion - (trustDecayRate * 2f));
        }
    }

    public void InitializeNPCTrust(string npcId)
    {
        if (!npcTrustLevels.ContainsKey(npcId))
        {
            npcTrustLevels[npcId] = new TrustData();
        }
    }

    public float GetTrustLevel(string npcId)
    {
        if (npcTrustLevels.ContainsKey(npcId))
        {
            return npcTrustLevels[npcId].trustLevel;
        }
        return 50f; // Default trust level
    }

    public void IncreaseSuspicion(string npcId, float amount)
    {
        if (npcTrustLevels.ContainsKey(npcId))
        {
            TrustData data = npcTrustLevels[npcId];
            data.suspicion = Mathf.Min(100f, data.suspicion + amount);
            
            // If suspicion is high and trust is low, NPC might report to police
            if (data.suspicion >= 75f && data.trustLevel <= 30f && !data.hasReportedCrime)
            {
                ReportToPolice(npcId);
            }
        }
    }

    void ReportToPolice(string npcId)
    {
        TrustData data = npcTrustLevels[npcId];
        data.hasReportedCrime = true;
        
        NPCController npc = FindNPCById(npcId);
        if (npc != null)
        {
            float severity = (100f - data.trustLevel) / 20f; // Convert trust to 0-5 wanted level
            PoliceSystem.Instance.ReportCrime(npc.transform.position, severity, npc);
        }
    }

    public void OfferFavor(string npcId)
    {
        if (!isFavorActive && npcTrustLevels.ContainsKey(npcId))
        {
            TrustData data = npcTrustLevels[npcId];
            
            // Select a random favor that hasn't been completed
            List<string> availableFavorsList = new List<string>(availableFavors);
            availableFavorsList.RemoveAll(f => data.completedFavors.Contains(f));
            
            if (availableFavorsList.Count > 0)
            {
                currentFavor = availableFavorsList[Random.Range(0, availableFavorsList.Count)];
                isFavorActive = true;
                
                // Show favor dialog
                string favorText = LocalizationManager.Instance.GetLocalizedText(currentFavor);
                UIManager.Instance.ShowFavorDialog(favorText, AcceptFavor, DeclineFavor);
            }
        }
    }

    void AcceptFavor()
    {
        if (isFavorActive)
        {
            // Add favor objective to quest system
            QuestManager.Instance.AddFavorQuest(currentFavor);
            UIManager.Instance.ShowMessage("FAVOR_ACCEPTED");
        }
    }

    void DeclineFavor()
    {
        if (isFavorActive)
        {
            // Decrease trust slightly for declining
            DecreaseTrust(GetNPCIdForFavor(), 5f);
            isFavorActive = false;
            currentFavor = null;
            UIManager.Instance.ShowMessage("FAVOR_DECLINED");
        }
    }

    public void CompleteFavor(string npcId, string favorId)
    {
        if (npcTrustLevels.ContainsKey(npcId))
        {
            TrustData data = npcTrustLevels[npcId];
            
            // Increase trust and add to completed favors
            IncreaseTrust(npcId, trustIncreaseFromFavor);
            data.completedFavors.Add(favorId);
            
            // Clear active favor
            isFavorActive = false;
            currentFavor = null;
            
            UIManager.Instance.ShowMessage("FAVOR_COMPLETED");
        }
    }

    public void IncreaseTrust(string npcId, float amount)
    {
        if (npcTrustLevels.ContainsKey(npcId))
        {
            TrustData data = npcTrustLevels[npcId];
            data.trustLevel = Mathf.Min(100f, data.trustLevel + amount);
            data.suspicion = Mathf.Max(0f, data.suspicion - (amount / 2f));
        }
    }

    public void DecreaseTrust(string npcId, float amount)
    {
        if (npcTrustLevels.ContainsKey(npcId))
        {
            TrustData data = npcTrustLevels[npcId];
            data.trustLevel = Mathf.Max(0f, data.trustLevel - amount);
            data.suspicion = Mathf.Min(100f, data.suspicion + (amount / 2f));
        }
    }

    public void OnDogStolen(string npcId)
    {
        if (npcTrustLevels.ContainsKey(npcId))
        {
            DecreaseTrust(npcId, trustDecreaseFromCrime);
            IncreaseSuspicion(npcId, suspicionIncreaseRate * 2f);
        }
    }

    public float GetDetectionRadius(string npcId)
    {
        if (npcTrustLevels.ContainsKey(npcId))
        {
            TrustData data = npcTrustLevels[npcId];
            // Detection radius increases with suspicion
            return baseDetectionRadius * (1f + (data.suspicion / 100f));
        }
        return baseDetectionRadius;
    }

    private NPCController FindNPCById(string npcId)
    {
        NPCController[] npcs = FindObjectsOfType<NPCController>();
        return System.Array.Find(npcs, npc => npc.npcId == npcId);
    }

    private string GetNPCIdForFavor()
    {
        // Find the NPC that offered the current favor
        NPCController[] npcs = FindObjectsOfType<NPCController>();
        NPCController npc = System.Array.Find(npcs, n => n.currentFavor == currentFavor);
        return npc != null ? npc.npcId : null;
    }
}
