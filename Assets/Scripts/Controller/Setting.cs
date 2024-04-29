using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Setting : MonoBehaviour
{
    private static Setting instance;

    private static int[] DEFAULT_SCREEN_RESOLUTION_SMALL = new int[2]{ 1280, 720 };
    private static int[] DEFAULT_SCREEN_RESOLUTION_NORMAL = new int[2]{ 1920, 1080 };
    private static int[] DEFAULT_SCREEN_RESOLUTION_BIG = new int[2]{ 2560, 1440 };
    private const int DEFAULT_SCREEN_RESOLUTION_GAP_X = 640;
    private const int DEFAULT_SCREEN_RESOLUTION_GAP_Y = 360;
    private const bool DEFAULT_FULL_SCREEN_STATE = true;
    private const bool DEFAULT_DAMAGE_SHOW_STATE = true;
    private const float DEFAULT_SOUND_VOLUME = 0.2f;

    private const string DEFAULT_NAME_RESOLUTION = "RESOLUTION";
    private const string DEFAULT_NAME_BGM = "BGM";
    private const string DEFAULT_NAME_SFX = "SFX";
    private const string DEFAULT_NAME_FULL_SCREEN = "FULLSCREEN";
    private const string DEFAULT_NAME_SHOW_DAMAGE = "SHOWDAMAGE";
    
    public TMP_Text resolutionText;
    public Toggle fullScreenToggle;
    public Slider bgmSlider;
    public Slider sfxSlider;
    public Toggle showDamageToggle;

    public float resolutionMagnification;

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
                new SettingInfo(DEFAULT_SCREEN_RESOLUTION_NORMAL, DEFAULT_FULL_SCREEN_STATE, DEFAULT_SOUND_VOLUME,
                    DEFAULT_SOUND_VOLUME, DEFAULT_DAMAGE_SHOW_STATE);
            JsonManager.CreateJsonFile(JsonManager.DEFAULT_SETTING_DATA_NAME, tempInfo);
            settingInfo = tempInfo;
        }

        resolutionMagnification = 0f;
        
        fullScreenToggle.isOn = settingInfo.fullScreen;
        bgmSlider.value = settingInfo.volumeBgm;
        sfxSlider.value = settingInfo.volumeSfx;
        showDamageToggle.isOn = settingInfo.showDamage;

        ApplySetting(DEFAULT_NAME_RESOLUTION);
        ApplySetting(DEFAULT_NAME_BGM);
        ApplySetting(DEFAULT_NAME_SFX);
        ApplySetting(DEFAULT_NAME_SHOW_DAMAGE);

        instance = this;
    }

    public static Setting GetInstance()
    {
        if (instance != null) return instance;
        instance = FindObjectOfType<Setting>();
        if (instance == null) Debug.Log("There's no active Setting.");
        return instance;
    }

    public void ToggleMagnificationPositive(bool positive)
    {
        if (positive) 
            resolutionMagnification = 1f;
        else
        {
            resolutionMagnification = -1f;
        }
    }
    
    public void ChangeSetting(string target)
    {
        switch (target)
        {
            case DEFAULT_NAME_RESOLUTION:
                if (resolutionMagnification == 0f) return;
                
                if (resolutionMagnification > 0f)
                {
                    if(settingInfo.screenResolution[0] >= DEFAULT_SCREEN_RESOLUTION_BIG[0])
                        settingInfo.screenResolution = (int[])DEFAULT_SCREEN_RESOLUTION_SMALL.Clone();
                    else
                    {
                        settingInfo.screenResolution[0] += DEFAULT_SCREEN_RESOLUTION_GAP_X;
                        settingInfo.screenResolution[1] += DEFAULT_SCREEN_RESOLUTION_GAP_Y;
                    }
                }
                else
                {
                    if(settingInfo.screenResolution[0] <= DEFAULT_SCREEN_RESOLUTION_SMALL[0])
                        settingInfo.screenResolution = (int[])DEFAULT_SCREEN_RESOLUTION_BIG.Clone();
                    else
                    {
                        settingInfo.screenResolution[0] -= DEFAULT_SCREEN_RESOLUTION_GAP_X;
                        settingInfo.screenResolution[1] -= DEFAULT_SCREEN_RESOLUTION_GAP_Y;
                    }
                }

                Debug.Log(DEFAULT_SCREEN_RESOLUTION_SMALL[0]);
                resolutionText.text = $"{settingInfo.screenResolution[0]} * {settingInfo.screenResolution[1]}";
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
            
            case DEFAULT_NAME_SHOW_DAMAGE:
                settingInfo.showDamage = showDamageToggle.isOn;
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
                resolutionText.text = $"{settingInfo.screenResolution[0]} * {settingInfo.screenResolution[1]}";
                break;
            
            case DEFAULT_NAME_BGM:
                SoundManager.GetInstance().audioSourceBgm.volume = settingInfo.volumeBgm;
                break;
            
            case DEFAULT_NAME_SFX:
                SoundManager.GetInstance().audioSourceSfx.volume = settingInfo.volumeSfx;
                SoundManager.GetInstance().UpdateSfxVolumes();
                break;
            
            case DEFAULT_NAME_SHOW_DAMAGE:
                if (SceneManager.GetActiveScene().name == "InGame")
                {
                    UIManager.GetInstance().ShowDamage(settingInfo.showDamage);
                }

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
