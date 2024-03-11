using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;
    
    private const string DEFAULT_BASIC_NAME = "basic";
    private const string DEFAULT_STAGE_NAME = "stage";    
    
    private AudioSource audioSource;

    public AudioClip basicBgm;
    public AudioClip stageBgm1;
    public AudioClip stageBgm2;
    public AudioClip stageBgm3;
    
    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(this);

        audioSource = this.gameObject.GetComponent<AudioSource>();
        audioSource.clip = basicBgm;
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
                audioSource.clip = basicBgm;
                break;
            
            case DEFAULT_STAGE_NAME + "1":
                audioSource.clip = stageBgm1;
                break;
            
            case DEFAULT_STAGE_NAME + "2":
                audioSource.clip = stageBgm2;
                break;
            
            case DEFAULT_STAGE_NAME + "3":
                audioSource.clip = stageBgm3;
                break;
            
            default:
                Debug.Log("No BGM Name matched.");
                break;
            
        }
    }
}
