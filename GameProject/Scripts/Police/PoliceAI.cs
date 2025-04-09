using UnityEngine;
using System.Collections;

public class PoliceAI : MonoBehaviour
{
    [Header("Police Unit Settings")]
    public float speed;
    public float catchRange;
    public float searchRadius;
    public float rotationSpeed = 180f;
    public float sirensVolume = 0.7f;
    
    [Header("Effects")]
    public GameObject sirenLights;
    public AudioClip sirenSound;
    public ParticleSystem exhaustEffect;
    
    private Vector3 targetLocation;
    private Transform playerTransform;
    private bool hasSpottedPlayer;
    private AudioSource audioSource;
    private float searchStartTime;
    private Vector3[] patrolPoints;
    private int currentPatrolIndex;
    
    private enum PoliceState
    {
        Searching,
        Pursuing,
        Returning
    }
    
    private PoliceState currentState;

    public void Initialize(float _speed, float _catchRange, float _searchRadius, Vector3 _targetLocation)
    {
        speed = _speed;
        catchRange = _catchRange;
        searchRadius = _searchRadius;
        targetLocation = _targetLocation;
        
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        audioSource = GetComponent<AudioSource>();
        
        // Setup audio
        if (audioSource && sirenSound)
        {
            audioSource.clip = sirenSound;
            audioSource.loop = true;
            audioSource.volume = sirensVolume;
            audioSource.Play();
        }
        
        // Generate patrol points around target location
        GeneratePatrolPoints();
        
        currentState = PoliceState.Searching;
        searchStartTime = Time.time;
        
        StartCoroutine(PoliceRoutine());
    }

    void GeneratePatrolPoints()
    {
        patrolPoints = new Vector3[4];
        for (int i = 0; i < 4; i++)
        {
            float angle = i * 90f * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * searchRadius;
            patrolPoints[i] = targetLocation + offset;
        }
    }

    IEnumerator PoliceRoutine()
    {
        while (true)
        {
            switch (currentState)
            {
                case PoliceState.Searching:
                    SearchForPlayer();
                    break;
                    
                case PoliceState.Pursuing:
                    PursuePlayer();
                    break;
                    
                case PoliceState.Returning:
                    ReturnToPatrol();
                    break;
            }
            
            yield return null;
        }
    }

    void SearchForPlayer()
    {
        // Move between patrol points
        Vector3 targetPoint = patrolPoints[currentPatrolIndex];
        MoveTowards(targetPoint);
        
        // Check if reached current patrol point
        if (Vector3.Distance(transform.position, targetPoint) < 0.5f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }
        
        // Check for player in sight
        CheckForPlayer();
        
        // Check if search time expired
        if (Time.time - searchStartTime > PoliceSystem.Instance.searchDuration)
        {
            currentState = PoliceState.Returning;
        }
    }

    void PursuePlayer()
    {
        if (!PoliceSystem.Instance.IsPlayerInPrison())
        {
            // Direct pursuit of player
            MoveTowards(playerTransform.position);
            
            // Check if in range to catch
            if (Vector3.Distance(transform.position, playerTransform.position) < catchRange)
            {
                CatchPlayer();
            }
            
            // Check if lost sight of player
            if (!CheckForPlayer())
            {
                hasSpottedPlayer = false;
                currentState = PoliceState.Searching;
                searchStartTime = Time.time;
            }
        }
        else
        {
            currentState = PoliceState.Returning;
        }
    }

    void ReturnToPatrol()
    {
        // Return to station or despawn
        Vector3 returnPoint = transform.position + Vector3.right * 20f;
        MoveTowards(returnPoint);
        
        if (Vector3.Distance(transform.position, returnPoint) < 0.5f)
        {
            Destroy(gameObject);
        }
    }

    void MoveTowards(Vector3 target)
    {
        // Calculate direction
        Vector3 direction = (target - transform.position).normalized;
        
        // Rotate towards target
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float currentAngle = transform.eulerAngles.z;
        float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);
        transform.eulerAngles = new Vector3(0, 0, newAngle);
        
        // Move forward
        transform.position += direction * speed * Time.deltaTime;
        
        // Update effects
        if (exhaustEffect != null)
        {
            exhaustEffect.emissionRate = speed;
        }
    }

    bool CheckForPlayer()
    {
        if (PoliceSystem.Instance.IsPlayerInPrison()) return false;
        
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer <= searchRadius)
        {
            // Check if there are obstacles between police and player
            RaycastHit2D hit = Physics2D.Raycast(
                transform.position,
                playerTransform.position - transform.position,
                distanceToPlayer,
                LayerMask.GetMask("Obstacles")
            );
            
            if (hit.collider == null)
            {
                hasSpottedPlayer = true;
                currentState = PoliceState.Pursuing;
                return true;
            }
        }
        return false;
    }

    void CatchPlayer()
    {
        // Play catch effect
        if (exhaustEffect != null)
        {
            exhaustEffect.Play();
        }
        
        // Arrest player
        PoliceSystem.Instance.ArrestPlayer();
        
        // Switch to returning state
        currentState = PoliceState.Returning;
    }

    void OnDrawGizmos()
    {
        // Draw detection ranges in editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, searchRadius);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, catchRange);
    }
}
