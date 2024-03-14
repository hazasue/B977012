using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Setting : MonoBehaviour
{
    private static Setting instance;
    
    private const int DEFAULT_SCREEN_RESOLUTION_X = 1920;
    private const int DEFAULT_SCREEN_RESOLUTION_Y = 1080;
    private const bool DEFAULT_FULL_SCREEN_STATE = true;
    private const bool DEFAULT_DAMAGE_SHOW_STATE = true;
    private const float DEFAULT_SOUND_VOLUME = 0.2f;

    private const string DEFAULT_NAME_RESOLUTION = "RESOLUTION";
    private const string DEFAULT_NAME_BGM = "BGM";
    private const string DEFAULT_NAME_SFX = "SFX";
    private const string DEFAULT_NAME_FULL_SCREEN = "FULLSCREEN";

    public Toggle fullScreenToggle;
    public Slider bgmSlider;
    public Slider sfxSlider;

    private SettingInfo settingInfo;

    void Start()
    {
        init();
    }

    private void init()
    {
        if (File.Exists(Application.dataPath + "/Data/" + JsonManager.DEFAULT_SETTING_DATA_NAME + ".json"))
            settingInfo = JsonManager.LoadJsonFile<SettingInfo>(JsonManager.DEFAULT_SETTING_DATA_NAME);
        else
        {
            SettingInfo tempInfo =
                new SettingInfo(new int[] { DEFAULT_SCREEN_RESOLUTION_X, DEFAULT_SCREEN_RESOLUTION_Y },
                    DEFAULT_FULL_SCREEN_STATE, DEFAULT_SOUND_VOLUME, DEFAULT_SOUND_VOLUME, DEFAULT_DAMAGE_SHOW_STATE);
            JsonManager.CreateJsonFile(JsonManager.DEFAULT_SETTING_DATA_NAME, tempInfo);
            settingInfo = tempInfo;
        }
        
        fullScreenToggle.isOn = settingInfo.fullScreen;
        bgmSlider.value = settingInfo.volumeBgm;
        sfxSlider.value = settingInfo.volumeSfx;

        ApplySetting(DEFAULT_NAME_RESOLUTION);
        ApplySetting(DEFAULT_NAME_BGM);
        ApplySetting(DEFAULT_NAME_SFX);

        instance = this;
    }

    public static Setting GetInstance()
    {
        if (instance != null) return instance;
        instance = FindObjectOfType<Setting>();
        if (instance == null) Debug.Log("There's no active Setting.");
        return instance;
    }

    public void ChangeSetting(string target)
    {
        switch (target)
        {
            case DEFAULT_NAME_RESOLUTION:
                break;
            
            case DEFAULT_NAME_FULL_SCREEN:
                settingInfo.fullScreen = fullScreenToggle.isOn;
                break;
            
            case DEFAULT_NAME_BGM:
                settingInfo.volumeBgm = bgmSlider.value;
                break;
            
            case DEFAULT_NAME_SFX:
                settingInfo.volumeSfx = sfxSlider.value;
                break;

            default:
                break;
        }
    }
    
    public void ApplySetting(string target)
    {
        switch (target)
        {
            case DEFAULT_NAME_RESOLUTION:
            case DEFAULT_NAME_FULL_SCREEN:
                Screen.SetResolution(settingInfo.screenResolution[0], settingInfo.screenResolution[1], settingInfo.fullScreen);
                break;
            
            case DEFAULT_NAME_BGM:
                SoundManager.GetInstance().audioSourceBgm.volume = settingInfo.volumeBgm;
                break;
            
            case DEFAULT_NAME_SFX:
                SoundManager.GetInstance().audioSourceSfx.volume = settingInfo.volumeSfx;
                break;

            default:
                break;
        }
    }

    public void SaveSettings()
    {
        JsonManager.CreateJsonFile(JsonManager.DEFAULT_SETTING_DATA_NAME, settingInfo);
    }
}
