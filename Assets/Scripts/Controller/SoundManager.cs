using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;
    
    private const string DEFAULT_BASIC_NAME = "basic";
    private const string DEFAULT_STAGE_NAME = "stage";    
    
    public AudioSource audioSourceBgm;
    public AudioSource audioSourceSfx;

    public AudioClip basicBgm;
    public AudioClip stageBgm1;
    public AudioClip stageBgm2;
    public AudioClip stageBgm3;
    
    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(this);

        audioSourceBgm.clip = basicBgm;
        audioSourceBgm.Play();
    }

    public static SoundManager GetInstance()
    {
        if (instance != null) return instance;
        instance = FindObjectOfType<SoundManager>();
        if (instance == null) Debug.Log("There's no active InputManager object");
        return instance;
    }

    public void ChangeBGM(string name)
    {
        switch (name)
        {
            case DEFAULT_BASIC_NAME:
                audioSourceBgm.clip = basicBgm;
                break;
            
            case DEFAULT_STAGE_NAME + "1":
                audioSourceBgm.clip = stageBgm1;
                break;
            
            case DEFAULT_STAGE_NAME + "2":
                audioSourceBgm.clip = stageBgm2;
                break;
            
            case DEFAULT_STAGE_NAME + "3":
                audioSourceBgm.clip = stageBgm3;
                break;
            
            default:
                Debug.Log("No BGM Name matched.");
                break;
            
        }
    }

    public void PlaySfx()
    {
        
    }
}
