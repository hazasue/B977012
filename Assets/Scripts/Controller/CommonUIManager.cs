using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CommonUIManager : MonoBehaviour
{
    private static CommonUIManager instance;
    
    public void MoveScene(string name)
    {
        switch (name)
        {
            case "Title":
            case "CharacterSelect":
            case "Lobby":
                SoundManager.GetInstance().ChangeBGM("basic");
                SceneManager.LoadScene(name);
                break;
            case "InGame":
                SceneManager.LoadScene(name);
                break;
            
            default:
                Debug.Log("Invalid scene name: " + name);
                break;
        }
    }
    
    public static CommonUIManager GetInstance()
    {
        if (instance != null) return instance;
        instance = FindObjectOfType<CommonUIManager>();
        if (instance == null) Debug.Log("There's no active CommonUIManager object");
        return instance;
    }
}
