using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string tooltipText;
    public float showDelay = 0.5f;
    
    private static GameObject tooltipPrefab;
    private GameObject currentTooltip;
    private float hoverStartTime;
    private bool isHovering;
    private Transform playerTransform;
    private Transform targetTransform;

    void Start()
    {
        if (tooltipPrefab == null)
        {
            CreateTooltipPrefab();
        }
    }

    void CreateTooltipPrefab()
    {
        tooltipPrefab = new GameObject("TooltipPrefab");
        tooltipPrefab.SetActive(false);
        
        // Background
        var background = new GameObject("Background");
        background.transform.SetParent(tooltipPrefab.transform);
        var bgImage = background.AddComponent<UnityEngine.UI.Image>();
        bgImage.color = new Color(0, 0, 0, 0.8f);
        
        // Text
        var textObj = new GameObject("Text");
        textObj.transform.SetParent(tooltipPrefab.transform);
        var tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.color = Color.white;
        tmp.fontSize = 14;
        tmp.alignment = TextAlignmentOptions.Center;
        
        // Layout
        var layout = tooltipPrefab.AddComponent<UnityEngine.UI.VerticalLayoutGroup>();
        layout.padding = new RectOffset(10, 10, 5, 5);
        
        // Size Fitter
        var fitter = tooltipPrefab.AddComponent<UnityEngine.UI.ContentSizeFitter>();
        fitter.horizontalFit = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize;
        fitter.verticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        hoverStartTime = Time.unscaledTime;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        if (currentTooltip != null)
        {
            Destroy(currentTooltip);
            currentTooltip = null;
        }
    }

    void Update()
    {
        if (isHovering && currentTooltip == null && Time.unscaledTime - hoverStartTime >= showDelay)
        {
            ShowTooltip();
        }
    }

    void ShowTooltip()
    {
        currentTooltip = Instantiate(tooltipPrefab, transform.position, Quaternion.identity, transform);
        currentTooltip.SetActive(true);

        // Set text
        string finalText = tooltipText;
        if (playerTransform != null && targetTransform != null)
        {
            float distance = Vector3.Distance(playerTransform.position, targetTransform.position);
            finalText += $"\n{distance:F0}m";
        }

        var tmp = currentTooltip.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null)
        {
            tmp.text = finalText;
        }

        // Position tooltip above marker
        RectTransform rt = currentTooltip.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(0, 50);
    }

    public void UpdateDistance(Transform player, Transform target)
    {
        playerTransform = player;
        targetTransform = target;
        
        if (currentTooltip != null && playerTransform != null && targetTransform != null)
        {
            var tmp = currentTooltip.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null)
            {
                float distance = Vector3.Distance(playerTransform.position, targetTransform.position);
                tmp.text = $"{tooltipText}\n{distance:F0}m";
            }
        }
    }

    void OnDestroy()
    {
        if (currentTooltip != null)
        {
            Destroy(currentTooltip);
        }
    }
}
