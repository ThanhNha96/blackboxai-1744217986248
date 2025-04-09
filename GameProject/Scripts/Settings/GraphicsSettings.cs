using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GraphicsSettings : MonoBehaviour
{
    [System.Serializable]
    public class QualityPreset
    {
        public string presetName;
        public int targetFPS;
        public int resolutionWidth;
        public int resolutionHeight;
        public bool useVSync;
        public int antiAliasing; // 0, 2, 4, 8
        public int anisotropicFiltering; // 0, 2, 4, 8, 16
        public ShadowQuality shadowQuality;
        public TextureQuality textureQuality;
        public bool useBloom;
        public bool useMotionBlur;
        public bool useAmbientOcclusion;
    }

    public enum ShadowQuality
    {
        Disabled,
        Low,
        Medium,
        High,
        Ultra
    }

    public enum TextureQuality
    {
        Low,        // Quarter resolution
        Medium,     // Half resolution
        High,       // Full resolution
        Ultra       // Original resolution
    }

    [Header("Quality Presets")]
    public QualityPreset[] qualityPresets = new QualityPreset[]
    {
        new QualityPreset {
            presetName = "Low",
            targetFPS = 60,
            resolutionWidth = 1280,
            resolutionHeight = 720,
            useVSync = false,
            antiAliasing = 0,
            anisotropicFiltering = 0,
            shadowQuality = ShadowQuality.Disabled,
            textureQuality = TextureQuality.Low,
            useBloom = false,
            useMotionBlur = false,
            useAmbientOcclusion = false
        },
        new QualityPreset {
            presetName = "Medium",
            targetFPS = 60,
            resolutionWidth = 1920,
            resolutionHeight = 1080,
            useVSync = true,
            antiAliasing = 2,
            anisotropicFiltering = 4,
            shadowQuality = ShadowQuality.Medium,
            textureQuality = TextureQuality.Medium,
            useBloom = true,
            useMotionBlur = false,
            useAmbientOcclusion = false
        },
        new QualityPreset {
            presetName = "High",
            targetFPS = 60,
            resolutionWidth = 2560,
            resolutionHeight = 1440,
            useVSync = true,
            antiAliasing = 4,
            anisotropicFiltering = 8,
            shadowQuality = ShadowQuality.High,
            textureQuality = TextureQuality.High,
            useBloom = true,
            useMotionBlur = true,
            useAmbientOcclusion = true
        },
        new QualityPreset {
            presetName = "Ultra",
            targetFPS = 60,
            resolutionWidth = 3840,
            resolutionHeight = 2160,
            useVSync = true,
            antiAliasing = 8,
            anisotropicFiltering = 16,
            shadowQuality = ShadowQuality.Ultra,
            textureQuality = TextureQuality.Ultra,
            useBloom = true,
            useMotionBlur = true,
            useAmbientOcclusion = true
        }
    };

    [Header("UI References")]
    public TMP_Dropdown qualityDropdown;
    public TMP_Dropdown resolutionDropdown;
    public Toggle vSyncToggle;
    public TMP_Dropdown antiAliasingDropdown;
    public TMP_Dropdown shadowQualityDropdown;
    public TMP_Dropdown textureQualityDropdown;
    public Toggle bloomToggle;
    public Toggle motionBlurToggle;
    public Toggle ambientOcclusionToggle;
    public Button applyButton;
    public Button autoDetectButton;

    private Resolution[] availableResolutions;
    private QualityPreset currentPreset;
    private UnityEngine.Rendering.PostProcessing.PostProcessVolume postProcessVolume;

    void Start()
    {
        InitializeUI();
        LoadCurrentSettings();
        postProcessVolume = FindObjectOfType<UnityEngine.Rendering.PostProcessing.PostProcessVolume>();
    }

    void InitializeUI()
    {
        // Setup quality preset dropdown
        qualityDropdown.ClearOptions();
        qualityDropdown.AddOptions(new List<string>(System.Array.ConvertAll(qualityPresets, x => x.presetName)));
        qualityDropdown.onValueChanged.AddListener(OnQualityPresetChanged);

        // Setup resolution dropdown
        availableResolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> resOptions = new List<string>();
        foreach (Resolution res in availableResolutions)
        {
            resOptions.Add($"{res.width}x{res.height} @{res.refreshRate}Hz");
        }
        resolutionDropdown.AddOptions(resOptions);

        // Setup other UI elements
        vSyncToggle.onValueChanged.AddListener(OnVSyncChanged);
        antiAliasingDropdown.onValueChanged.AddListener(OnAntiAliasingChanged);
        shadowQualityDropdown.onValueChanged.AddListener(OnShadowQualityChanged);
        textureQualityDropdown.onValueChanged.AddListener(OnTextureQualityChanged);
        bloomToggle.onValueChanged.AddListener(OnBloomChanged);
        motionBlurToggle.onValueChanged.AddListener(OnMotionBlurChanged);
        ambientOcclusionToggle.onValueChanged.AddListener(OnAmbientOcclusionChanged);

        applyButton.onClick.AddListener(ApplySettings);
        autoDetectButton.onClick.AddListener(AutoDetectSettings);
    }

    void LoadCurrentSettings()
    {
        // Load saved settings or use defaults
        int qualityLevel = PlayerPrefs.GetInt("QualityLevel", 1);
        currentPreset = qualityPresets[qualityLevel];
        ApplyPreset(currentPreset);
        UpdateUIFromPreset(currentPreset);
    }

    void UpdateUIFromPreset(QualityPreset preset)
    {
        qualityDropdown.value = System.Array.FindIndex(qualityPresets, x => x.presetName == preset.presetName);
        vSyncToggle.isOn = preset.useVSync;
        antiAliasingDropdown.value = GetAntiAliasingIndex(preset.antiAliasing);
        shadowQualityDropdown.value = (int)preset.shadowQuality;
        textureQualityDropdown.value = (int)preset.textureQuality;
        bloomToggle.isOn = preset.useBloom;
        motionBlurToggle.isOn = preset.useMotionBlur;
        ambientOcclusionToggle.isOn = preset.useAmbientOcclusion;

        // Find closest resolution
        int resIndex = FindClosestResolution(preset.resolutionWidth, preset.resolutionHeight);
        resolutionDropdown.value = resIndex;
    }

    int FindClosestResolution(int width, int height)
    {
        int closest = 0;
        int minDiff = int.MaxValue;
        for (int i = 0; i < availableResolutions.Length; i++)
        {
            int diff = Mathf.Abs(availableResolutions[i].width - width) + 
                      Mathf.Abs(availableResolutions[i].height - height);
            if (diff < minDiff)
            {
                minDiff = diff;
                closest = i;
            }
        }
        return closest;
    }

    int GetAntiAliasingIndex(int aa)
    {
        switch (aa)
        {
            case 0: return 0;
            case 2: return 1;
            case 4: return 2;
            case 8: return 3;
            default: return 0;
        }
    }

    void ApplyPreset(QualityPreset preset)
    {
        // Apply resolution
        Resolution res = availableResolutions[FindClosestResolution(preset.resolutionWidth, preset.resolutionHeight)];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);

        // Apply quality settings
        QualitySettings.vSyncCount = preset.useVSync ? 1 : 0;
        QualitySettings.antiAliasing = preset.antiAliasing;
        QualitySettings.anisotropicFiltering = preset.anisotropicFiltering == 0 ? 
            AnisotropicFiltering.Disable : AnisotropicFiltering.Enable;
        QualitySettings.shadowQuality = (UnityEngine.ShadowQuality)preset.shadowQuality;

        // Apply post-processing
        if (postProcessVolume != null)
        {
            var bloom = postProcessVolume.profile.GetSetting<UnityEngine.Rendering.PostProcessing.Bloom>();
            var motionBlur = postProcessVolume.profile.GetSetting<UnityEngine.Rendering.PostProcessing.MotionBlur>();
            var ao = postProcessVolume.profile.GetSetting<UnityEngine.Rendering.PostProcessing.AmbientOcclusion>();

            bloom.active = preset.useBloom;
            motionBlur.active = preset.useMotionBlur;
            ao.active = preset.useAmbientOcclusion;
        }

        // Save settings
        PlayerPrefs.SetInt("QualityLevel", System.Array.FindIndex(qualityPresets, x => x.presetName == preset.presetName));
        PlayerPrefs.Save();
    }

    void AutoDetectSettings()
    {
        // Get system info
        SystemInfo.graphicsMemorySize;
        SystemInfo.processorCount;
        SystemInfo.systemMemorySize;

        // Simple auto-detection based on system specs
        int qualityLevel;
        if (SystemInfo.graphicsMemorySize >= 8000 && SystemInfo.systemMemorySize >= 16000)
            qualityLevel = 3; // Ultra
        else if (SystemInfo.graphicsMemorySize >= 4000 && SystemInfo.systemMemorySize >= 8000)
            qualityLevel = 2; // High
        else if (SystemInfo.graphicsMemorySize >= 2000 && SystemInfo.systemMemorySize >= 4000)
            qualityLevel = 1; // Medium
        else
            qualityLevel = 0; // Low

        currentPreset = qualityPresets[qualityLevel];
        UpdateUIFromPreset(currentPreset);
        ApplyPreset(currentPreset);
    }

    // UI Event Handlers
    void OnQualityPresetChanged(int index)
    {
        currentPreset = qualityPresets[index];
        UpdateUIFromPreset(currentPreset);
    }

    void OnVSyncChanged(bool value)
    {
        currentPreset.useVSync = value;
    }

    void OnAntiAliasingChanged(int index)
    {
        int[] aaValues = { 0, 2, 4, 8 };
        currentPreset.antiAliasing = aaValues[index];
    }

    void OnShadowQualityChanged(int index)
    {
        currentPreset.shadowQuality = (ShadowQuality)index;
    }

    void OnTextureQualityChanged(int index)
    {
        currentPreset.textureQuality = (TextureQuality)index;
    }

    void OnBloomChanged(bool value)
    {
        currentPreset.useBloom = value;
    }

    void OnMotionBlurChanged(bool value)
    {
        currentPreset.useMotionBlur = value;
    }

    void OnAmbientOcclusionChanged(bool value)
    {
        currentPreset.useAmbientOcclusion = value;
    }

    void ApplySettings()
    {
        ApplyPreset(currentPreset);
        UIManager.Instance.ShowMessage("SETTINGS_APPLIED");
    }

    public void ResetToDefaults()
    {
        currentPreset = qualityPresets[1]; // Medium preset
        UpdateUIFromPreset(currentPreset);
        ApplyPreset(currentPreset);
    }
}
