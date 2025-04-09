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

    public void ShowLoadingScreen()
    {
        // Implement loading screen logic
    }

    public void HideLoadingScreen()
    {
        // Implement hide loading screen logic
    }

    public void ShowMessage(string message)
    {
        // Implement message display logic
    }
}
