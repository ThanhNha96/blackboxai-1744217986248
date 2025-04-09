using UnityEngine;

public class NPCController : MonoBehaviour
{
    public float alertDistance = 10f; // Distance at which NPCs can detect the player
    public Transform player; // Reference to the player
    private bool isAlerted = false;

    void Update()
    {
        if (!isAlerted)
        {
            DetectPlayer();
        }
    }

    void DetectPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer < alertDistance)
        {
            AlertPlayer();
        }
    }

    public void AlertPlayer()
    {
        isAlerted = true;
        // Implement alert behavior (e.g., chase the player, call for help)
        Debug.Log("NPC has detected the player!");

        // Example of chasing the player
        StartCoroutine(ChasePlayer());
    }

    private IEnumerator ChasePlayer()
    {
        while (isAlerted)
        {
            // Move towards the player
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * Time.deltaTime * 3f; // Speed of 3 units per second

            // Check if the player is still in range
            if (Vector3.Distance(transform.position, player.position) > alertDistance)
            {
                isAlerted = false; // Stop chasing if the player is out of range
                Debug.Log("NPC lost sight of the player.");
            }

            yield return null; // Wait for the next frame
        }
    }
}
