using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPCController : MonoBehaviour
{
    [Header("NPC Properties")]
    public string npcName;
    public NPCType npcType;
    public DogAppearance dogAppearance;
    public float moveSpeed = 3f;
    public float interactionRadius = 2f;
    
    [Header("Dialogue")]
    public DialogueData[] dialogues;
    public float typingSpeed = 0.05f;
    
    [Header("Shop")]
    public bool isShopkeeper;
    public ShopItem[] shopItems;
    
    [Header("Movement")]
    public bool canMove = true;
    public Transform[] patrolPoints;
    public float waitTimeAtPoint = 2f;
    
    private int currentPatrolIndex;
    private bool isWaiting;
    private bool isInteracting;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    
    public enum NPCType
    {
        Villager,
        Shopkeeper,
        QuestGiver,
        Trainer
    }
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        StartCoroutine(PatrolRoutine());
    }
    
    void Update()
    {
        if (isInteracting) return;
        
        // Check for nearby player
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, interactionRadius);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                UIManager.Instance.ShowMessage("PRESS_E_TO_TALK");
                if (Input.GetKeyDown(KeyCode.E))
                {
                    StartInteraction();
                }
                break;
            }
        }
    }
    
    IEnumerator PatrolRoutine()
    {
        if (!canMove || patrolPoints == null || patrolPoints.Length == 0)
            yield break;
            
        while (true)
        {
            if (!isInteracting && !isWaiting)
            {
                Transform targetPoint = patrolPoints[currentPatrolIndex];
                
                // Move towards target
                while (Vector2.Distance(transform.position, targetPoint.position) > 0.1f)
                {
                    if (isInteracting) yield break;
                    
                    Vector2 direction = (targetPoint.position - transform.position).normalized;
                    transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
                    
                    // Update facing direction
                    spriteRenderer.flipX = direction.x < 0;
                    
                    // Update animation
                    if (animator != null)
                    {
                        animator.SetBool("IsWalking", true);
                    }
                    
                    yield return null;
                }
                
                // Wait at point
                if (animator != null)
                {
                    animator.SetBool("IsWalking", false);
                }
                
                isWaiting = true;
                yield return new WaitForSeconds(waitTimeAtPoint);
                isWaiting = false;
                
                // Move to next point
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            }
            yield return null;
        }
    }
    
    void StartInteraction()
    {
        isInteracting = true;
        StopAllCoroutines();
        
        if (animator != null)
        {
            animator.SetBool("IsWalking", false);
        }
        
        if (isShopkeeper)
        {
            OpenShop();
        }
        else
        {
            StartCoroutine(ShowDialogue());
        }
    }
    
    IEnumerator ShowDialogue()
    {
        if (dialogues == null || dialogues.Length == 0)
        {
            EndInteraction();
            yield break;
        }
        
        foreach (DialogueData dialogue in dialogues)
        {
            string localizedText = LocalizationManager.Instance.GetLocalizedText(dialogue.dialogueKey);
            
            // Show dialogue UI
            UIManager.Instance.ShowDialogue(npcName, "");
            
            // Type out text
            string currentText = "";
            foreach (char c in localizedText)
            {
                currentText += c;
                UIManager.Instance.UpdateDialogueText(currentText);
                yield return new WaitForSeconds(typingSpeed);
            }
            
            // Wait for player input
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        }
        
        EndInteraction();
    }
    
    void OpenShop()
    {
        if (shopItems == null || shopItems.Length == 0)
        {
            EndInteraction();
            return;
        }
        
        UIManager.Instance.ShowShop(shopItems, EndInteraction);
    }
    
    void EndInteraction()
    {
        isInteracting = false;
        UIManager.Instance.HideDialogue();
        UIManager.Instance.HideShop();
        StartCoroutine(PatrolRoutine());
    }
}

[System.Serializable]
public class DialogueData
{
    public string dialogueKey;
    public AudioClip voiceClip;
}

[System.Serializable]
public class ShopItem
{
    public string nameKey;
    public string descriptionKey;
    public Sprite icon;
    public int price;
    public ItemType type;
    public int itemId;
    
    public enum ItemType
    {
        Clothing,
        Accessory,
        Vehicle,
        Consumable
    }
}
