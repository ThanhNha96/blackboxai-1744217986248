using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

public class KeyBindingSystem : MonoBehaviour
{
    [System.Serializable]
    public class KeyBinding
    {
        public string actionName;
        public string displayName;
        public KeyCode defaultKey;
        public KeyCode currentKey;
        public string category;
        public bool allowMouseButton;
        public bool isWaitingForInput;
    }

    [System.Serializable]
    public class BindingCategory
    {
        public string name;
        public List<KeyBinding> bindings;
    }

    [Header("Categories")]
    public List<BindingCategory> categories = new List<BindingCategory>
    {
        new BindingCategory
        {
            name = "Movement",
            bindings = new List<KeyBinding>
            {
                new KeyBinding { 
                    actionName = "MoveForward",
                    displayName = "Di chuyển tiến",
                    defaultKey = KeyCode.W,
                    category = "Movement"
                },
                new KeyBinding {
                    actionName = "MoveBack",
                    displayName = "Di chuyển lùi",
                    defaultKey = KeyCode.S,
                    category = "Movement"
                },
                new KeyBinding {
                    actionName = "MoveLeft",
                    displayName = "Di chuyển trái",
                    defaultKey = KeyCode.A,
                    category = "Movement"
                },
                new KeyBinding {
                    actionName = "MoveRight",
                    displayName = "Di chuyển phải",
                    defaultKey = KeyCode.D,
                    category = "Movement"
                },
                new KeyBinding {
                    actionName = "Sprint",
                    displayName = "Chạy nhanh",
                    defaultKey = KeyCode.LeftShift,
                    category = "Movement"
                },
                new KeyBinding {
                    actionName = "Jump",
                    displayName = "Nhảy",
                    defaultKey = KeyCode.Space,
                    category = "Movement"
                }
            }
        },
        new BindingCategory
        {
            name = "Combat",
            bindings = new List<KeyBinding>
            {
                new KeyBinding {
                    actionName = "Attack",
                    displayName = "Tấn công",
                    defaultKey = KeyCode.Mouse0,
                    category = "Combat",
                    allowMouseButton = true
                },
                new KeyBinding {
                    actionName = "Block",
                    displayName = "Đỡ đòn",
                    defaultKey = KeyCode.Mouse1,
                    category = "Combat",
                    allowMouseButton = true
                },
                new KeyBinding {
                    actionName = "UseItem",
                    displayName = "Dùng vật phẩm",
                    defaultKey = KeyCode.Q,
                    category = "Combat"
                },
                new KeyBinding {
                    actionName = "Dodge",
                    displayName = "Lăn",
                    defaultKey = KeyCode.Space,
                    category = "Combat"
                }
            }
        },
        new BindingCategory
        {
            name = "Interaction",
            bindings = new List<KeyBinding>
            {
                new KeyBinding {
                    actionName = "Interact",
                    displayName = "Tương tác",
                    defaultKey = KeyCode.E,
                    category = "Interaction"
                },
                new KeyBinding {
                    actionName = "PickupDog",
                    displayName = "Bắt chó",
                    defaultKey = KeyCode.F,
                    category = "Interaction"
                },
                new KeyBinding {
                    actionName = "UseTaser",
                    displayName = "Dùng súng điện",
                    defaultKey = KeyCode.T,
                    category = "Interaction"
                }
            }
        },
        new BindingCategory
        {
            name = "Interface",
            bindings = new List<KeyBinding>
            {
                new KeyBinding {
                    actionName = "Inventory",
                    displayName = "Túi đồ",
                    defaultKey = KeyCode.I,
                    category = "Interface"
                },
                new KeyBinding {
                    actionName = "Map",
                    displayName = "Bản đồ",
                    defaultKey = KeyCode.M,
                    category = "Interface"
                },
                new KeyBinding {
                    actionName = "Quest",
                    displayName = "Nhiệm vụ",
                    defaultKey = KeyCode.J,
                    category = "Interface"
                },
                new KeyBinding {
                    actionName = "Skills",
                    displayName = "Kỹ năng",
                    defaultKey = KeyCode.K,
                    category = "Interface"
                }
            }
        },
        new BindingCategory
        {
            name = "Vehicle",
            bindings = new List<KeyBinding>
            {
                new KeyBinding {
                    actionName = "EnterVehicle",
                    displayName = "Lên xe",
                    defaultKey = KeyCode.F,
                    category = "Vehicle"
                },
                new KeyBinding {
                    actionName = "Horn",
                    displayName = "Bóp còi",
                    defaultKey = KeyCode.H,
                    category = "Vehicle"
                },
                new KeyBinding {
                    actionName = "Boost",
                    displayName = "Tăng tốc",
                    defaultKey = KeyCode.LeftShift,
                    category = "Vehicle"
                }
            }
        }
    };

