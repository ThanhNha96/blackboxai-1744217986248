using UnityEngine;
using System.Collections;

public class MouseCombatSystem : MonoBehaviour
{
    [Header("Combat Settings")]
    public float punchDamage = 10f;
    public float kickDamage = 15f;
    public float pushForce = 500f;
    public float comboTimeWindow = 0.5f;
    public int maxComboCount = 3;
    public float attackRange = 1.5f;
    public float pushRange = 1.0f;

    [Header("Combat Timing")]
    public float punchCooldown = 0.3f;
    public float kickCooldown = 0.4f;
    public float pushCooldown = 0.5f;

    [Header("Combat Effects")]
    public ParticleSystem punchEffect;
    public ParticleSystem kickEffect;
    public ParticleSystem pushEffect;
    public AudioClip punchSound;
    public AudioClip kickSound;
    public AudioClip pushSound;

    private Animator animator;
    private AudioSource audioSource;
    private int currentCombo;
    private float lastAttackTime;
    private float lastPunchTime;
    private float lastKickTime;
    private float lastPushTime;
    private bool isAttacking;
    private Rigidbody2D rb;

    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Reset combo if too much time has passed
        if (Time.time - lastAttackTime > comboTimeWindow)
        {
            currentCombo = 0;
        }

        // Check for mouse inputs
        if (Input.GetMouseButtonDown(0) && CanPunch()) // Left click for punch
        {
            PerformPunch();
        }
        else if (Input.GetMouseButtonDown(1) && CanKick()) // Right click for kick
        {
            PerformKick();
        }

