using UnityEngine;

public class CombatAnimator : MonoBehaviour
{
    [System.Serializable]
    public class CombatAnimationClip
    {
        public string name;
        public AnimationClip animation;
        public float transitionDuration = 0.1f;
        public ParticleSystem effect;
        public AudioClip sound;
    }

    [Header("Combat Animations")]
    public CombatAnimationClip[] punchAnimations = new CombatAnimationClip[3]; // Light, Medium, Heavy punch
    public CombatAnimationClip[] kickAnimations = new CombatAnimationClip[3];  // Light, Medium, Heavy kick
    public CombatAnimationClip tackleAnimation;
    public CombatAnimationClip dodgeAnimation;
    public CombatAnimationClip stunAnimation;
    public CombatAnimationClip knockdownAnimation;
    public CombatAnimationClip getUpAnimation;

    [Header("Hit Reactions")]
    public CombatAnimationClip lightHitReaction;
    public CombatAnimationClip heavyHitReaction;
    public CombatAnimationClip blockAnimation;

    [Header("Combat States")]
    public float comboTimeWindow = 0.5f;
    public float maxComboCount = 3;

    private Animator animator;
    private AudioSource audioSource;
    private int currentCombo = 0;
    private float lastAttackTime;
    private bool isStunned;
    private bool isKnockedDown;

    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        SetupAnimatorParameters();
    }

    void SetupAnimatorParameters()
    {
        // Combat state parameters
        animator.SetBool("InCombat", false);
        animator.SetBool("IsStunned", false);
        animator.SetBool("IsKnockedDown", false);
        animator.SetInteger("ComboCount", 0);
    }

    public void PlayPunchAnimation(int comboLevel)
    {
        if (CanPerformAction() && comboLevel < punchAnimations.Length)
        {
            PlayCombatAnimation(punchAnimations[comboLevel]);
            UpdateCombo();
        }
    }

    public void PlayKickAnimation(int comboLevel)
    {
        if (CanPerformAction() && comboLevel < kickAnimations.Length)
        {
            PlayCombatAnimation(kickAnimations[comboLevel]);
            UpdateCombo();
        }
    }

    public void PlayTackleAnimation()
    {
        if (CanPerformAction())
        {
            PlayCombatAnimation(tackleAnimation);
            ResetCombo();
        }
    }

    public void PlayDodgeAnimation()
    {
        if (CanPerformAction())
        {
            PlayCombatAnimation(dodgeAnimation);
            ResetCombo();
        }
    }

    public void PlayHitReaction(bool isHeavyHit)
    {
        CombatAnimationClip hitReaction = isHeavyHit ? heavyHitReaction : lightHitReaction;
        PlayCombatAnimation(hitReaction);
        ResetCombo();
    }

    public void PlayBlockAnimation()
    {
        if (CanPerformAction())
        {
            PlayCombatAnimation(blockAnimation);
        }
    }

    public void PlayStunAnimation()
    {
        isStunned = true;
        animator.SetBool("IsStunned", true);
        PlayCombatAnimation(stunAnimation);
        ResetCombo();
    }

    public void PlayKnockdownAnimation()
    {
        isKnockedDown = true;
        animator.SetBool("IsKnockedDown", true);
        PlayCombatAnimation(knockdownAnimation);
        ResetCombo();
    }

    public void PlayGetUpAnimation()
    {
        if (isKnockedDown)
        {
            PlayCombatAnimation(getUpAnimation);
            isKnockedDown = false;
            animator.SetBool("IsKnockedDown", false);
        }
    }

    void PlayCombatAnimation(CombatAnimationClip animClip)
    {
        // Play animation
        animator.CrossFade(animClip.name, animClip.transitionDuration);

        // Play effect
        if (animClip.effect != null)
        {
            ParticleSystem effect = Instantiate(animClip.effect, transform.position, Quaternion.identity);
            effect.Play();
            Destroy(effect.gameObject, effect.main.duration);
        }

        // Play sound
        if (animClip.sound != null && audioSource != null)
        {
            audioSource.PlayOneShot(animClip.sound);
        }
    }

    void UpdateCombo()
    {
        if (Time.time - lastAttackTime <= comboTimeWindow)
        {
            currentCombo = Mathf.Min(currentCombo + 1, (int)maxComboCount);
        }
        else
        {
            currentCombo = 0;
        }

        animator.SetInteger("ComboCount", currentCombo);
        lastAttackTime = Time.time;
    }

    void ResetCombo()
    {
        currentCombo = 0;
        animator.SetInteger("ComboCount", 0);
        lastAttackTime = 0f;
    }

    bool CanPerformAction()
    {
        return !isStunned && !isKnockedDown && !animator.IsInTransition(0);
    }

    public void EndStun()
    {
        isStunned = false;
        animator.SetBool("IsStunned", false);
    }

    public void SetCombatState(bool inCombat)
    {
        animator.SetBool("InCombat", inCombat);
        if (!inCombat)
        {
            ResetCombo();
        }
    }

    public bool IsInAction()
    {
        return animator.IsInTransition(0) || isStunned || isKnockedDown;
    }
}
