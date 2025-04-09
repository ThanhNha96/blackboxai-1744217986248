using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class LocalizedText : MonoBehaviour
{
    public string localizationKey;
    private Text textComponent;

    void Start()
    {
        textComponent = GetComponent<Text>();
        UpdateText();
        LocalizationManager.Instance.OnLanguageChanged += UpdateText;
    }

    void OnDestroy()
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged -= UpdateText;
        }
    }

    public void UpdateText()
    {
        if (textComponent != null && !string.IsNullOrEmpty(localizationKey))
        {
            textComponent.text = LocalizationManager.Instance.GetLocalizedText(localizationKey);
        }
    }

    public void SetKey(string key)
    {
        localizationKey = key;
        UpdateText();
    }
}
