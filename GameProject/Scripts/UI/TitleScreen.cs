using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TitleScreen : MonoBehaviour
{
    [Header("Title Settings")]
    public string gameTitle = "GIẢ LẬP TRỘM CHÓ";
    public string subtitle = "DOG THEFT SIMULATOR";
    public Color titleColor = new Color(1f, 0.8f, 0f); // Gold color
    public float titlePulseSpeed = 1f;
    public float titlePulseAmount = 0.1f;

    [Header("UI Elements")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI subtitleText;
    public Button startButton;
    public Button continueButton;
    public Button optionsButton;
    public Button creditsButton;
    public Button quitButton;

    [Header("Visual Effects")]
    public ParticleSystem backgroundParticles;
    public AudioClip titleMusic;
    public AudioClip buttonSound;
    public GameObject dogSilhouette;
    public float dogRotateSpeed = 30f;

    [Header("Version Info")]
    public string versionNumber = "v1.0";
    public TextMeshProUGUI versionText;

    [Header("Features Preview")]
    public string[] featureTexts = new string[]
    {
        "Trở thành kẻ trộm chó chuyên nghiệp",
        "Nâng cấp kỹ năng đánh đấm và ẩn nấp",
        "Đối đầu với cảnh sát và chủ nhân của chó",
        "Thu thập và bán chó để kiếm tiền",
        "Khám phá thành phố rộng lớn",
        "Nhiều nhiệm vụ ẩn thú vị"
    };
    public float featureScrollSpeed = 2f;
    private int currentFeatureIndex = 0;
    public TextMeshProUGUI featureText;

    private void Start()
    {
        InitializeUI();
        StartCoroutine(CycleFeaturesRoutine());
        
        if (titleMusic != null)
        {
            AudioManager.Instance.PlayMusic(titleMusic);
        }
    }

    private void InitializeUI()
    {
        // Set up title
        if (titleText != null)
        {
            titleText.text = gameTitle;
            titleText.color = titleColor;
        }

        // Set up subtitle
        if (subtitleText != null)
        {
            subtitleText.text = subtitle;
        }

        // Set up version
        if (versionText != null)
        {
            versionText.text = versionNumber;
        }

        // Set up buttons
        SetupButton(startButton, "BẮT ĐẦU", StartNewGame);
        SetupButton(continueButton, "TIẾP TỤC", ContinueGame);
        SetupButton(optionsButton, "TÙY CHỌN", OpenOptions);
        SetupButton(creditsButton, "CREDIT", ShowCredits);
        SetupButton(quitButton, "THOÁT", QuitGame);

        // Check for save game
        continueButton.interactable = SaveSystem.HasSaveGame();
    }

    private void SetupButton(Button button, string text, UnityEngine.Events.UnityAction action)
    {
        if (button != null)
        {
            TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = text;
            }
            button.onClick.AddListener(() => {
                PlayButtonSound();
                action.Invoke();
            });
        }
    }

    private void Update()
    {
        // Pulse title effect
        if (titleText != null)
        {
            float pulse = 1f + Mathf.Sin(Time.time * titlePulseSpeed) * titlePulseAmount;
            titleText.transform.localScale = Vector3.one * pulse;
        }

        // Rotate dog silhouette
        if (dogSilhouette != null)
        {
            dogSilhouette.transform.Rotate(0, 0, dogRotateSpeed * Time.deltaTime);
        }
    }

    System.Collections.IEnumerator CycleFeaturesRoutine()
    {
        while (true)
        {
            if (featureText != null)
            {
                featureText.text = featureTexts[currentFeatureIndex];
                currentFeatureIndex = (currentFeatureIndex + 1) % featureTexts.Length;
            }
            yield return new WaitForSeconds(featureScrollSpeed);
        }
    }

    private void PlayButtonSound()
    {
        if (buttonSound != null)
        {
            AudioManager.Instance.PlaySFX(buttonSound);
        }
    }

    public void StartNewGame()
    {
        SaveSystem.DeleteSaveGame(); // Clear any existing save
        GameManager.Instance.StartNewGame();
    }

    public void ContinueGame()
    {
        GameManager.Instance.LoadGame();
    }

    public void OpenOptions()
    {
        UIManager.Instance.ShowOptionsMenu();
    }

    public void ShowCredits()
    {
        UIManager.Instance.ShowCredits();
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    void OnDestroy()
    {
        // Clean up button listeners
        if (startButton != null) startButton.onClick.RemoveAllListeners();
        if (continueButton != null) continueButton.onClick.RemoveAllListeners();
        if (optionsButton != null) optionsButton.onClick.RemoveAllListeners();
        if (creditsButton != null) creditsButton.onClick.RemoveAllListeners();
        if (quitButton != null) quitButton.onClick.RemoveAllListeners();
    }
}
