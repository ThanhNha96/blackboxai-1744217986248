using UnityEngine;
using System.Collections;

public class DogStunAndSteal : MonoBehaviour
{
    [Header("Stun Settings")]
    public float stunDuration = 5f;
    public float stealRange = 2f;
    public float stealDuration = 1.5f;
    
    [Header("Animation Settings")]
    public float shakeIntensity = 0.2f;
    public float shakeFrequency = 20f;
    public float sparksDuration = 1f;
    
    [Header("Visual Effects")]
    public GameObject electricSparksPrefab;
    public GameObject stunStarsPrefab;
    public ParticleSystem captureEffect;
    public GameObject stunOverlayPrefab;
    
    [Header("Audio Effects")]
    public AudioClip taserSound;
    public AudioClip dogYelpSound;
    public AudioClip stunSound;
    public AudioClip captureSound;

    private Animator dogAnimator;
    private SpriteRenderer dogSprite;
    private bool isStunned;
    private GameObject currentStunEffect;
    private Vector3 originalPosition;

    void Start()
    {
        dogAnimator = GetComponent<Animator>();
        dogSprite = GetComponent<SpriteRenderer>();
        originalPosition = transform.position;
    }

    public void OnTaserHit(Vector2 hitPoint)
    {
        if (!isStunned)
        {
            StartCoroutine(StunSequence(hitPoint));
        }
    }

    IEnumerator StunSequence(Vector2 hitPoint)
    {
        isStunned = true;

        // Play taser hit animation
        PlayTaserHitEffects(hitPoint);
        
        // Initial shock reaction
        dogAnimator.SetTrigger("shocked");
        AudioManager.Instance.PlaySFX(dogYelpSound);
        
        // Shake animation
        StartCoroutine(ShakeAnimation());
        
        // Electric sparks effect
        GameObject sparks = Instantiate(electricSparksPrefab, hitPoint, Quaternion.identity);
        Destroy(sparks, sparksDuration);
        
        yield return new WaitForSeconds(sparksDuration);

        // Stunned state
        dogAnimator.SetBool("stunned", true);
        
        // Show stun stars
        currentStunEffect = Instantiate(stunStarsPrefab, 
            transform.position + Vector3.up * 0.5f, 
            Quaternion.identity, 
            transform);
        
        // Add stun overlay
        GameObject stunOverlay = Instantiate(stunOverlayPrefab, transform);
        stunOverlay.transform.localPosition = Vector3.zero;
        
        // Play stun sound
        AudioManager.Instance.PlaySFX(stunSound);
        
        // Wait for stun duration or until stolen
        float stunEndTime = Time.time + stunDuration;
        while (Time.time < stunEndTime && isStunned)
        {
            // Check if player is in range to steal
            if (IsPlayerInStealRange())
            {
                UIManager.Instance.ShowPrompt("PRESS_E_TO_STEAL");
                
                if (Input.GetKeyDown(KeyCode.E))
                {
                    StartCoroutine(StealSequence());
                    yield break;
                }
            }
            yield return null;
        }
        
        // Recovery if not stolen
        if (isStunned)
        {
            RecoverFromStun();
        }
    }

    IEnumerator ShakeAnimation()
    {
        float elapsed = 0f;
        
        while (elapsed < sparksDuration)
        {
            float offsetX = Mathf.Sin(elapsed * shakeFrequency) * shakeIntensity;
            float offsetY = Mathf.Cos(elapsed * shakeFrequency) * shakeIntensity;
            
            transform.position = originalPosition + new Vector3(offsetX, offsetY, 0);
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        transform.position = originalPosition;
    }

    void PlayTaserHitEffects(Vector2 hitPoint)
    {
        // Visual effect at hit point
        if (electricSparksPrefab != null)
        {
            GameObject hitEffect = Instantiate(electricSparksPrefab, hitPoint, Quaternion.identity);
            Destroy(hitEffect, 1f);
        }
        
        // Play taser sound
        AudioManager.Instance.PlaySFX(taserSound);
        
        // Flash sprite
        StartCoroutine(FlashSprite());
    }

    IEnumerator FlashSprite()
    {
        Color originalColor = dogSprite.color;
        dogSprite.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        dogSprite.color = originalColor;
    }

    bool IsPlayerInStealRange()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            float distance = Vector2.Distance(transform.position, player.transform.position);
            return distance <= stealRange;
        }
        return false;
    }

    IEnumerator StealSequence()
    {
        // Start capture animation
        dogAnimator.SetTrigger("capture");
        
        // Play capture effect
        if (captureEffect != null)
        {
            captureEffect.Play();
        }
        
        // Play capture sound
        AudioManager.Instance.PlaySFX(captureSound);
        
        // Animate dog being captured
        Vector3 startPos = transform.position;
        Vector3 endPos = transform.position + Vector3.up * 2f;
        float elapsed = 0f;
        
        while (elapsed < stealDuration)
        {
            float t = elapsed / stealDuration;
            
            // Parabolic arc movement
            Vector3 center = (startPos + endPos) * 0.5f;
            center.y += 1f;
            Vector3 startRelCenter = startPos - center;
            Vector3 endRelCenter = endPos - center;
            transform.position = center + Vector3.Slerp(startRelCenter, endRelCenter, t);
            
            // Fade out
            Color color = dogSprite.color;
            color.a = 1 - t;
            dogSprite.color = color;
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        // Add dog to player's inventory
        NPCDog npcDog = GetComponent<NPCDog>();
        if (npcDog != null)
        {
            GameManager.Instance.AddStolenDog(npcDog.GetDogData());
        }
        
        // Clean up effects
        if (currentStunEffect != null)
        {
            Destroy(currentStunEffect);
        }
        
        // Destroy the dog object
        Destroy(gameObject);
    }

    void RecoverFromStun()
    {
        isStunned = false;
        dogAnimator.SetBool("stunned", false);
        dogAnimator.SetTrigger("recover");
        
        if (currentStunEffect != null)
        {
            Destroy(currentStunEffect);
        }
        
        // Play recovery animation and sound
        AudioManager.Instance.PlaySFX("dogRecover");
    }

    void OnDestroy()
    {
        if (currentStunEffect != null)
        {
            Destroy(currentStunEffect);
        }
    }
}
