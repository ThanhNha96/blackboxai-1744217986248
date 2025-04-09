using UnityEngine;

public class StealthSystem : MonoBehaviour
{
    public float detectionRadius = 5f; // Radius within which NPCs can detect the player
    public LayerMask npcLayer; // Layer for NPCs
    public float hideDuration = 3f; // Duration to stay hidden
    private bool isHiding = false;

    void Update()
    {
        DetectNPCs();
    }

    void DetectNPCs()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius, npcLayer);
        foreach (var hitCollider in hitColliders)
        {
            NPCController npc = hitCollider.GetComponent<NPCController>();
            if (npc != null && !isHiding)
            {
                // If the player is detected, trigger NPC alert
                npc.AlertPlayer();
            }
        }
    }

}
