using UnityEngine;
using System.Collections;

public class TimeSystem : MonoBehaviour
{
    public enum TimeOfDay
    {
        Day,
        Night
    }

    [Header("Time Settings")]
    public float dayDuration = 300f; // Duration of a day in seconds
    public float nightDuration = 180f; // Duration of a night in seconds
    public TimeOfDay currentTimeOfDay;

    [Header("UI References")]
    public GameObject dayNightUI;
    public TMPro.TextMeshProUGUI timeText;

    private float timeElapsed;

    void Start()
    {
        currentTimeOfDay = TimeOfDay.Day;
        timeElapsed = 0f;
        UpdateTimeText();
        StartCoroutine(TimeCycle());
    }

    IEnumerator TimeCycle()
    {
        while (true)
        {
            if (currentTimeOfDay == TimeOfDay.Day)
            {
                timeElapsed += Time.deltaTime;
                if (timeElapsed >= dayDuration)
                {
                    currentTimeOfDay = TimeOfDay.Night;
                    timeElapsed = 0f;
                    UpdateTimeText();
                    // Trigger any events for night transition
                    OnNightStart();
                }
            }
            else
            {
                timeElapsed += Time.deltaTime;
                if (timeElapsed >= nightDuration)
                {
                    currentTimeOfDay = TimeOfDay.Day;
                    timeElapsed = 0f;
                    UpdateTimeText();
                    // Trigger any events for day transition
                    OnDayStart();
                }
            }
            yield return null;
        }
    }

    void UpdateTimeText()
    {
        string timeString = currentTimeOfDay == TimeOfDay.Day ? "Ngày" : "Đêm";
        timeText.text = $"Thời gian: {timeString}";
    }

    public void SkipToNextDay()
    {
        if (currentTimeOfDay == TimeOfDay.Day)
        {
            currentTimeOfDay = TimeOfDay.Night;
            timeElapsed = 0f;
            UpdateTimeText();
            OnNightStart();
        }
        else
        {
            currentTimeOfDay = TimeOfDay.Day;
            timeElapsed = 0f;
            UpdateTimeText();
            OnDayStart();
        }
    }

    void OnNightStart()
    {
        // Implement any logic needed when night starts
        // e.g., spawn more NPCs, change lighting, etc.
        Debug.Log("Night has started. Time to steal dogs!");
    }

    void OnDayStart()
    {
        // Implement any logic needed when day starts
        // e.g., reset NPC positions, change lighting, etc.
        Debug.Log("Day has started. Time to plan your next move!");
    }
}
