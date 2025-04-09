using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [System.Serializable]
    public class SoundEffect
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)]
        public float volume = 1f;
        [Range(0.1f, 3f)]
        public float pitch = 1f;
        public bool loop = false;
        [HideInInspector]
        public AudioSource source;
    }

    [Header("Background Music")]
    public AudioClip[] mainMenuMusic;
    public AudioClip[] gameplayMusic;
    public AudioClip[] shopMusic;
    
    [Header("Dog Sounds")]
    public AudioClip[] barkSounds;
    public AudioClip[] jumpSounds;
    public AudioClip[] happySounds;
    public AudioClip[] sleepySounds;
    
    [Header("Vehicle Sounds")]
    public AudioClip[] carSounds;
    public AudioClip[] scooterSounds;
    public AudioClip[] bicycleSounds;
    public AudioClip[] skateboardSounds;
    
    [Header("Environment Sounds")]
    public AudioClip[] birdSounds;
    public AudioClip[] windSounds;
    public AudioClip[] citySounds;
    public AudioClip[] parkSounds;
    
    [Header("UI Sounds")]
    public AudioClip buttonClick;
    public AudioClip collectItem;
    public AudioClip levelComplete;
    public AudioClip error;

    private AudioSource musicSource;
    private Dictionary<string, AudioSource> soundEffects;
    private float masterVolume = 1f;
    private float musicVolume = 1f;
    private float sfxVolume = 1f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudio();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializeAudio()
    {
        soundEffects = new Dictionary<string, AudioSource>();
        
        // Setup music source
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        
        // Load saved volume settings
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        
        UpdateVolumes();
    }

    public void PlayMusic(GameState.GameScene scene)
    {
        AudioClip[] musicArray = null;
        switch (scene)
        {
            case GameState.GameScene.MainMenu:
                musicArray = mainMenuMusic;
                break;
            case GameState.GameScene.Gameplay:
                musicArray = gameplayMusic;
                break;
            case GameState.GameScene.Shop:
                musicArray = shopMusic;
                break;
        }

        if (musicArray != null && musicArray.Length > 0)
        {
            AudioClip newTrack = musicArray[Random.Range(0, musicArray.Length)];
            if (musicSource.clip != newTrack)
            {
                musicSource.clip = newTrack;
                musicSource.Play();
            }
        }
    }

    public void PlayDogSound(string type)
    {
        AudioClip[] soundArray = null;
        switch (type.ToLower())
        {
            case "bark":
                soundArray = barkSounds;
                break;
            case "jump":
                soundArray = jumpSounds;
                break;
            case "happy":
                soundArray = happySounds;
                break;
            case "sleepy":
                soundArray = sleepySounds;
                break;
        }

        if (soundArray != null && soundArray.Length > 0)
        {
            PlayOneShot(soundArray[Random.Range(0, soundArray.Length)], "dogSound");
        }
    }

    public void PlayVehicleSound(VehicleController.VehicleType type)
    {
        AudioClip[] soundArray = null;
        switch (type)
        {
            case VehicleController.VehicleType.Car:
                soundArray = carSounds;
                break;
            case VehicleController.VehicleType.Scooter:
                soundArray = scooterSounds;
                break;
            case VehicleController.VehicleType.Bicycle:
                soundArray = bicycleSounds;
                break;
            case VehicleController.VehicleType.Skateboard:
                soundArray = skateboardSounds;
                break;
        }

        if (soundArray != null && soundArray.Length > 0)
        {
            PlayOneShot(soundArray[Random.Range(0, soundArray.Length)], "vehicleSound");
        }
    }

    public void PlayEnvironmentSound(string type)
    {
        AudioClip[] soundArray = null;
        switch (type.ToLower())
        {
            case "birds":
                soundArray = birdSounds;
                break;
            case "wind":
                soundArray = windSounds;
                break;
            case "city":
                soundArray = citySounds;
                break;
            case "park":
                soundArray = parkSounds;
                break;
        }

        if (soundArray != null && soundArray.Length > 0)
        {
            PlayOneShot(soundArray[Random.Range(0, soundArray.Length)], "environmentSound");
        }
    }

    public void PlayUISound(string type)
    {
        AudioClip clip = null;
        switch (type.ToLower())
        {
            case "click":
                clip = buttonClick;
                break;
            case "collect":
                clip = collectItem;
                break;
            case "complete":
                clip = levelComplete;
                break;
            case "error":
                clip = error;
                break;
        }

        if (clip != null)
        {
            PlayOneShot(clip, "uiSound");
        }
    }

    private void PlayOneShot(AudioClip clip, string sourceKey)
    {
        if (!soundEffects.ContainsKey(sourceKey))
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            soundEffects.Add(sourceKey, source);
        }

        AudioSource audioSource = soundEffects[sourceKey];
        audioSource.volume = sfxVolume * masterVolume;
        audioSource.PlayOneShot(clip);
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = volume;
        PlayerPrefs.SetFloat("MasterVolume", volume);
        UpdateVolumes();
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        PlayerPrefs.SetFloat("MusicVolume", volume);
        UpdateVolumes();
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        PlayerPrefs.SetFloat("SFXVolume", volume);
        UpdateVolumes();
    }

    private void UpdateVolumes()
    {
        musicSource.volume = musicVolume * masterVolume;
        
        foreach (var source in soundEffects.Values)
        {
            source.volume = sfxVolume * masterVolume;
        }
    }
}
