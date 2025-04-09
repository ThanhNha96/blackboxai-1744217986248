using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AnimateButtonClick(Button button)
    {
        StartCoroutine(ButtonClickAnimation(button));
    }

    private IEnumerator ButtonClickAnimation(Button button)
    {
        Vector3 originalScale = button.transform.localScale;
        float animationDuration = 0.1f;
        
        // Scale down
        button.transform.localScale = originalScale * 0.9f;
        yield return new WaitForSeconds(animationDuration);
        
        // Scale back up
        button.transform.localScale = originalScale;
    }

    public GameObject loadingScreenPrefab;
    public GameObject messageBoxPrefab;
    private GameObject activeLoadingScreen;
    private GameObject activeMessageBox;

    public void ShowLoadingScreen()
    {
        if (loadingScreenPrefab != null)
        {
            activeLoadingScreen = Instantiate(loadingScreenPrefab, transform);
            activeLoadingScreen.SetActive(true);
        }
    }

    public void HideLoadingScreen()
    {
        if (activeLoadingScreen != null)
        {
            Destroy(activeLoadingScreen);
            activeLoadingScreen = null;
        }
    }

    public void ShowMessage(string messageKey)
    {
        if (messageBoxPrefab != null)
        {
            if (activeMessageBox != null)
            {
                Destroy(activeMessageBox);
            }
            
            activeMessageBox = Instantiate(messageBoxPrefab, transform);
            Text messageText = activeMessageBox.GetComponentInChildren<Text>();
            LocalizedText localizedText = activeMessageBox.GetComponentInChildren<LocalizedText>();
            
            if (localizedText != null)
            {
                localizedText.SetKey(messageKey);
            }
            else if (messageText != null)
            {
                messageText.text = LocalizationManager.Instance.GetLocalizedText(messageKey);
            }
            
            StartCoroutine(AutoHideMessage());
        }
    }

    private IEnumerator AutoHideMessage()
    {
        yield return new WaitForSeconds(3f);
        if (activeMessageBox != null)
        {
            Destroy(activeMessageBox);
            activeMessageBox = null;
        }
    }
}
