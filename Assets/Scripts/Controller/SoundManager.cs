using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;
    
    private const string DEFAULT_BASIC_NAME = "basic";
    private const string DEFAULT_STAGE_NAME = "stage";    
    private const string DEFAULT_BOSS_STAGE_NAME = "bossStage";
    private const string DEFAULT_BOSS_WARNING_NAME = "bossWarning";
    private const string DEFAULT_SHOP_NAME = "shop";
    private const string DEFAULT_FAIL_NAME = "fail";
    private const string DEFAULT_CLEAR_NAME = "clear";
    
    public AudioSource audioSourceBgm;
    public AudioSource audioSourceSfx;

    public AudioClip basicBgm;
    public AudioClip stageBgm1Night;
    public AudioClip stageBgm1Noon;
    public AudioClip stageBgm2;
    public AudioClip stageBgm3;
    public AudioClip bossStageBgm;
    public AudioClip bossWarningBgm;
    public AudioClip shopBgm;
    public AudioClip failBgm;
    public AudioClip clearBgm;

    private List<AudioSource> sfxs;
    
    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(this);

        sfxs = new List<AudioSource>();

        audioSourceBgm.clip = basicBgm;
        audioSourceBgm.Play();
    }

    public static SoundManager GetInstance()
    {
        if (instance != null) return instance;
        instance = FindObjectOfType<SoundManager>();
        if (instance == null) Debug.Log("There's no active SoundManager object");
        return instance;
    }

    public void ChangeBGM(string name, bool night = false)
    {
        bool isSameBgm = false;
        switch (name)
        {
            case DEFAULT_BASIC_NAME:
                if(audioSourceBgm.clip != basicBgm) audioSourceBgm.clip = basicBgm;
                else isSameBgm = true;
                break;
            
            case DEFAULT_STAGE_NAME + "1":
                if(night && audioSourceBgm.clip != stageBgm1Night) audioSourceBgm.clip = stageBgm1Night;
                else if (!night && audioSourceBgm.clip != stageBgm1Noon) {
                    audioSourceBgm.clip = stageBgm1Noon;
                }
                else isSameBgm = true;
                break;
            
            case DEFAULT_STAGE_NAME + "2":
                if(audioSourceBgm.clip != stageBgm2) audioSourceBgm.clip = stageBgm2;
                else isSameBgm = true;
                break;
            
            case DEFAULT_STAGE_NAME + "3":
                if(audioSourceBgm.clip != stageBgm3) audioSourceBgm.clip = stageBgm3;
                else isSameBgm = true;
                break;

            case DEFAULT_BOSS_STAGE_NAME:
                if(audioSourceBgm.clip != bossStageBgm) audioSourceBgm.clip = bossStageBgm;
                else isSameBgm = true;
                break;

            case DEFAULT_BOSS_WARNING_NAME:
                if(audioSourceBgm.clip != bossWarningBgm) audioSourceBgm.clip = bossWarningBgm;
                else isSameBgm = true;
                break;

            case DEFAULT_SHOP_NAME:
                if(audioSourceBgm.clip != shopBgm) audioSourceBgm.clip = shopBgm;
                else isSameBgm = true;
                break;

            case DEFAULT_FAIL_NAME:
                if(audioSourceBgm.clip != failBgm) audioSourceBgm.clip = failBgm;
                else isSameBgm = true;
                break;

            case DEFAULT_CLEAR_NAME:
                if(audioSourceBgm.clip != clearBgm) audioSourceBgm.clip = clearBgm;
                else isSameBgm = true;
                break;
            
            default:
                Debug.Log("No BGM Name matched.");
                break;
        }

        if(!isSameBgm) audioSourceBgm.Play();
    }

    public void AddToSfxList(AudioSource audioSource)
    {
        sfxs.Add(audioSource);
    }

    public void UpdateSfxVolumes() {
        for (int i = sfxs.Count - 1; i >= 0; i--) {
                if(sfxs[i] == null) sfxs.RemoveAt(i);
        }

        foreach(AudioSource audioSource in sfxs) {
            audioSource.volume = audioSourceSfx.volume;
        }
    }
}
