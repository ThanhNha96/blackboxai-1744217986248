using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public GameObject platformPrefab;
    public GameObject collectiblePrefab;
    public GameObject levelEndPrefab;
    public int platformCount = 10;
    public float minPlatformY = 2f;
    public float maxPlatformY = 4f;
    public float minPlatformX = 2f;
    public float maxPlatformX = 4f;
    public float collectibleSpawnChance = 0.5f;

    void Start()
    {
        GenerateLevel();
    }

    void GenerateLevel()
    {
        Vector3 currentPosition = Vector3.zero;

        // Create starting platform
        CreatePlatform(currentPosition, 3f); // Wider starting platform

        for (int i = 0; i < platformCount; i++)
        {
            // Calculate next platform position
            float nextX = Random.Range(minPlatformX, maxPlatformX);
            float nextY = Random.Range(-minPlatformY, maxPlatformY);
            currentPosition += new Vector3(nextX, nextY, 0);

            // Create platform
            GameObject platform = CreatePlatform(currentPosition);

            // Possibly spawn collectible above platform
            if (Random.value < collectibleSpawnChance)
            {
                Vector3 collectiblePos = currentPosition + Vector3.up * 1.5f;
                CreateCollectible(collectiblePos);
            }
        }

        // Create end platform and goal
        Vector3 endPosition = currentPosition + new Vector3(3f, 2f, 0);
        CreatePlatform(endPosition, 3f); // Wider end platform
        
        // Place level end trigger
        Vector3 goalPosition = endPosition + new Vector3(0f, 2f, 0f);
        CreateLevelEnd(goalPosition);
    }

    GameObject CreatePlatform(Vector3 position, float scaleMultiplier = 1f)
    {
        GameObject platform = Instantiate(platformPrefab, position, Quaternion.identity);
        Vector3 currentScale = platform.transform.localScale;
        platform.transform.localScale = new Vector3(currentScale.x * scaleMultiplier, currentScale.y, currentScale.z);
        return platform;
    }

    void CreateCollectible(Vector3 position)
    {
        Instantiate(collectiblePrefab, position, Quaternion.identity);
    }

    void CreateLevelEnd(Vector3 position)
    {
        Instantiate(levelEndPrefab, position, Quaternion.identity);
    }
}
