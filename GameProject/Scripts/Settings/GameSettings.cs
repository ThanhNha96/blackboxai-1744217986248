using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;
using System;

public class GameSettings : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioMixer audioMixer;
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Slider voiceVolumeSlider;
    public Toggle muteToggle;

    [Header("Mouse Settings")]
    public Slider mouseSensitivitySlider;
    public Toggle mouseInvertYToggle;
    public Toggle mouseAccelerationToggle;
    public TMP_Dropdown mousePresetDropdown;

    [Header("Voice Chat Settings")]
    public Toggle voiceChatEnabledToggle;
    public TMP_Dropdown microphoneDropdown;
    public Slider microphoneVolumeSlider;
    public Toggle pushToTalkToggle;
    public TMP_InputField pushToTalkKeyBind;
    public Toggle voiceActivationToggle;
    public Slider voiceActivationThresholdSlider;
    public Toggle autoGainControlToggle;
    public Toggle noiseCancellationToggle;

    private bool isInitialized = false;

    void Start()
    {
        InitializeSettings();
    }

    void InitializeSettings()
    {
        if (isInitialized) return;

        // Initialize Audio Settings
        SetupAudioSettings();

        // Initialize Mouse Settings
        SetupMouseSettings();

        // Initialize Voice Chat Settings
        SetupVoiceChatSettings();

        LoadSavedSettings();
        isInitialized = true;
    }

    void SetupAudioSettings()
    {
        // Setup volume sliders
        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
        voiceVolumeSlider.onValueChanged.AddListener(SetVoiceVolume);

        // Setup mute toggle
        muteToggle.onValueChanged.AddListener(SetMute);
    }

    void SetupMouseSettings()
    {
        // Setup mouse sensitivity
        mouseSensitivitySlider.onValueChanged.AddListener(SetMouseSensitivity);
        mouseInvertYToggle.onValueChanged.AddListener(SetMouseInvertY);
        mouseAccelerationToggle.onValueChanged.AddListener(SetMouseAcceleration);

        // Setup mouse presets
        mousePresetDropdown.ClearOptions();
        mousePresetDropdown.AddOptions(new System.Collections.Generic.List<string> {
            "Default",
            "FPS",
            "Strategy",
            "Custom"
        });
        mousePresetDropdown.onValueChanged.AddListener(SetMousePreset);
    }

    void SetupVoiceChatSettings()
    {
        // Setup microphone devices dropdown
        microphoneDropdown.ClearOptions();
        var mics = Microphone.devices;
        if (mics.Length > 0)
        {
            microphoneDropdown.AddOptions(new System.Collections.Generic.List<string>(mics));
        }

        // Setup voice chat toggles and sliders
        voiceChatEnabledToggle.onValueChanged.AddListener(SetVoiceChatEnabled);
        microphoneVolumeSlider.onValueChanged.AddListener(SetMicrophoneVolume);
        pushToTalkToggle.onValueChanged.AddListener(SetPushToTalk);
        voiceActivationToggle.onValueChanged.AddListener(SetVoiceActivation);
        voiceActivationThresholdSlider.onValueChanged.AddListener(SetVoiceActivationThreshold);
        autoGainControlToggle.onValueChanged.AddListener(SetAutoGainControl);
        noiseCancellationToggle.onValueChanged.AddListener(SetNoiseCancellation);
    }

    void LoadSavedSettings()
    {
        // Load Audio Settings
        masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
        voiceVolumeSlider.value = PlayerPrefs.GetFloat("VoiceVolume", 1f);
        muteToggle.isOn = PlayerPrefs.GetInt("Muted", 0) == 1;

        // Load Mouse Settings
        mouseSensitivitySlider.value = PlayerPrefs.GetFloat("MouseSensitivity", 1f);
        mouseInvertYToggle.isOn = PlayerPrefs.GetInt("MouseInvertY", 0) == 1;
        mouseAccelerationToggle.isOn = PlayerPrefs.GetInt("MouseAcceleration", 0) == 1;
        mousePresetDropdown.value = PlayerPrefs.GetInt("MousePreset", 0);

        // Load Voice Chat Settings
        voiceChatEnabledToggle.isOn = PlayerPrefs.GetInt("VoiceChatEnabled", 0) == 1;
        microphoneVolumeSlider.value = PlayerPrefs.GetFloat("MicrophoneVolume", 1f);
        pushToTalkToggle.isOn = PlayerPrefs.GetInt("PushToTalk", 1) == 1;
        voiceActivationToggle.isOn = PlayerPrefs.GetInt("VoiceActivation", 0) == 1;
        voiceActivationThresholdSlider.value = PlayerPrefs.GetFloat("VoiceActivationThreshold", 0.1f);
        autoGainControlToggle.isOn = PlayerPrefs.GetInt("AutoGainControl", 1) == 1;
        noiseCancellationToggle.isOn = PlayerPrefs.GetInt("NoiseCancellation", 1) == 1;

        // Apply loaded settings
        ApplyAllSettings();
    }

    void ApplyAllSettings()
    {
        // Apply Audio Settings
        SetMasterVolume(masterVolumeSlider.value);
        SetMusicVolume(musicVolumeSlider.value);
        SetSFXVolume(sfxVolumeSlider.value);
        SetVoiceVolume(voiceVolumeSlider.value);
        SetMute(muteToggle.isOn);

        // Apply Mouse Settings
        SetMouseSensitivity(mouseSensitivitySlider.value);
        SetMouseInvertY(mouseInvertYToggle.isOn);
        SetMouseAcceleration(mouseAccelerationToggle.isOn);
        SetMousePreset(mousePresetDropdown.value);

        // Apply Voice Chat Settings
        SetVoiceChatEnabled(voiceChatEnabledToggle.isOn);
        SetMicrophoneVolume(microphoneVolumeSlider.value);
        SetPushToTalk(pushToTalkToggle.isOn);
        SetVoiceActivation(voiceActivationToggle.isOn);
        SetVoiceActivationThreshold(voiceActivationThresholdSlider.value);
        SetAutoGainControl(autoGainControlToggle.isOn);
        SetNoiseCancellation(noiseCancellationToggle.isOn);
    }

    #region Audio Settings Methods
    void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    void SetVoiceVolume(float volume)
    {
        audioMixer.SetFloat("VoiceVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("VoiceVolume", volume);
    }

    void SetMute(bool muted)
    {
        audioMixer.SetFloat("MasterVolume", muted ? -80f : Mathf.Log10(masterVolumeSlider.value) * 20);
        PlayerPrefs.SetInt("Muted", muted ? 1 : 0);
    }
    #endregion

    #region Mouse Settings Methods
    void SetMouseSensitivity(float sensitivity)
    {
        InputSystem.Instance.mouseSensitivity = sensitivity;
        PlayerPrefs.SetFloat("MouseSensitivity", sensitivity);
    }

    void SetMouseInvertY(bool invert)
    {
        InputSystem.Instance.invertMouseY = invert;
        PlayerPrefs.SetInt("MouseInvertY", invert ? 1 : 0);
    }

    void SetMouseAcceleration(bool enabled)
    {
        InputSystem.Instance.mouseAcceleration = enabled;
        PlayerPrefs.SetInt("MouseAcceleration", enabled ? 1 : 0);
    }

    void SetMousePreset(int preset)
    {
        switch (preset)
        {
            case 0: // Default
                mouseSensitivitySlider.value = 1f;
                mouseInvertYToggle.isOn = false;
                mouseAccelerationToggle.isOn = true;
                break;
            case 1: // FPS
                mouseSensitivitySlider.value = 0.7f;
                mouseInvertYToggle.isOn = false;
                mouseAccelerationToggle.isOn = false;
                break;
            case 2: // Strategy
                mouseSensitivitySlider.value = 1.3f;
                mouseInvertYToggle.isOn = false;
                mouseAccelerationToggle.isOn = true;
                break;
        }
        PlayerPrefs.SetInt("MousePreset", preset);
    }
    #endregion

    #region Voice Chat Settings Methods
    void SetVoiceChatEnabled(bool enabled)
    {
        if (enabled && Microphone.devices.Length == 0)
        {
            UIManager.Instance.ShowMessage("NO_MICROPHONE_FOUND");
            voiceChatEnabledToggle.isOn = false;
            return;
        }

        VoiceChatManager.Instance.enabled = enabled;
        PlayerPrefs.SetInt("VoiceChatEnabled", enabled ? 1 : 0);
    }

    void SetMicrophoneVolume(float volume)
    {
        VoiceChatManager.Instance.microphoneVolume = volume;
        PlayerPrefs.SetFloat("MicrophoneVolume", volume);
    }

    void SetPushToTalk(bool enabled)
    {
        VoiceChatManager.Instance.usePushToTalk = enabled;
        PlayerPrefs.SetInt("PushToTalk", enabled ? 1 : 0);
    }

    void SetVoiceActivation(bool enabled)
    {
        VoiceChatManager.Instance.useVoiceActivation = enabled;
        PlayerPrefs.SetInt("VoiceActivation", enabled ? 1 : 0);
    }

    void SetVoiceActivationThreshold(float threshold)
    {
        VoiceChatManager.Instance.voiceActivationThreshold = threshold;
        PlayerPrefs.SetFloat("VoiceActivationThreshold", threshold);
    }

    void SetAutoGainControl(bool enabled)
    {
        VoiceChatManager.Instance.useAutoGainControl = enabled;
        PlayerPrefs.SetInt("AutoGainControl", enabled ? 1 : 0);
    }

    void SetNoiseCancellation(bool enabled)
    {
        VoiceChatManager.Instance.useNoiseCancellation = enabled;
        PlayerPrefs.SetInt("NoiseCancellation", enabled ? 1 : 0);
    }
    #endregion

    public void ResetToDefaults()
    {
        PlayerPrefs.DeleteAll();
        LoadSavedSettings();
    }

    void OnDestroy()
    {
        PlayerPrefs.Save();
    }
}
