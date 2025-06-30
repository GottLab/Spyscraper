using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Linq;
using TMPro;

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


    public TMP_Dropdown qualityDropdown;

    public Toggle VSyncToggle;

    public Slider fpsSlider;

    public TextMeshProUGUI fpsLabel;

    public static string QUALITY_KEY = "game_quality";
    public static string VSYNC_KEY = "vsync";
    public static string FPS_KEY = "fps";

    void Start()
    {
        List<string> qualityNames = Enum
            .GetValues(typeof(GameQuality))
            .Cast<GameQuality>()
            .Select(q => q.ToString())
            .ToList();


        this.qualityDropdown.AddOptions(qualityNames);

        //game quality setup
        GameQuality preferred = PreferredQuality();
        SetGameQuality((int)preferred);

        qualityDropdown.onValueChanged.AddListener(SetGameQuality);

        //vSync
        this.VSyncToggle.isOn = HasVSync();
        this.VSyncToggle.onValueChanged.AddListener(this.SetVSync);
        this.SetVSync(HasVSync());


        
        this.SetFpsLimit(this.GetTargetFps());
        this.fpsSlider.onValueChanged.AddListener(this.SetFpsLimit);

    }

    void OnDestroy()
    {
        qualityDropdown.onValueChanged.RemoveListener(SetGameQuality);
        VSyncToggle.onValueChanged.RemoveListener(this.SetVSync);
        fpsSlider.onValueChanged.RemoveListener(this.SetFpsLimit);
    }

    private GameQuality PreferredQuality()
    {
        return (GameQuality)PlayerPrefs.GetInt(QUALITY_KEY, (int)GameQuality.Medium);
    }

    private void SetGameQuality(int quality)
    {
        UnityEngine.QualitySettings.SetQualityLevel((int)quality);
        PlayerPrefs.SetInt(QUALITY_KEY, (int)quality);
        this.qualityDropdown.value = (int)quality;
        this.qualityDropdown.RefreshShownValue();
        Debug.Log($"Quality set to {quality}");

        this.SetVSync(HasVSync());
    }


    private void SetVSync(bool isOn)
    {

        UnityEngine.QualitySettings.vSyncCount = isOn ? 1 : 0;
        if (isOn)
        {
            fpsSlider.interactable = false;
        }
        else
        {
            this.SetFpsLimit(GetTargetFps());
            fpsSlider.interactable = true;
        }

        PlayerPrefs.SetInt(VSYNC_KEY, isOn ? 1 : 0);
        Debug.Log($"VSync set to {UnityEngine.QualitySettings.vSyncCount}");

    }

    public bool HasVSync()
    {
        return PlayerPrefs.GetInt(VSYNC_KEY, 0) > 0;
    }


    private void SetFpsLimit(float value)
    {
        int fps = (int)value;

        bool unlimited = fps >= 200;

        fpsLabel.text = unlimited ? "Unlimited" : fps.ToString();
        fpsSlider.value = fps;

        Application.targetFrameRate = unlimited ? -1 : fps;

        PlayerPrefs.SetInt(FPS_KEY, fps);

        Debug.Log($"FPS target set to {fpsLabel.text}");
    }

    public int GetTargetFps()
    {
        return PlayerPrefs.GetInt(FPS_KEY, 60);
    }


}