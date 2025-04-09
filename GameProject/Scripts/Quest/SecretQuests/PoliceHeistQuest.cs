using UnityEngine;
using System.Collections;

public class PoliceHeistQuest : MonoBehaviour
{
    [System.Serializable]
    public class HeistStage
    {
        public string stageName;
        public string descriptionKey;
        public float difficulty; // 1-5
        public int reward;
        public bool isCompleted;
    }

    [Header("Quest Stages")]
    public HeistStage[] stages = new HeistStage[]
    {
        new HeistStage {
            stageName = "STAGE_OBSERVE",
            descriptionKey = "QUEST_OBSERVE_POLICE",
            difficulty = 1,
            reward = 500
        },
        new HeistStage {
            stageName = "STAGE_KEYCARD",
            descriptionKey = "QUEST_STEAL_KEYCARD",
            difficulty = 2,
            reward = 1000
        },
        new HeistStage {
            stageName = "STAGE_INFILTRATE",
            descriptionKey = "QUEST_INFILTRATE_STATION",
            difficulty = 3,
            reward = 1500
        },
        new HeistStage {
            stageName = "STAGE_ARMORY",
            descriptionKey = "QUEST_FIND_ARMORY",
            difficulty = 4,
            reward = 2000
        },
        new HeistStage {
            stageName = "STAGE_TASER",
            descriptionKey = "QUEST_STEAL_TASER",
            difficulty = 5,
            reward = 5000
        }
    };

    [Header("Stealth Settings")]
    public float detectionRadius = 5f;
    public float suspicionRate = 1f;
    public float maxSuspicion = 100f;
    public float suspicionDecayRate = 0.5f;

    [Header("Equipment")]
    public GameObject taserPrefab;
    public float taserDamage = 50f;
    public float taserRange = 5f;
    public float taserCooldown = 3f;
    public float stunDuration = 2f;

    private int currentStage = -1;
    private float currentSuspicion = 0f;
    private bool isQuestActive = false;
    private bool hasTaser = false;
    private float lastTaserTime;

    void Start()
    {
        // Quest is discovered by observing police station for some time
        StartCoroutine(CheckForQuestDiscovery());
    }

    IEnumerator CheckForQuestDiscovery()
    {
        while (!isQuestActive)
        {
            // Check if player is observing police station
            if (IsPlayerObservingPoliceStation())
            {
                currentSuspicion += suspicionRate * Time.deltaTime;
                
                if (currentSuspicion >= maxSuspicion)
                {
                    DiscoverQuest();
                    break;
                }
            }
            else
            {
                currentSuspicion = Mathf.Max(0, currentSuspicion - suspicionDecayRate * Time.deltaTime);
            }
            
            yield return null;
        }
    }

    bool IsPlayerObservingPoliceStation()
    {
        // Check if player is near and looking at police station
        PoliceSystem policeStation = FindObjectOfType<PoliceSystem>();
        if (policeStation != null)
        {
            Vector3 playerPos = GameManager.Instance.GetPlayer().transform.position;
            float distance = Vector3.Distance(playerPos, policeStation.transform.position);
            
            if (distance <= detectionRadius * 2)
            {
                // Check if player is looking at station
                Vector3 toStation = (policeStation.transform.position - playerPos).normalized;
                float dot = Vector3.Dot(GameManager.Instance.GetPlayer().transform.forward, toStation);
                return dot > 0.7f; // Looking within ~45 degrees
            }
        }
        return false;
    }

    void DiscoverQuest()
    {
        isQuestActive = true;
        currentStage = 0;
        UIManager.Instance.ShowMessage("QUEST_HEIST_DISCOVERED");
        UpdateQuestMarker();
    }

    public void AdvanceStage()
    {
        if (currentStage >= 0 && currentStage < stages.Length)
        {
            stages[currentStage].isCompleted = true;
            GameManager.Instance.AddCoins(stages[currentStage].reward);
            
            currentStage++;
            if (currentStage < stages.Length)
            {
                UIManager.Instance.ShowMessage("QUEST_STAGE_COMPLETE");
                UpdateQuestMarker();
            }
            else
            {
                CompleteHeist();
            }
        }
    }

