using UnityEngine;
using System.Collections;

public class CombatSystem : MonoBehaviour
{
    [System.Serializable]
    public class CombatAnimation
    {
        public string name;
        public float damage;
        public float duration;
        public float cooldown;
        public AudioClip soundEffect;
        public ParticleSystem hitEffect;
    }

    [Header("Combat Stats")]
    public float health = 100f;
    public float stamina = 100f;
    public float staminaRegenRate = 10f;
    public float defenseMultiplier = 1f;
    public float stunDuration = 1f;

    [Header("Combat Animations")]
    public CombatAnimation[] combatMoves = new CombatAnimation[]
    {
        new CombatAnimation { 
            name = "punch",
            damage = 10f,
            duration = 0.5f,
            cooldown = 0.7f
        },
        new CombatAnimation {
            name = "kick",
            damage = 15f,
            duration = 0.6f,
            cooldown = 1f
        },
        new CombatAnimation {
            name = "tackle",
            damage = 20f,
            duration = 0.8f,
            cooldown = 1.5f
        },
        new CombatAnimation {
            name = "dodge",
            damage = 0f,
            duration = 0.3f,
            cooldown = 0.5f
        }
    };

    [Header("Effects")]
    public ParticleSystem stunEffect;
    public AudioClip hitSound;
    public AudioClip dodgeSound;

    private Animator animator;
    private AudioSource audioSource;
    private bool isInCombat;
    private bool isStunned;
    private float[] moveCooldowns;
    private Rigidbody2D rb;

    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        moveCooldowns = new float[combatMoves.Length];
    }

    void Update()
    {
        if (isStunned) return;

        // Update cooldowns
        for (int i = 0; i < moveCooldowns.Length; i++)
        {
            if (moveCooldowns[i] > 0)
            {
                moveCooldowns[i] -= Time.deltaTime;
            }
        }

        // Regenerate stamina
        stamina = Mathf.Min(100f, stamina + staminaRegenRate * Time.deltaTime);

        // Combat inputs
        if (isInCombat)
        {
            if (Input.GetKeyDown(KeyCode.J) && CanPerformMove(0)) // Punch
            {
                PerformCombatMove(0);
            }
            else if (Input.GetKeyDown(KeyCode.K) && CanPerformMove(1)) // Kick
            {
                PerformCombatMove(1);
            }
            else if (Input.GetKeyDown(KeyCode.L) && CanPerformMove(2)) // Tackle
            {
                PerformCombatMove(2);
            }
            else if (Input.GetKeyDown(KeyCode.Space) && CanPerformMove(3)) // Dodge
            {
                PerformCombatMove(3);
            }
        }
    }

    bool CanPerformMove(int moveIndex)
    {
        return moveCooldowns[moveIndex] <= 0 && stamina >= 10f;
    }

    void PerformCombatMove(int moveIndex)
    {
        CombatAnimation move = combatMoves[moveIndex];
        
        // Start animation
        animator.SetTrigger(move.name);
        
        // Apply cooldown
        moveCooldowns[moveIndex] = move.cooldown;
        
        // Use stamina
        stamina -= 10f;
        
        // Play sound effect
        if (move.soundEffect != null)
        {
            audioSource.PlayOneShot(move.soundEffect);
        }
        
        // Check for hits
        StartCoroutine(CheckHitsDuringAnimation(move));
    }

    IEnumerator CheckHitsDuringAnimation(CombatAnimation move)
    {
        float elapsed = 0f;
        while (elapsed < move.duration)
        {
            // Check for hits in front of the character
            Vector2 direction = transform.right;
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction, 1.5f);
            
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider != null)
                {
                    // Check if hit police or NPC
                    PoliceAI police = hit.collider.GetComponent<PoliceAI>();
                    NPCController npc = hit.collider.GetComponent<NPCController>();
                    
                    if (police != null)
                    {
                        ApplyDamageToPolice(police, move);
                    }
                    else if (npc != null)
                    {
                        ApplyDamageToNPC(npc, move);
                    }
                }
            }
            
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    void ApplyDamageToPolice(PoliceAI police, CombatAnimation move)
    {
        // Increase wanted level significantly
        PoliceSystem.Instance.ReportCrime(transform.position, 1f, null);
        
        // Play hit effect
        if (move.hitEffect != null)
        {
            Instantiate(move.hitEffect, police.transform.position, Quaternion.identity);
        }
        
        // Stun police briefly
        StartCoroutine(StunTarget(police.gameObject));
    }

    void ApplyDamageToNPC(NPCController npc, CombatAnimation move)
    {
        // Decrease trust dramatically
        npc.GetComponent<NPCTrustSystem>().DecreaseTrust(npc.npcId, 30f);
        
        // Play hit effect
        if (move.hitEffect != null)
        {
            Instantiate(move.hitEffect, npc.transform.position, Quaternion.identity);
        }
        
        // Stun NPC
        StartCoroutine(StunTarget(npc.gameObject));
        
        // NPC might call police
        if (Random.value < 0.5f)
        {
            PoliceSystem.Instance.ReportCrime(transform.position, 0.5f, npc);
        }
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
        if (stunEffect != null)
        {
            ParticleSystem stun = Instantiate(stunEffect, target.transform.position + Vector3.up, Quaternion.identity);
            stun.transform.parent = target.transform;
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

    public void StartCombat()
    {
        isInCombat = true;
        UIManager.Instance.ShowCombatUI();
    }

    public void EndCombat()
    {
        isInCombat = false;
        UIManager.Instance.HideCombatUI();
    }

    public void TakeDamage(float damage)
    {
        health -= damage * defenseMultiplier;
        
        if (health <= 0)
        {
            Die();
        }
        else
        {
            // Play hit animation
            animator.SetTrigger("hit");
            
            // Play hit sound
            if (hitSound != null)
            {
                audioSource.PlayOneShot(hitSound);
            }
        }
        
        UIManager.Instance.UpdateHealthUI(health);
    }

    void Die()
    {
        // Play death animation
        animator.SetTrigger("die");
        
        // Disable controls
        enabled = false;
        
        // Game over or respawn logic
        GameManager.Instance.OnPlayerDeath();
    }

    public void Dodge()
    {
        // Temporary invincibility
        StartCoroutine(DodgeRoutine());
    }

    IEnumerator DodgeRoutine()
    {
        // Play dodge animation
        animator.SetTrigger("dodge");
        
        // Play dodge sound
        if (dodgeSound != null)
        {
            audioSource.PlayOneShot(dodgeSound);
        }
        
        // Make temporarily invincible
        defenseMultiplier = 0f;
        
        yield return new WaitForSeconds(0.5f);
        
        // Restore normal defense
        defenseMultiplier = 1f;
    }
}
