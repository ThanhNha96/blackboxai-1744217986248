using UnityEngine;

public class DynamicEventSystem : MonoBehaviour
{
    public float eventInterval = 60f; // Time interval for random events
    private float timeSinceLastEvent;

    void Update()
    {
        timeSinceLastEvent += Time.deltaTime;
        if (timeSinceLastEvent >= eventInterval)
        {
            TriggerRandomEvent();
            timeSinceLastEvent = 0f;
        }
    }

    void TriggerRandomEvent()
    {
        // Implement logic for random events (e.g., patrols, special missions)
        Debug.Log("A random event has occurred!");

        // Example: Spawn a patrol NPC
        SpawnPatrolNPC();
    }

    void SpawnPatrolNPC()
    {
        // Logic to spawn a patrol NPC at a random location
        Vector3 spawnPosition = new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f));
        GameObject patrolNPC = Instantiate(Resources.Load("Prefabs/NPCDog"), spawnPosition, Quaternion.identity) as GameObject;
        Debug.Log("Patrol NPC spawned at: " + spawnPosition);
    }
}
