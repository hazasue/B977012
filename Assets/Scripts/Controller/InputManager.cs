using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private static InputManager instance;

    void Awake()
    {
        instance = this;
    }
    
    // Update is called once per frame
    void Update()
    {
        applyKeyInput();
    }

    private void applyKeyInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (UIManager.GetInstance().CheckPauseScreenActivate())
            {
                GameManager.GetInstance().ResumeGame();
                UIManager.GetInstance().ActivatePauseScreen(false);
            }
            else
            {
                GameManager.GetInstance().PauseGame();
                UIManager.GetInstance().ActivatePauseScreen(true);
            }
        }
    }

    public static InputManager GetInstance()
    {
        if (instance != null) return instance;
        instance = FindObjectOfType<InputManager>();
        if (instance == null) Debug.Log("There's no active InputManager object");
        return instance;
    }
}
