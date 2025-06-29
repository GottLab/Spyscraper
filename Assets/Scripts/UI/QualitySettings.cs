using System;
using UnityEngine;


public class QualitySettings : MonoBehaviour
{
    /*
        Manager that implements the quality changing methods
    */

    public enum GameQuality
    {
        // the low/medium/high mapping follows the QualitySettings interface
        Low = 0,
        Medium = 1,
        High = 2
    }

    public Action OnSetQualityLow;
    public Action OnSetQualityMedium;
    public Action OnSetQualityHigh;


    public static string QUALITY_KEY = "game_quality";

    void Start()
    {
        GameQuality preferred = PreferredQuality();
        ChangeToPreferredSettings(preferred);
    }

    public void SetQualityLow()
    {
        UnityEngine.QualitySettings.SetQualityLevel((int) GameQuality.Low);
        PlayerPrefs.SetInt(QUALITY_KEY, (int) GameQuality.Low);
        OnSetQualityLow?.Invoke();
        Debug.Log("Quality set to low");
    }

    public void SetQualityMedium()
    {
        UnityEngine.QualitySettings.SetQualityLevel((int) GameQuality.Medium);
        PlayerPrefs.SetInt(QUALITY_KEY, (int) GameQuality.Medium);
        OnSetQualityMedium?.Invoke();
        Debug.Log("Quality set to medium");
    }

    public void SetQualityHigh()
    {
        UnityEngine.QualitySettings.SetQualityLevel((int) GameQuality.High);
        PlayerPrefs.SetInt(QUALITY_KEY, (int) GameQuality.High);
        OnSetQualityHigh?.Invoke();
        Debug.Log("Quality set to high");
    }

    private GameQuality PreferredQuality()
    {
        return (GameQuality) PlayerPrefs.GetInt(QUALITY_KEY, (int) GameQuality.Medium);
    }

    private void ChangeToPreferredSettings(GameQuality quality)
    {
        switch (quality)
        {
            case GameQuality.Low:
                SetQualityLow();
                break;
            case GameQuality.High:
                SetQualityHigh();
                break;
            default:
                SetQualityMedium();
                break;
        }
    }
}