    [Header("UI References")]
    public GameObject bindingEntryPrefab;
    public Transform bindingContainer;
    public TMP_Dropdown categoryDropdown;
    public Button resetButton;
    public TextMeshProUGUI waitingForInputText;

    private Dictionary<string, KeyBinding> bindingsMap = new Dictionary<string, KeyBinding>();
    private KeyBinding currentlyBinding;

    void Start()
    {
        InitializeBindings();
        SetupUI();
        LoadSavedBindings();
    }

    void InitializeBindings()
    {
        foreach (var category in categories)
        {
            foreach (var binding in category.bindings)
            {
                binding.currentKey = binding.defaultKey;
                bindingsMap[binding.actionName] = binding;
            }
        }
    }

    void SetupUI()
    {
        // Setup category dropdown
        categoryDropdown.ClearOptions();
        categoryDropdown.AddOptions(categories.ConvertAll(c => c.name));
        categoryDropdown.onValueChanged.AddListener(OnCategoryChanged);

        // Setup reset button
        resetButton.onClick.AddListener(ResetToDefaults);

        // Initial UI setup
        OnCategoryChanged(0);
    }

    void OnCategoryChanged(int index)
    {
        // Clear existing bindings
        foreach (Transform child in bindingContainer)
        {
            Destroy(child.gameObject);
        }

        // Create new binding entries
        var category = categories[index];
        foreach (var binding in category.bindings)
        {
            CreateBindingEntry(binding);
        }
    }

    void CreateBindingEntry(KeyBinding binding)
    {
        GameObject entry = Instantiate(bindingEntryPrefab, bindingContainer);
        
        // Setup entry UI
        TextMeshProUGUI nameText = entry.transform.Find("NameText").GetComponent<TextMeshProUGUI>();
        Button keyButton = entry.transform.Find("KeyButton").GetComponent<Button>();
        TextMeshProUGUI keyText = keyButton.GetComponentInChildren<TextMeshProUGUI>();

        nameText.text = binding.displayName;
        keyText.text = binding.currentKey.ToString();

        keyButton.onClick.AddListener(() => StartBinding(binding));
    }

    void StartBinding(KeyBinding binding)
    {
        if (currentlyBinding != null)
            return;

        currentlyBinding = binding;
        binding.isWaitingForInput = true;
        waitingForInputText.gameObject.SetActive(true);
        waitingForInputText.text = $"Nhấn phím cho {binding.displayName}...";
    }

    void Update()
    {
        if (currentlyBinding != null)
        {
            foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(kcode))
                {
                    // Check if key is allowed
                    if (!currentlyBinding.allowMouseButton && 
                        (kcode == KeyCode.Mouse0 || kcode == KeyCode.Mouse1))
                        continue;

                    // Check if key is already used
                    bool keyInUse = false;
                    foreach (var category in categories)
                    {
                        foreach (var binding in category.bindings)
                        {
                            if (binding != currentlyBinding && binding.currentKey == kcode)
                            {
                                keyInUse = true;
                                break;
                            }
                        }
                    }

                    if (keyInUse)
                    {
                        UIManager.Instance.ShowMessage("KEY_ALREADY_IN_USE");
                        continue;
                    }

                    // Assign new key
                    AssignNewBinding(currentlyBinding, kcode);
                    break;
                }
            }
        }
    }

    void AssignNewBinding(KeyBinding binding, KeyCode newKey)
    {
        binding.currentKey = newKey;
        binding.isWaitingForInput = false;
        currentlyBinding = null;
        waitingForInputText.gameObject.SetActive(false);

        // Update UI
        OnCategoryChanged(categoryDropdown.value);

        // Save bindings
        SaveBindings();
    }

    void SaveBindings()
    {
        foreach (var category in categories)
        {
            foreach (var binding in category.bindings)
            {
                PlayerPrefs.SetInt($"KeyBinding_{binding.actionName}", (int)binding.currentKey);
            }
        }
        PlayerPrefs.Save();
    }

    void LoadSavedBindings()
    {
        foreach (var category in categories)
        {
            foreach (var binding in category.bindings)
            {
                if (PlayerPrefs.HasKey($"KeyBinding_{binding.actionName}"))
                {
                    binding.currentKey = (KeyCode)PlayerPrefs.GetInt($"KeyBinding_{binding.actionName}");
                }
            }
        }
        OnCategoryChanged(categoryDropdown.value);
    }

    public void ResetToDefaults()
    {
        foreach (var category in categories)
        {
            foreach (var binding in category.bindings)
            {
                binding.currentKey = binding.defaultKey;
            }
        }
        OnCategoryChanged(categoryDropdown.value);
        SaveBindings();
    }

    public KeyCode GetBinding(string actionName)
    {
        if (bindingsMap.TryGetValue(actionName, out KeyBinding binding))
        {
            return binding.currentKey;
        }
        return KeyCode.None;
    }

    void OnDestroy()
    {
        SaveBindings();
    }
}
