using System;
using UnityEngine;
using UnityEngine.UI;


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


    public Toggle lowQualityToggle;
    public Toggle mediumQualityToggle;
    public Toggle highQualityToggle;

    public static string QUALITY_KEY = "game_quality";

    void Start()
    {
        GameQuality preferred = PreferredQuality();
        ChangeToPreferredSettings(preferred);

        lowQualityToggle.group.allowSwitchOff = false;
    }

    private Toggle GetToggle(GameQuality quality)
    {
        switch (quality)
        {
            case GameQuality.Low:
                return lowQualityToggle;
            case GameQuality.Medium:
                return mediumQualityToggle;
            default:
                return highQualityToggle;
        }
    }

    public void SetQualityLow(bool flag)
    {
        if (flag)
            ChangeToPreferredSettings(GameQuality.Low);
    }

    public void SetQualityMedium(bool flag)
    {
        if (flag)
            ChangeToPreferredSettings(GameQuality.Medium);
    }

    public void SetQualityHigh(bool flag)
    {
        if (flag)
            ChangeToPreferredSettings(GameQuality.High);
    }

    private GameQuality PreferredQuality()
    {
        return (GameQuality) PlayerPrefs.GetInt(QUALITY_KEY, (int) GameQuality.Medium);
    }


    private void ChangeToPreferredSettings(GameQuality quality)
    {
        UnityEngine.QualitySettings.SetQualityLevel((int) quality);
        PlayerPrefs.SetInt(QUALITY_KEY, (int) quality);
        GetToggle(quality).isOn = true;
        Debug.Log($"Quality set to {quality}");
    }
}