    void CompleteHeist()
    {
        // Give player the taser
        hasTaser = true;
        UIManager.Instance.ShowMessage("QUEST_HEIST_COMPLETE");
        
        // Add taser to player's inventory
        GameManager.Instance.AddSpecialItem("taser");
    }

    void UpdateQuestMarker()
    {
        if (currentStage >= 0 && currentStage < stages.Length)
        {
            // Update quest marker on map
            MapSystem.Instance.UpdateQuestMarker(
                "heist_quest",
                GetCurrentStageLocation(),
                stages[currentStage].descriptionKey
            );
        }
    }

    Vector3 GetCurrentStageLocation()
    {
        PoliceSystem policeStation = FindObjectOfType<PoliceSystem>();
        if (policeStation != null)
        {
            switch (currentStage)
            {
                case 0: // Observe
                    return policeStation.transform.position + Vector3.right * 10f;
                case 1: // Keycard
                    return policeStation.transform.position + Vector3.right * 5f;
                case 2: // Infiltrate
                    return policeStation.transform.position;
                case 3: // Find Armory
                    return policeStation.transform.position + Vector3.up * 2f;
                case 4: // Steal Taser
                    return policeStation.transform.position + Vector3.up * 2f + Vector3.right * 2f;
            }
        }
        return Vector3.zero;
    }

    public void UseTaser()
    {
        if (!hasTaser || Time.time - lastTaserTime < taserCooldown)
            return;

        // Raycast to find target
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            transform.right,
            taserRange,
            LayerMask.GetMask("NPC", "Police")
        );

        if (hit.collider != null)
        {
            // Apply taser effect
            if (hit.collider.CompareTag("Police"))
            {
                PoliceAI police = hit.collider.GetComponent<PoliceAI>();
                if (police != null)
                {
                    StartCoroutine(StunTarget(police.gameObject));
                    PoliceSystem.Instance.ReportCrime(transform.position, 1f, null);
                }
            }
            else if (hit.collider.CompareTag("NPC"))
            {
                NPCController npc = hit.collider.GetComponent<NPCController>();
                if (npc != null)
                {
                    StartCoroutine(StunTarget(npc.gameObject));
                    npc.GetComponent<NPCTrustSystem>().DecreaseTrust(npc.npcId, 50f);
                }
            }

            // Play effects
            PlayTaserEffects(hit.point);
        }

        lastTaserTime = Time.time;
    }

    IEnumerator StunTarget(GameObject target)
    {
        // Disable target's movement and actions
        MonoBehaviour[] components = target.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour component in components)
        {
            if (component != this)
            {
                component.enabled = false;
            }
        }

        // Play stun effect
        ParticleSystem stunEffect = target.GetComponent<ParticleSystem>();
        if (stunEffect != null)
        {
            stunEffect.Play();
        }

        yield return new WaitForSeconds(stunDuration);

        // Re-enable components
        foreach (MonoBehaviour component in components)
        {
            if (component != this)
            {
                component.enabled = true;
            }
        }
    }

    void PlayTaserEffects(Vector2 hitPoint)
    {
        // Spawn taser effect
        if (taserPrefab != null)
        {
            GameObject effect = Instantiate(taserPrefab, hitPoint, Quaternion.identity);
            Destroy(effect, 1f);
        }

        // Play sound
        AudioManager.Instance.PlaySFX("taser");
    }

    public bool IsStageCompleted(int stage)
    {
        return stage >= 0 && stage < stages.Length && stages[stage].isCompleted;
    }

    public string GetCurrentStageDescription()
    {
        if (currentStage >= 0 && currentStage < stages.Length)
        {
            return LocalizationManager.Instance.GetLocalizedText(stages[currentStage].descriptionKey);
        }
        return "";
    }

    public bool HasTaser()
    {
        return hasTaser;
    }
}
