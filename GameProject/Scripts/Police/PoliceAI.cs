using UnityEngine;

public class PoliceAI : MonoBehaviour
{
    public Transform target; // The player to chase
    public float chaseSpeed = 5f; // Speed of the police car
    public float stoppingDistance = 2f; // Distance to stop chasing
    private bool isChasing = false;

    void Update()
    {
        if (isChasing && target != null)
        {
            ChasePlayer();
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        isChasing = true;
    }

    void ChasePlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, target.position);
        if (distanceToPlayer > stoppingDistance)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * chaseSpeed * Time.deltaTime;
            transform.LookAt(target); // Rotate to face the player
        }
        else
        {
            // Logic for catching the player (e.g., stop the race, trigger a cutscene)
            CatchPlayer();
        }
    }

    void CatchPlayer()
    {
        isChasing = false;
        Debug.Log("Police have caught the player!");
        // Implement logic for what happens when the player is caught
    }
}
