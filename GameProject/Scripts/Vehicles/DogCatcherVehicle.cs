using UnityEngine;
using System.Collections;

public class DogCatcherVehicle : VehicleController
{
    [Header("Dog Catching Equipment")]
    public float catchRange = 5f;
    public float catchSpeed = 2f;
    public float netCooldown = 3f;
    public Transform netLaunchPoint;
    public GameObject netPrefab;
    public ParticleSystem catchEffect;
    
    [Header("Stealth System")]
    public float stealthLevel = 0f; // 0-100
    public float detectionRadius = 10f;
    public float noiseLevel = 50f; // 0-100
    
    [Header("Storage")]
    public int maxCaughtDogs = 3;
    public Transform storageArea;
    
    private bool canCatch = true;
    private int caughtDogCount = 0;
    private float lastCatchTime;

    [System.Serializable]
    public class CatcherUpgrades
    {
        public int netLevel = 1;
        public int storageLevel = 1;
        public int stealthLevel = 1;
        public int engineLevel = 1;
    }

    public CatcherUpgrades upgrades = new CatcherUpgrades();

    protected override void Start()
    {
        base.Start();
        UpdateStatsFromUpgrades();
    }

    void UpdateStatsFromUpgrades()
    {
        // Net upgrades
        catchRange = 5f + (upgrades.netLevel * 1f);
        catchSpeed = 2f + (upgrades.netLevel * 0.5f);
        netCooldown = 3f - (upgrades.netLevel * 0.2f);

        // Storage upgrades
        maxCaughtDogs = 3 + (upgrades.storageLevel * 2);

        // Stealth upgrades
        stealthLevel = upgrades.stealthLevel * 20f;
        noiseLevel = 50f - (upgrades.stealthLevel * 5f);

        // Engine upgrades
        maxSpeed += upgrades.engineLevel * 2f;
        acceleration += upgrades.engineLevel * 1f;
    }

    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.Space) && canCatch)
        {
            AttemptCatch();
        }

        // Update stealth status
        UpdateStealthStatus();
    }

    void UpdateStealthStatus()
    {
        // Check for nearby dogs that might detect the vehicle
        Collider2D[] nearbyDogs = Physics2D.OverlapCircleAll(transform.position, detectionRadius, LayerMask.GetMask("Dogs"));
        
        foreach (Collider2D dogCollider in nearbyDogs)
        {
            NPCDog npcDog = dogCollider.GetComponent<NPCDog>();
            if (npcDog != null)
            {
                float distance = Vector2.Distance(transform.position, dogCollider.transform.position);
                float detectionChance = (noiseLevel / 100f) * (1f - (distance / detectionRadius));
                
                if (Random.value < detectionChance)
                {
                    npcDog.OnDetectDogCatcher();
                }
            }
        }
    }

    void AttemptCatch()
    {
        if (Time.time - lastCatchTime < netCooldown) return;
        if (caughtDogCount >= maxCaughtDogs)
        {
            UIManager.Instance.ShowMessage("STORAGE_FULL");
            return;
        }

        StartCoroutine(CatchSequence());
    }

    IEnumerator CatchSequence()
    {
        canCatch = false;
        lastCatchTime = Time.time;

        // Spawn and launch net
        if (netPrefab != null && netLaunchPoint != null)
        {
            GameObject net = Instantiate(netPrefab, netLaunchPoint.position, netLaunchPoint.rotation);
            Rigidbody2D netRb = net.GetComponent<Rigidbody2D>();
            
            if (netRb != null)
            {
                netRb.velocity = netLaunchPoint.right * catchSpeed;
            }

            // Check for dogs in catch range
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(netLaunchPoint.position, catchRange);
            
            foreach (Collider2D hitCollider in hitColliders)
            {
                NPCDog npcDog = hitCollider.GetComponent<NPCDog>();
                if (npcDog != null && !npcDog.IsProtected())
                {
                    StartCoroutine(CatchDog(npcDog, net.transform));
                }
            }

            // Destroy net after delay
            Destroy(net, 2f);
        }

        yield return new WaitForSeconds(netCooldown);
        canCatch = true;
    }

    IEnumerator CatchDog(NPCDog dog, Transform net)
    {
        // Play catch effect
        if (catchEffect != null)
        {
            catchEffect.Play();
        }

        // Animate dog being caught
        float catchTime = 1f;
        Vector3 startPos = dog.transform.position;
        Vector3 endPos = storageArea.position;

        float elapsed = 0f;
        while (elapsed < catchTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / catchTime;
            
            // Arc motion
            Vector3 center = (startPos + endPos) * 0.5f;
            center.y += 2f;
            
            Vector3 startRelCenter = startPos - center;
            Vector3 endRelCenter = endPos - center;
            
            dog.transform.position = center + Vector3.Slerp(startRelCenter, endRelCenter, t);
            
            yield return null;
        }

        // Add dog to storage
        caughtDogCount++;
        GameManager.Instance.AddCaughtDog(dog.GetDogData());
        
        // Remove dog from scene
        Destroy(dog.gameObject);

        // Show feedback
        UIManager.Instance.ShowMessage("DOG_CAUGHT");
    }

    public void UpgradeNet()
    {
        if (upgrades.netLevel < 5)
        {
            int cost = 1000 * (upgrades.netLevel + 1);
            if (GameManager.Instance.SpendCoins(cost))
            {
                upgrades.netLevel++;
                UpdateStatsFromUpgrades();
                UIManager.Instance.ShowMessage("NET_UPGRADED");
            }
        }
    }

    public void UpgradeStorage()
    {
        if (upgrades.storageLevel < 5)
        {
            int cost = 1500 * (upgrades.storageLevel + 1);
            if (GameManager.Instance.SpendCoins(cost))
            {
                upgrades.storageLevel++;
                UpdateStatsFromUpgrades();
                UIManager.Instance.ShowMessage("STORAGE_UPGRADED");
            }
        }
    }

    public void UpgradeStealth()
    {
        if (upgrades.stealthLevel < 5)
        {
            int cost = 2000 * (upgrades.stealthLevel + 1);
            if (GameManager.Instance.SpendCoins(cost))
            {
                upgrades.stealthLevel++;
                UpdateStatsFromUpgrades();
                UIManager.Instance.ShowMessage("STEALTH_UPGRADED");
            }
        }
    }

    public void UpgradeEngine()
    {
        if (upgrades.engineLevel < 5)
        {
            int cost = 1200 * (upgrades.engineLevel + 1);
            if (GameManager.Instance.SpendCoins(cost))
            {
                upgrades.engineLevel++;
                UpdateStatsFromUpgrades();
                UIManager.Instance.ShowMessage("ENGINE_UPGRADED");
            }
        }
    }
}
