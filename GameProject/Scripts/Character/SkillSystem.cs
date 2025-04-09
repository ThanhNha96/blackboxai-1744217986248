using UnityEngine;
using System.Collections.Generic;

public class SkillSystem : MonoBehaviour
{
    [System.Serializable]
    public class Skill
    {
        public string id;
        public string nameKey;
        public string descriptionKey;
        public SkillType type;
        public int level;
        public int maxLevel = 5;
        public float baseValue;
        public float valuePerLevel;
        public int pointsToUnlock;
        public bool isUnlocked;
        public Sprite icon;
    }

    public enum SkillType
    {
        Combat,     // Đánh đấm
        Stealth,    // Ẩn nấp
        Movement,   // Di chuyển
        Social,     // Giao tiếp
        Special     // Đặc biệt
    }

    [Header("Player Stats")]
    public int level = 1;
    public int experience = 0;
    public int skillPoints = 0;
    public int baseExpToLevel = 1000;

    [Header("Skills")]
    public List<Skill> combatSkills = new List<Skill>
    {
        new Skill {
            id = "punch_mastery",
            nameKey = "SKILL_PUNCH_MASTERY",
            type = SkillType.Combat,
            baseValue = 10f,    // Sát thương cơ bản
            valuePerLevel = 5f,  // Tăng sát thương mỗi cấp
            pointsToUnlock = 1
        },
        new Skill {
            id = "kick_mastery",
            nameKey = "SKILL_KICK_MASTERY",
            type = SkillType.Combat,
            baseValue = 15f,
            valuePerLevel = 7f,
            pointsToUnlock = 2
        },
        new Skill {
            id = "combo_mastery",
            nameKey = "SKILL_COMBO_MASTERY",
            type = SkillType.Combat,
            baseValue = 0.5f,   // Tăng sát thương combo
            valuePerLevel = 0.1f,
            pointsToUnlock = 3
        }
    };

    public List<Skill> stealthSkills = new List<Skill>
    {
        new Skill {
            id = "stealth_movement",
            nameKey = "SKILL_STEALTH_MOVEMENT",
            type = SkillType.Stealth,
            baseValue = 0.7f,   // Độ ồn khi di chuyển
            valuePerLevel = -0.1f, // Giảm độ ồn mỗi cấp
            pointsToUnlock = 2
        },
        new Skill {
            id = "detection_range",
            nameKey = "SKILL_DETECTION_RANGE",
            type = SkillType.Stealth,
            baseValue = 10f,    // Phạm vi phát hiện
            valuePerLevel = 2f,
            pointsToUnlock = 2
        }
    };

    public List<Skill> movementSkills = new List<Skill>
    {
        new Skill {
            id = "sprint_speed",
            nameKey = "SKILL_SPRINT_SPEED",
            type = SkillType.Movement,
            baseValue = 5f,     // Tốc độ chạy
            valuePerLevel = 1f,
            pointsToUnlock = 1
        },
        new Skill {
            id = "stamina",
            nameKey = "SKILL_STAMINA",
            type = SkillType.Movement,
            baseValue = 100f,   // Thể lực tối đa
            valuePerLevel = 20f,
            pointsToUnlock = 2
        }
    };

    public List<Skill> socialSkills = new List<Skill>
    {
        new Skill {
            id = "persuasion",
            nameKey = "SKILL_PERSUASION",
            type = SkillType.Social,
            baseValue = 0.1f,   // Tăng độ tin cậy
            valuePerLevel = 0.05f,
            pointsToUnlock = 2
        },
        new Skill {
            id = "bargaining",
            nameKey = "SKILL_BARGAINING",
            type = SkillType.Social,
            baseValue = 0f,     // Giảm giá mua hàng
            valuePerLevel = 0.05f, // 5% mỗi cấp
            pointsToUnlock = 2
        }
    };

    public List<Skill> specialSkills = new List<Skill>
    {
        new Skill {
            id = "taser_mastery",
            nameKey = "SKILL_TASER_MASTERY",
            type = SkillType.Special,
            baseValue = 3f,     // Thời gian choáng
            valuePerLevel = 0.5f,
            pointsToUnlock = 3
        },
        new Skill {
            id = "dog_whisperer",
            nameKey = "SKILL_DOG_WHISPERER",
            type = SkillType.Special,
            baseValue = 0.2f,   // Giảm chống cự của chó
            valuePerLevel = 0.1f,
            pointsToUnlock = 3
        }
    };

    private Dictionary<string, Skill> skillsById = new Dictionary<string, Skill>();