        // Check for nearby targets to push
        CheckForPushTargets();
    }

    bool CanPunch()
    {
        return !isAttacking && Time.time - lastPunchTime >= punchCooldown;
    }

    bool CanKick()
    {
        return !isAttacking && Time.time - lastKickTime >= kickCooldown;
    }

    void PerformPunch()
    {
        StartCoroutine(PunchSequence());
    }

    void PerformKick()
    {
        StartCoroutine(KickSequence());
    }

    IEnumerator PunchSequence()
    {
        isAttacking = true;
        lastPunchTime = Time.time;
        lastAttackTime = Time.time;

        // Determine combo animation
        string punchAnim = currentCombo switch
        {
            0 => "punch1",
            1 => "punch2",
            _ => "punch3"
        };

        // Play animation
        animator.SetTrigger(punchAnim);

        // Play sound
        if (punchSound != null)
        {
            audioSource.PlayOneShot(punchSound);
        }

        // Check for hits
        RaycastHit2D[] hits = Physics2D.RaycastAll(
            transform.position,
            transform.right,
            attackRange,
            LayerMask.GetMask("NPC", "Police", "Dog")
        );

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null)
            {
                // Apply damage and effects
                ApplyPunchDamage(hit.collider.gameObject);
                
                // Spawn hit effect
                if (punchEffect != null)
                {
                    Instantiate(punchEffect, hit.point, Quaternion.identity);
                }
            }
        }

        // Update combo
        currentCombo = (currentCombo + 1) % maxComboCount;

        // Wait for animation
        yield return new WaitForSeconds(punchCooldown);
        isAttacking = false;
    }

    IEnumerator KickSequence()
    {
        isAttacking = true;
        lastKickTime = Time.time;
        lastAttackTime = Time.time;

        // Determine combo animation
        string kickAnim = currentCombo switch
        {
            0 => "kick1",
            1 => "kick2",
            _ => "kick3"
        };

        // Play animation
        animator.SetTrigger(kickAnim);

        // Play sound
        if (kickSound != null)
        {
            audioSource.PlayOneShot(kickSound);
        }

        // Check for hits
        RaycastHit2D[] hits = Physics2D.RaycastAll(
            transform.position,
            transform.right,
            attackRange,
            LayerMask.GetMask("NPC", "Police", "Dog")
        );

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null)
            {
                // Apply damage and effects
                ApplyKickDamage(hit.collider.gameObject);
                
                // Spawn hit effect
                if (kickEffect != null)
                {
                    Instantiate(kickEffect, hit.point, Quaternion.identity);
                }
            }
        }

        // Update combo
        currentCombo = (currentCombo + 1) % maxComboCount;

        // Wait for animation
        yield return new WaitForSeconds(kickCooldown);
        isAttacking = false;
    }

    void CheckForPushTargets()
    {
        if (Time.time - lastPushTime < pushCooldown)
            return;

        Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(
            transform.position,
            pushRange,
            LayerMask.GetMask("NPC", "Police", "Dog")
        );

        foreach (Collider2D collider in nearbyColliders)
        {
            if (collider != null)
            {
                // Show push prompt
                UIManager.Instance.ShowPrompt("PRESS_SPACE_TO_PUSH");

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    PushTarget(collider.gameObject);
                    break;
                }
            }
        }
    }

    void PushTarget(GameObject target)
    {
        lastPushTime = Time.time;

        // Play push animation
        animator.SetTrigger("push");

        // Play sound
        if (pushSound != null)
        {
            audioSource.PlayOneShot(pushSound);
        }

        // Apply push force
        Rigidbody2D targetRb = target.GetComponent<Rigidbody2D>();
        if (targetRb != null)
        {
            Vector2 pushDirection = (target.transform.position - transform.position).normalized;
            targetRb.AddForce(pushDirection * pushForce);
        }

        // Spawn effect
        if (pushEffect != null)
        {
            Instantiate(pushEffect, target.transform.position, Quaternion.identity);
        }

        // Handle specific target types
        HandlePushTarget(target);
    }

    void HandlePushTarget(GameObject target)
    {
        if (target.CompareTag("Police"))
        {
            PoliceAI police = target.GetComponent<PoliceAI>();
            if (police != null)
            {
                StartCoroutine(police.GetStunned(1f));
                PoliceSystem.Instance.ReportCrime(transform.position, 0.5f, null);
            }
        }
        else if (target.CompareTag("NPC"))
        {
            NPCController npc = target.GetComponent<NPCController>();
            if (npc != null)
            {
                npc.GetComponent<NPCTrustSystem>().DecreaseTrust(npc.npcId, 20f);
            }
        }
        else if (target.CompareTag("Dog"))
        {
            NPCDog dog = target.GetComponent<NPCDog>();
            if (dog != null)
            {
                StartCoroutine(dog.GetStunned(1f));
            }
        }
    }

    void ApplyPunchDamage(GameObject target)
    {
        float damage = punchDamage * (1 + currentCombo * 0.5f);
        
        if (target.CompareTag("Police"))
        {
            PoliceSystem.Instance.ReportCrime(transform.position, 1f, null);
        }
        else if (target.CompareTag("NPC"))
        {
            NPCController npc = target.GetComponent<NPCController>();
            if (npc != null)
            {
                npc.GetComponent<NPCTrustSystem>().DecreaseTrust(npc.npcId, 30f);
            }
        }

        // Apply damage to target's health system if it exists
        HealthSystem health = target.GetComponent<HealthSystem>();
        if (health != null)
        {
            health.TakeDamage(damage);
        }
    }

    void ApplyKickDamage(GameObject target)
    {
        float damage = kickDamage * (1 + currentCombo * 0.5f);
        
        if (target.CompareTag("Police"))
        {
            PoliceSystem.Instance.ReportCrime(transform.position, 1.5f, null);
        }
        else if (target.CompareTag("NPC"))
        {
            NPCController npc = target.GetComponent<NPCController>();
            if (npc != null)
            {
                npc.GetComponent<NPCTrustSystem>().DecreaseTrust(npc.npcId, 40f);
            }
        }

        // Apply damage to target's health system if it exists
        HealthSystem health = target.GetComponent<HealthSystem>();
        if (health != null)
        {
            health.TakeDamage(damage);
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Draw push range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pushRange);
    }
}
