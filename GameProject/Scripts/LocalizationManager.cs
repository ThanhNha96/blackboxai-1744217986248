using UnityEngine;
using System.Collections.Generic;
using System;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance { get; private set; }
    
    public enum Language
    {
        English,
        Vietnamese,
        Chinese,
        Korean,
        Japanese
    }

    private Dictionary<string, Dictionary<Language, string>> localizedText;
    public Language currentLanguage = Language.English;

    public event Action OnLanguageChanged;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeLocalization();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializeLocalization()
    {
        localizedText = new Dictionary<string, Dictionary<Language, string>>();
        LoadLanguageData();
        
        // Load saved language preference
        if (PlayerPrefs.HasKey("Language"))
        {
            currentLanguage = (Language)PlayerPrefs.GetInt("Language");
        }
    }

    public void SetLanguage(Language language)
    {
        currentLanguage = language;
        PlayerPrefs.SetInt("Language", (int)language);
        PlayerPrefs.Save();
        OnLanguageChanged?.Invoke();
    }

    public string GetLocalizedText(string key)
    {
        if (localizedText.ContainsKey(key) && localizedText[key].ContainsKey(currentLanguage))
        {
            return localizedText[key][currentLanguage];
        }
        return $"[{key}]";
    }

    private void LoadLanguageData()
    {
        // Main Menu
        AddLocalization("START_GAME", new Dictionary<Language, string>
        {
            {Language.English, "Start Game"},
            {Language.Vietnamese, "Bắt Đầu"},
            {Language.Chinese, "开始游戏"},
            {Language.Korean, "게임 시작"},
            {Language.Japanese, "ゲームスタート"}
        });

        AddLocalization("CONTINUE", new Dictionary<Language, string>
        {
            {Language.English, "Continue"},
            {Language.Vietnamese, "Tiếp Tục"},
            {Language.Chinese, "继续"},
            {Language.Korean, "계속하기"},
            {Language.Japanese, "続ける"}
        });

        AddLocalization("CUSTOMIZE", new Dictionary<Language, string>
        {
            {Language.English, "Customize"},
            {Language.Vietnamese, "Tùy Chỉnh"},
            {Language.Chinese, "自定义"},
            {Language.Korean, "커스터마이즈"},
            {Language.Japanese, "カスタマイズ"}
        });

        AddLocalization("COLLECTION", new Dictionary<Language, string>
        {
            {Language.English, "Collection"},
            {Language.Vietnamese, "Bộ Sưu Tập"},
            {Language.Chinese, "收藏品"},
            {Language.Korean, "컬렉션"},
            {Language.Japanese, "コレクション"}
        });

        AddLocalization("SETTINGS", new Dictionary<Language, string>
        {
            {Language.English, "Settings"},
            {Language.Vietnamese, "Cài Đặt"},
            {Language.Chinese, "设置"},
            {Language.Korean, "설정"},
            {Language.Japanese, "設定"}
        });

        // Settings
        AddLocalization("LANGUAGE", new Dictionary<Language, string>
        {
            {Language.English, "Language"},
            {Language.Vietnamese, "Ngôn Ngữ"},
            {Language.Chinese, "语言"},
            {Language.Korean, "언어"},
            {Language.Japanese, "言語"}
        });

        AddLocalization("BGM_VOLUME", new Dictionary<Language, string>
        {
            {Language.English, "BGM Volume"},
            {Language.Vietnamese, "Âm Lượng Nhạc"},
            {Language.Chinese, "背景音乐音量"},
            {Language.Korean, "배경음악 볼륨"},
            {Language.Japanese, "BGM音量"}
        });

        AddLocalization("SFX_VOLUME", new Dictionary<Language, string>
        {
            {Language.English, "SFX Volume"},
            {Language.Vietnamese, "Âm Lượng Hiệu Ứng"},
            {Language.Chinese, "音效音量"},
            {Language.Korean, "효과음 볼륨"},
            {Language.Japanese, "効果音量"}
        });

        // Game
        AddLocalization("LEVEL", new Dictionary<Language, string>
        {
            {Language.English, "Level"},
            {Language.Vietnamese, "Cấp Độ"},
            {Language.Chinese, "关卡"},
            {Language.Korean, "레벨"},
            {Language.Japanese, "レベル"}
        });

        AddLocalization("SCORE", new Dictionary<Language, string>
        {
            {Language.English, "Score"},
            {Language.Vietnamese, "Điểm"},
            {Language.Chinese, "分数"},
            {Language.Korean, "점수"},
            {Language.Japanese, "スコア"}
        });

        AddLocalization("PAUSE", new Dictionary<Language, string>
        {
            {Language.English, "Pause"},
            {Language.Vietnamese, "Tạm Dừng"},
            {Language.Chinese, "暂停"},
            {Language.Korean, "일시정지"},
            {Language.Japanese, "ポーズ"}
        });

        // Messages
        AddLocalization("LEVEL_COMPLETE", new Dictionary<Language, string>
        {
            {Language.English, "Level Complete!"},
            {Language.Vietnamese, "Hoàn Thành Cấp Độ!"},
            {Language.Chinese, "关卡完成！"},
            {Language.Korean, "레벨 완료!"},
            {Language.Japanese, "レベルクリア！"}
        });

        AddLocalization("GAME_SAVED", new Dictionary<Language, string>
        {
            {Language.English, "Game Saved"},
            {Language.Vietnamese, "Đã Lưu Game"},
            {Language.Chinese, "游戏已保存"},
            {Language.Korean, "게임 저장됨"},
            {Language.Japanese, "ゲームをセーブしました"}
        });
    }

    private void AddLocalization(string key, Dictionary<Language, string> translations)
    {
        if (!localizedText.ContainsKey(key))
        {
            localizedText.Add(key, translations);
        }
    }
}
