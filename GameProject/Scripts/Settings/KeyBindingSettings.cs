using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class KeyBindingSettings : MonoBehaviour
{
    [System.Serializable]
    public class KeyBinding
    {
        public string actionName;
        public string displayName;
        public KeyCode defaultKey;
        public KeyCode currentKey;
    }

    [Header("Key Bindings")]
    public List<KeyBinding> keyBindings = new List<KeyBinding>
    {
        new KeyBinding { actionName = "MoveForward", displayName = "Di chuyển tiến", defaultKey = KeyCode.W },
        new KeyBinding { actionName = "MoveBack", displayName = "Di chuyển lùi", defaultKey = KeyCode.S },
        new KeyBinding { actionName = "MoveLeft", displayName = "Di chuyển trái", defaultKey = KeyCode.A },
        new KeyBinding { actionName = "MoveRight", displayName = "Di chuyển phải", defaultKey = KeyCode.D },
        new KeyBinding { actionName = "Jump", displayName = "Nhảy", defaultKey = KeyCode.Space },
        new KeyBinding { actionName = "Attack", displayName = "Tấn công", defaultKey = KeyCode.Mouse0 },
        new KeyBinding { actionName = "Block", displayName = "Đỡ đòn", defaultKey = KeyCode.Mouse1 },
        new KeyBinding { actionName = "Interact", displayName = "Tương tác", defaultKey = KeyCode.E },
        new KeyBinding { actionName = "UseItem", displayName = "Dùng vật phẩm", defaultKey = KeyCode.Q },
        new KeyBinding { actionName = "Inventory", displayName = "Túi đồ", defaultKey = KeyCode.I },
        new KeyBinding { actionName = "Map", displayName = "Bản đồ", defaultKey = KeyCode.M },
        new KeyBinding { actionName = "Quest", displayName = "Nhiệm vụ", defaultKey = KeyCode.J },
        new KeyBinding { actionName = "Skills", displayName = "Kỹ năng", defaultKey = KeyCode.K }
    };

    [Header("UI References")]
    public GameObject bindingEntryPrefab;
    public Transform bindingContainer;
    public Button resetButton;

    private KeyBinding currentlyBinding;

    void Start()
    {
        InitializeBindings();
        SetupUI();
        LoadSavedBindings();
    }

    void InitializeBindings()
    {
        foreach (var binding in keyBindings)
        {
            binding.currentKey = binding.defaultKey;
        }
    }

    void SetupUI()
    {
        foreach (var binding in keyBindings)
        {
            CreateBindingEntry(binding);
        }

        resetButton.onClick.AddListener(ResetToDefaults);
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
        StartCoroutine(WaitForKeyPress());
    }

    System.Collections.IEnumerator WaitForKeyPress()
    {
        while (currentlyBinding != null)
        {
            for (int i = 0; i < 256; i++)
            {
                if (Input.GetKeyDown((KeyCode)i))
                {
                    AssignNewBinding(currentlyBinding, (KeyCode)i);
                    yield break;
                }
            }
            yield return null;
        }
    }

    void AssignNewBinding(KeyBinding binding, KeyCode newKey)
    {
        binding.currentKey = newKey;
        currentlyBinding = null;

        // Update UI
        UpdateBindingUI(binding);
        SaveBindings();
    }

    void UpdateBindingUI(KeyBinding binding)
    {
        foreach (Transform child in bindingContainer)
        {
            TextMeshProUGUI keyText = child.transform.Find("KeyButton").GetComponentInChildren<TextMeshProUGUI>();
            if (keyText != null && keyText.text == binding.displayName)
            {
                keyText.text = binding.currentKey.ToString();
                break;
            }
        }
    }

    void SaveBindings()
    {
        foreach (var binding in keyBindings)
        {
            PlayerPrefs.SetInt($"KeyBinding_{binding.actionName}", (int)binding.currentKey);
        }
        PlayerPrefs.Save();
    }

    void LoadSavedBindings()
    {
        foreach (var binding in keyBindings)
        {
            if (PlayerPrefs.HasKey($"KeyBinding_{binding.actionName}"))
            {
                binding.currentKey = (KeyCode)PlayerPrefs.GetInt($"KeyBinding_{binding.actionName}");
            }
        }
        UpdateAllBindingsUI();
    }

    void UpdateAllBindingsUI()
    {
        foreach (var binding in keyBindings)
        {
            UpdateBindingUI(binding);
        }
    }

    public void ResetToDefaults()
    {
        foreach (var binding in keyBindings)
        {
            binding.currentKey = binding.defaultKey;
        }
        UpdateAllBindingsUI();
        SaveBindings();
    }

    void OnDestroy()
    {
        SaveBindings();
    }
}
