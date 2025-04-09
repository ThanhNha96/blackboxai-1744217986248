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
