using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;

public class NPCVoiceChat : MonoBehaviour
{
    [System.Serializable]
    public class VoiceConfig
    {
        public string voiceId;
        public float pitch = 1f;
        public float speed = 1f;
        public string language = "vi-VN";
        public string accent = "Northern";
        public float emotionStrength = 1f;
    }

    [Header("Voice Settings")]
    public VoiceConfig voiceConfig;
    public float maxListeningTime = 10f;
    public float silenceThreshold = 0.1f;
    public float responseDelay = 0.5f;

    [Header("AI Settings")]
    public string aiModel = "gpt-4";
    public string personality;
    public string background;
    public float temperature = 0.7f;
    public int maxTokens = 150;

    [Header("Audio")]
    public AudioSource voiceAudioSource;
    public float voiceVolume = 1f;
    public float spatialBlend = 0.8f;
    public float maxDistance = 10f;

    private bool isListening;
    private bool isSpeaking;
    private AudioClip microphoneClip;
    private string currentConversationContext;
    private NPCController npcController;
    private NPCTrustSystem trustSystem;

    void Start()
    {
        npcController = GetComponent<NPCController>();
        trustSystem = GetComponent<NPCTrustSystem>();
        InitializeVoiceChat();
    }

    void InitializeVoiceChat()
    {
        if (voiceAudioSource == null)
        {
            voiceAudioSource = gameObject.AddComponent<AudioSource>();
        }

        voiceAudioSource.spatialBlend = spatialBlend;
        voiceAudioSource.maxDistance = maxDistance;
        voiceAudioSource.volume = voiceVolume;
        voiceAudioSource.rolloffMode = AudioRolloffMode.Linear;
    }

    public void StartConversation()
    {
        if (!VoiceChatManager.Instance.IsVoiceChatEnabled())
        {
            UIManager.Instance.ShowMessage("ENABLE_VOICE_CHAT_FIRST");
            return;
        }

        currentConversationContext = GenerateContext();
        StartListening();
    }

    string GenerateContext()
    {
        return $"NPC Info:\n" +
               $"Name: {npcController.GetName()}\n" +
               $"Personality: {personality}\n" +
               $"Background: {background}\n" +
               $"Trust Level: {trustSystem.GetTrustLevel()}\n" +
               $"Current Time: {GameManager.Instance.GetGameTime()}\n" +
               $"Location: {GameManager.Instance.GetCurrentLocation()}\n";
    }

    void StartListening()
    {
        if (isListening || isSpeaking) return;

        isListening = true;
        microphoneClip = Microphone.Start(null, false, (int)maxListeningTime, 44100);
        StartCoroutine(ListeningRoutine());
    }

    IEnumerator ListeningRoutine()
    {
        float startTime = Time.time;
        float silenceTime = 0f;

        while (isListening)
        {
            // Check for silence
            float[] samples = new float[128];
            microphoneClip.GetData(samples, microphoneClip.samples - 128);
            float rms = 0f;
            foreach (float sample in samples)
            {
                rms += sample * sample;
            }
            rms = Mathf.Sqrt(rms / 128);

            if (rms < silenceThreshold)
            {
                silenceTime += Time.deltaTime;
            }
            else
            {
                silenceTime = 0f;
            }

            // Stop listening if silence too long or max time reached
            if (silenceTime > 2f || Time.time - startTime > maxListeningTime)
            {
                StopListening();
                break;
            }

            yield return null;
        }
    }

    void StopListening()
    {
        if (!isListening) return;

        isListening = false;
        Microphone.End(null);

        // Convert audio to text
        StartCoroutine(ProcessSpeech());
    }

    IEnumerator ProcessSpeech()
    {
        // Convert microphone clip to WAV bytes
        byte[] wavData = AudioClipToWav(microphoneClip);

        // Send to speech-to-text service
        using (UnityWebRequest www = UnityWebRequest.Post("your-speech-to-text-api", wavData))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Speech to text error: " + www.error);
                yield break;
            }

            string userText = www.downloadHandler.text;
            ProcessUserInput(userText);
        }
    }

    void ProcessUserInput(string userInput)
    {
        // Prepare AI request
        string prompt = $"{currentConversationContext}\n\nUser: {userInput}\n\nResponse:";

        StartCoroutine(GetAIResponse(prompt));
    }

    IEnumerator GetAIResponse(string prompt)
    {
        // Send to AI service
        using (UnityWebRequest www = UnityWebRequest.Post("your-ai-api", prompt))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("AI response error: " + www.error);
                yield break;
            }

            string aiResponse = www.downloadHandler.text;
            StartCoroutine(SpeakResponse(aiResponse));
        }
    }

    IEnumerator SpeakResponse(string text)
    {
        yield return new WaitForSeconds(responseDelay);

        // Convert text to speech
        using (UnityWebRequest www = UnityWebRequest.Post("your-text-to-speech-api", text))
        {
            www.SetRequestHeader("Voice-ID", voiceConfig.voiceId);
            www.SetRequestHeader("Language", voiceConfig.language);
            www.SetRequestHeader("Accent", voiceConfig.accent);

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Text to speech error: " + www.error);
                yield break;
            }

            // Play the response
            byte[] audioData = www.downloadHandler.data;
            PlayVoiceResponse(audioData);
        }
    }

    void PlayVoiceResponse(byte[] audioData)
    {
        StartCoroutine(PlayVoiceResponseRoutine(audioData));
    }

    IEnumerator PlayVoiceResponseRoutine(byte[] audioData)
    {
        isSpeaking = true;

        // Convert audio data to AudioClip
        AudioClip clip = WavToAudioClip(audioData);
        voiceAudioSource.clip = clip;
        voiceAudioSource.Play();

        // Wait for audio to finish
        yield return new WaitForSeconds(clip.length);

        isSpeaking = false;
    }

    byte[] AudioClipToWav(AudioClip clip)
    {
        // Implementation of converting AudioClip to WAV format
        // This is a placeholder - actual implementation needed
        return new byte[0];
    }

    AudioClip WavToAudioClip(byte[] wavData)
    {
        // Implementation of converting WAV data to AudioClip
        // This is a placeholder - actual implementation needed
        return null;
    }

    public bool IsInConversation()
    {
        return isListening || isSpeaking;
    }

    public void StopConversation()
    {
        if (isListening)
        {
            StopListening();
        }
        if (isSpeaking)
        {
            voiceAudioSource.Stop();
            isSpeaking = false;
        }
    }

    void OnDestroy()
    {
        StopConversation();
    }
}
