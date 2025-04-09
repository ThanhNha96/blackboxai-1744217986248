using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PoliceSystem : MonoBehaviour
{
    public static PoliceSystem Instance { get; private set; }

    [System.Serializable]
    public class PoliceUnit
    {
        public GameObject policeCar;
        public float speed = 15f;
        public float catchRange = 5f;
        public float searchRadius = 20f;
        public Transform spawnPoint;
    }

    [Header("Police Settings")]
    public PoliceUnit[] policeUnits;
    public float responseTime = 3f;
    public float searchDuration = 30f;
    public int maxActiveUnits = 3;
    public float wantedLevelDecayTime = 300f; // 5 minutes

    [Header("Prison Settings")]
    public Transform prisonLocation;
    public float prisonTime = 60f; // Base prison time in seconds
    public float bailCostMultiplier = 100f; // Cost per wanted level

    private List<GameObject> activePoliceUnits = new List<GameObject>();
    private float currentWantedLevel = 0f; // 0-5 stars
    private bool isPlayerWanted = false;
    private bool isPlayerInPrison = false;
    private float prisonReleaseTime;

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

    void Update()
    {
        if (isPlayerWanted)
        {
            UpdatePoliceSearch();
        }
        else if (currentWantedLevel > 0)
        {
            DecayWantedLevel();
        }

        if (isPlayerInPrison)
        {
            UpdatePrisonTime();
        }
    }

    public void ReportCrime(Vector3 location, float severity, NPCController reporter)
    {
        // Increase wanted level based on severity and reporter's trust level
        float trustMultiplier = (100f - reporter.GetTrustLevel()) / 100f;
        float wantedIncrease = severity * trustMultiplier;
        
        IncreaseWantedLevel(wantedIncrease);

        if (currentWantedLevel >= 1f && !isPlayerWanted)
        {
            StartPoliceSearch(location);
        }
    }

    void IncreaseWantedLevel(float amount)
    {
        currentWantedLevel = Mathf.Min(currentWantedLevel + amount, 5f);
        UIManager.Instance.UpdateWantedLevel(currentWantedLevel);
    }

    void DecayWantedLevel()
    {
        currentWantedLevel = Mathf.Max(0f, currentWantedLevel - (Time.deltaTime / wantedLevelDecayTime));
        UIManager.Instance.UpdateWantedLevel(currentWantedLevel);
    }

    void StartPoliceSearch(Vector3 location)
    {
        isPlayerWanted = true;
        StartCoroutine(SpawnPoliceUnits(location));
        UIManager.Instance.ShowMessage("POLICE_ALERTED");
    }

    IEnumerator SpawnPoliceUnits(Vector3 location)
    {
        yield return new WaitForSeconds(responseTime);

        int unitsToSpawn = Mathf.Min(Mathf.CeilToInt(currentWantedLevel), maxActiveUnits);
        
        for (int i = 0; i < unitsToSpawn; i++)
        {
            if (activePoliceUnits.Count < maxActiveUnits)
            {
                SpawnPoliceUnit(location);
                yield return new WaitForSeconds(1f);
            }
        }
    }

    void SpawnPoliceUnit(Vector3 targetLocation)
    {
        PoliceUnit unit = policeUnits[Random.Range(0, policeUnits.Length)];
        Vector3 spawnPos = unit.spawnPoint.position;
        
        GameObject policeUnit = Instantiate(unit.policeCar, spawnPos, Quaternion.identity);
        PoliceAI policeAI = policeUnit.GetComponent<PoliceAI>();
        
        if (policeAI != null)
        {
            policeAI.Initialize(unit.speed, unit.catchRange, unit.searchRadius, targetLocation);
        }
        
        activePoliceUnits.Add(policeUnit);
    }

    void UpdatePoliceSearch()
    {
        // Remove destroyed or inactive units
        activePoliceUnits.RemoveAll(unit => unit == null);

        if (activePoliceUnits.Count == 0 && currentWantedLevel < 1f)
        {
            isPlayerWanted = false;
            UIManager.Instance.ShowMessage("POLICE_LOST");
        }
    }

    public void ArrestPlayer()
    {
        if (!isPlayerInPrison)
        {
            isPlayerInPrison = true;
            float imprisonmentTime = prisonTime * currentWantedLevel;
            prisonReleaseTime = Time.time + imprisonmentTime;

            // Teleport player to prison
            GameManager.Instance.GetPlayer().transform.position = prisonLocation.position;
            
            // Confiscate stolen dogs
            GameManager.Instance.ConfiscateStolenDogs();
            
            // Show prison UI
            UIManager.Instance.ShowPrisonUI(imprisonmentTime, CalculateBailCost());
            
            // Clear police units
            ClearPoliceUnits();
        }
    }

    void UpdatePrisonTime()
    {
        if (Time.time >= prisonReleaseTime)
        {
            ReleasePlayer();
        }
        else
        {
            UIManager.Instance.UpdatePrisonTime(prisonReleaseTime - Time.time);
        }
    }

    public void PayBail()
    {
        int bailCost = CalculateBailCost();
        if (GameManager.Instance.SpendCoins(bailCost))
        {
            ReleasePlayer();
            UIManager.Instance.ShowMessage("BAIL_PAID");
        }
        else
        {
            UIManager.Instance.ShowMessage("NOT_ENOUGH_MONEY");
        }
    }

    void ReleasePlayer()
    {
        isPlayerInPrison = false;
        currentWantedLevel = 0f;
        UIManager.Instance.HidePrisonUI();
        UIManager.Instance.ShowMessage("RELEASED_FROM_PRISON");
        
        // Teleport player to release point
        GameManager.Instance.GetPlayer().transform.position = 
            prisonLocation.position + new Vector3(10f, 0f, 0f);
    }

    int CalculateBailCost()
    {
        return Mathf.RoundToInt(currentWantedLevel * bailCostMultiplier);
    }

    void ClearPoliceUnits()
    {
        foreach (GameObject unit in activePoliceUnits)
        {
            if (unit != null)
            {
                Destroy(unit);
            }
        }
        activePoliceUnits.Clear();
        isPlayerWanted = false;
    }

    public float GetWantedLevel()
    {
        return currentWantedLevel;
    }

    public bool IsPlayerWanted()
    {
        return isPlayerWanted;
    }

    public bool IsPlayerInPrison()
    {
        return isPlayerInPrison;
    }
}