    void Start()
    {
        InitializeSkills();
    }

    void InitializeSkills()
    {
        // Add all skills to dictionary
        foreach (var skill in combatSkills) skillsById[skill.id] = skill;
        foreach (var skill in stealthSkills) skillsById[skill.id] = skill;
        foreach (var skill in movementSkills) skillsById[skill.id] = skill;
        foreach (var skill in socialSkills) skillsById[skill.id] = skill;
        foreach (var skill in specialSkills) skillsById[skill.id] = skill;
    }

    public void AddExperience(int amount)
    {
        experience += amount;
        CheckLevelUp();
    }

    void CheckLevelUp()
    {
        int expNeeded = baseExpToLevel * level;
        while (experience >= expNeeded)
        {
            experience -= expNeeded;
            LevelUp();
            expNeeded = baseExpToLevel * level;
        }
    }

    void LevelUp()
    {
        level++;
        skillPoints += 2; // 2 điểm kỹ năng mỗi cấp
        
        UIManager.Instance.ShowLevelUp(level, skillPoints);
        AudioManager.Instance.PlaySFX("levelUp");
    }

    public bool UpgradeSkill(string skillId)
    {
        if (!skillsById.ContainsKey(skillId)) return false;

        Skill skill = skillsById[skillId];
        
        if (skill.level >= skill.maxLevel) return false;
        
        if (!skill.isUnlocked)
        {
            if (skillPoints >= skill.pointsToUnlock)
            {
                skillPoints -= skill.pointsToUnlock;
                skill.isUnlocked = true;
                skill.level = 1;
                
                OnSkillUnlocked(skill);
                return true;
            }
        }
        else if (skillPoints > 0)
        {
            skillPoints--;
            skill.level++;
            
            OnSkillUpgraded(skill);
            return true;
        }
        
        return false;
    }

    void OnSkillUnlocked(Skill skill)
    {
        string message = LocalizationManager.Instance.GetLocalizedText("SKILL_UNLOCKED")
            .Replace("{skill}", LocalizationManager.Instance.GetLocalizedText(skill.nameKey));
        
        UIManager.Instance.ShowMessage(message);
        AudioManager.Instance.PlaySFX("skillUnlock");
        
        ApplySkillEffects(skill);
    }

    void OnSkillUpgraded(Skill skill)
    {
        string message = LocalizationManager.Instance.GetLocalizedText("SKILL_UPGRADED")
            .Replace("{skill}", LocalizationManager.Instance.GetLocalizedText(skill.nameKey))
            .Replace("{level}", skill.level.ToString());
        
        UIManager.Instance.ShowMessage(message);
        AudioManager.Instance.PlaySFX("skillUpgrade");
        
        ApplySkillEffects(skill);
    }

    void ApplySkillEffects(Skill skill)
    {
        float value = skill.baseValue + (skill.level - 1) * skill.valuePerLevel;
        
        switch (skill.id)
        {
            case "punch_mastery":
                GetComponent<MouseCombatSystem>().punchDamage = value;
                break;
            case "kick_mastery":
                GetComponent<MouseCombatSystem>().kickDamage = value;
                break;
            case "combo_mastery":
                GetComponent<MouseCombatSystem>().comboTimeWindow = value;
                break;
            case "stealth_movement":
                // Apply to movement system
                break;
            case "detection_range":
                // Apply to stealth system
                break;
            case "sprint_speed":
                // Apply to movement system
                break;
            case "stamina":
                // Apply to stamina system
                break;
            case "taser_mastery":
                GetComponent<DogStunAndSteal>().stunDuration = value;
                break;
            // Add other skill effects
        }
    }

    public float GetSkillValue(string skillId)
    {
        if (skillsById.TryGetValue(skillId, out Skill skill))
        {
            if (!skill.isUnlocked) return 0f;
            return skill.baseValue + (skill.level - 1) * skill.valuePerLevel;
        }
        return 0f;
    }

    public List<Skill> GetSkillsByType(SkillType type)
    {
        switch (type)
        {
            case SkillType.Combat: return combatSkills;
            case SkillType.Stealth: return stealthSkills;
            case SkillType.Movement: return movementSkills;
            case SkillType.Social: return socialSkills;
            case SkillType.Special: return specialSkills;
            default: return new List<Skill>();
        }
    }

    public int GetAvailableSkillPoints()
    {
        return skillPoints;
    }

    public float GetLevelProgress()
    {
        return (float)experience / (baseExpToLevel * level);
    }
}
