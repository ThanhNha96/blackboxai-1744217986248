using UnityEngine;

public class Collectible : MonoBehaviour
{
    public float rotateSpeed = 100f;
    public float bobSpeed = 2f;
    public float bobHeight = 0.5f;
    
    private Vector3 startPosition;
    
    void Start()
    {
        startPosition = transform.position;
    }
    
    void Update()
    {
        // Rotate the collectible
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
        
        // Bob up and down
        float newY = startPosition.y + (Mathf.Sin(Time.time * bobSpeed) * bobHeight);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Play collection effect
            PlayCollectionEffect();
            
            // Destroy after effect
            Destroy(gameObject);
        }
    }

    void PlayCollectionEffect()
    {
        // Create sparkle effect
        GameObject effect = new GameObject("CollectionEffect");
        effect.transform.position = transform.position;
        
        // Add particle system
        ParticleSystem particles = effect.AddComponent<ParticleSystem>();
        var main = particles.main;
        main.startColor = Color.yellow;
        main.startSize = 0.2f;
        main.startSpeed = 2f;
        main.maxParticles = 20;
        main.duration = 0.5f;
        
        // Destroy effect after duration
        Destroy(effect, main.duration);
    }
}
