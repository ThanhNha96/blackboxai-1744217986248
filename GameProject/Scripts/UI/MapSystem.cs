using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MapSystem : MonoBehaviour
{
    [System.Serializable]
    public class MapMarker
    {
        public string id;
        public Transform target;
        public Sprite icon;
        public Color color = Color.white;
        public float scale = 1f;
        public MarkerType type;
        public string tooltipKey;
        public bool showDistance;
    }

    public enum MarkerType
    {
        Player,
        QuestGiver,
        QuestTarget,
        Shop,
        GasStation,
        Police,
        Important
    }

    [Header("Minimap Settings")]
    public Camera minimapCamera;
    public RectTransform minimapRect;
    public float minimapZoom = 5f;
    public float minimapRotation = true;
    public GameObject minimapMarkerPrefab;

    [Header("World Map Settings")]
    public GameObject worldMapPanel;
    public RectTransform worldMapRect;
    public float worldMapZoom = 20f;
    public bool worldMapRotation = false;
    public GameObject worldMapMarkerPrefab;

    [Header("Marker Icons")]
    public Sprite playerMarker;
    public Sprite questGiverMarker;
    public Sprite questTargetMarker;
    public Sprite shopMarker;
    public Sprite gasStationMarker;
    public Sprite policeMarker;
    public Sprite importantMarker;

    [Header("Marker Colors")]
    public Color availableQuestColor = Color.yellow;
    public Color activeQuestColor = Color.green;
    public Color completedQuestColor = Color.grey;
    public Color importantColor = Color.red;

    private Dictionary<string, MapMarker> markers = new Dictionary<string, MapMarker>();
    private Dictionary<string, GameObject> minimapMarkerObjects = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> worldMapMarkerObjects = new Dictionary<string, GameObject>();
    private bool isWorldMapOpen;
    private Transform playerTransform;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        InitializeMap();
        UpdateMapMarkers();
        InvokeRepeating("UpdateQuestMarkers", 1f, 1f);
    }

    void InitializeMap()
    {
        // Add player marker
        AddMarker(new MapMarker
        {
            id = "player",
            target = playerTransform,
            icon = playerMarker,
            type = MarkerType.Player,
            showDistance = false
        });

        // Add all NPCs with quests
        NPCController[] npcs = FindObjectsOfType<NPCController>();
        foreach (NPCController npc in npcs)
        {
            if (npc.GetComponent<QuestGiver>() != null)
            {
                AddMarker(new MapMarker
                {
                    id = "npc_" + npc.npcId,
                    target = npc.transform,
                    icon = questGiverMarker,
                    type = MarkerType.QuestGiver,
                    tooltipKey = npc.nameKey,
                    showDistance = true
                });
            }
        }

        // Add shops and services
        AddServiceMarkers();
    }

    void AddServiceMarkers()
    {
        // Add gas stations
        GasStation[] gasStations = FindObjectsOfType<GasStation>();
        foreach (GasStation station in gasStations)
        {
            AddMarker(new MapMarker
            {
                id = "gas_" + station.GetInstanceID(),
                target = station.transform,
                icon = gasStationMarker,
                type = MarkerType.GasStation,
                tooltipKey = "MAP_GAS_STATION",
                showDistance = true
            });
        }

        // Add shops
        ItemTrader[] shops = FindObjectsOfType<ItemTrader>();
        foreach (ItemTrader shop in shops)
        {
            AddMarker(new MapMarker
            {
                id = "shop_" + shop.GetInstanceID(),
                target = shop.transform,
                icon = shopMarker,
                type = MarkerType.Shop,
                tooltipKey = "MAP_SHOP",
                showDistance = true
            });
        }
    }

    void AddMarker(MapMarker marker)
    {
        markers[marker.id] = marker;
        CreateMarkerObjects(marker);
    }

    void CreateMarkerObjects(MapMarker marker)
    {
        // Create minimap marker
        GameObject minimapMarker = Instantiate(minimapMarkerPrefab, minimapRect);
        Image minimapIcon = minimapMarker.GetComponent<Image>();
        minimapIcon.sprite = marker.icon;
        minimapIcon.color = marker.color;
        minimapMarker.transform.localScale *= marker.scale;
        minimapMarkerObjects[marker.id] = minimapMarker;

        // Create world map marker
        GameObject worldMapMarker = Instantiate(worldMapMarkerPrefab, worldMapRect);
        Image worldMapIcon = worldMapMarker.GetComponent<Image>();
        worldMapIcon.sprite = marker.icon;
        worldMapIcon.color = marker.color;
        worldMapMarker.transform.localScale *= marker.scale;
        
        // Add tooltip if needed
        if (!string.IsNullOrEmpty(marker.tooltipKey))
        {
            TooltipTrigger tooltip = worldMapMarker.AddComponent<TooltipTrigger>();
            tooltip.tooltipText = LocalizationManager.Instance.GetLocalizedText(marker.tooltipKey);
            if (marker.showDistance)
            {
                tooltip.UpdateDistance(playerTransform, marker.target);
            }
        }
        
        worldMapMarkerObjects[marker.id] = worldMapMarker;
    }

    void Update()
    {
        // Toggle world map
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleWorldMap();
        }

        // Update markers
        UpdateMapMarkers();

        // Update minimap camera
        UpdateMinimapCamera();
    }

    void UpdateMapMarkers()
    {
        foreach (var marker in markers.Values)
        {
            if (marker.target != null)
            {
                // Update minimap marker
                if (minimapMarkerObjects.TryGetValue(marker.id, out GameObject minimapMarker))
                {
                    Vector2 minimapPosition = WorldToMinimapPoint(marker.target.position);
                    minimapMarker.transform.position = minimapPosition;
                }

                // Update world map marker
                if (worldMapMarkerObjects.TryGetValue(marker.id, out GameObject worldMapMarker))
                {
                    Vector2 worldMapPosition = WorldToWorldMapPoint(marker.target.position);
                    worldMapMarker.transform.position = worldMapPosition;
                    
                    // Update distance if needed
                    if (marker.showDistance)
                    {
                        TooltipTrigger tooltip = worldMapMarker.GetComponent<TooltipTrigger>();
                        if (tooltip != null)
                        {
                            tooltip.UpdateDistance(playerTransform, marker.target);
                        }
                    }
                }
            }
        }
    }

    void UpdateMinimapCamera()
    {
        if (minimapCamera != null && playerTransform != null)
        {
            // Position
            Vector3 newPos = playerTransform.position;
            newPos.z = minimapCamera.transform.position.z;
            minimapCamera.transform.position = newPos;

            // Rotation
            if (minimapRotation)
            {
                minimapCamera.transform.rotation = Quaternion.Euler(0, 0, -playerTransform.eulerAngles.z);
            }
        }
    }

    void UpdateQuestMarkers()
    {
        QuestManager questManager = QuestManager.Instance;
        foreach (var marker in markers.Values)
        {
            if (marker.type == MarkerType.QuestGiver)
            {
                string npcId = marker.id.Replace("npc_", "");
                NPCController npc = FindNPCById(npcId);
                if (npc != null)
                {
                    QuestGiver questGiver = npc.GetComponent<QuestGiver>();
                    if (questGiver != null)
                    {
                        // Update marker color based on quest status
                        Color newColor = availableQuestColor;
                        if (questManager.HasActiveQuest(npcId))
                        {
                            newColor = activeQuestColor;
                        }
                        else if (questManager.HasCompletedAllQuests(npcId))
                        {
                            newColor = completedQuestColor;
                        }

                        UpdateMarkerColor(marker.id, newColor);
                    }
                }
            }
        }
    }

    void UpdateMarkerColor(string markerId, Color color)
    {
        if (minimapMarkerObjects.TryGetValue(markerId, out GameObject minimapMarker))
        {
            minimapMarker.GetComponent<Image>().color = color;
        }
        if (worldMapMarkerObjects.TryGetValue(markerId, out GameObject worldMapMarker))
        {
            worldMapMarker.GetComponent<Image>().color = color;
        }
    }

    Vector2 WorldToMinimapPoint(Vector3 worldPosition)
    {
        Vector2 viewportPoint = minimapCamera.WorldToViewportPoint(worldPosition);
        return new Vector2(
            minimapRect.rect.x + viewportPoint.x * minimapRect.rect.width,
            minimapRect.rect.y + viewportPoint.y * minimapRect.rect.height
        );
    }

    Vector2 WorldToWorldMapPoint(Vector3 worldPosition)
    {
        return new Vector2(
            worldMapRect.rect.x + worldPosition.x * worldMapZoom,
            worldMapRect.rect.y + worldPosition.y * worldMapZoom
        );
    }

    void ToggleWorldMap()
    {
        isWorldMapOpen = !isWorldMapOpen;
        worldMapPanel.SetActive(isWorldMapOpen);
        
        // Pause/unpause game when map is open
        Time.timeScale = isWorldMapOpen ? 0f : 1f;
    }

    NPCController FindNPCById(string npcId)
    {
        NPCController[] npcs = FindObjectsOfType<NPCController>();
        return System.Array.Find(npcs, npc => npc.npcId == npcId);
    }
}
