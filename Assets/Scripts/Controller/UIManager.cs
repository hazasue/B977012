using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;

    private static int DEFAULT_OPTION_COUNT = 4;

    private float time;

    public Slider playerHp;
    public Slider playerExp;

    public TMP_Text timeText;

    public GameObject augmentScreen;
    public TMP_Text[] weaponNames = new TMP_Text[DEFAULT_OPTION_COUNT];

    private Player player;
    
    // Start is called before the first frame update
    void Start()
    {
        init();
    }

    void Update()
    {
        updateTime();
    }

    public static UIManager GetInstance()
    {
        if (instance != null) return instance;
        instance = FindObjectOfType<UIManager>();
        if (instance == null) Debug.Log("There's no active UIManager object");
        return instance;
    }

    private void init()
    {
        instance = this;

        time = 0f;
        player = GameManager.GetInstance().GetPlayer();
        
        playerHp.maxValue = player.GetMaxHp();
        playerHp.value = player.GetCurrentHp();

        playerExp.maxValue = player.GetLevelInfo().CheckRequiredExp();
        playerExp.value = player.GetLevelInfo().CheckCurrentExp();
    }

    private void updateTime()
    {
        time += Time.deltaTime;
        timeText.text = ((int)(time / 60)).ToString("D2") + " : " + ((int)(time % 60)).ToString("D2");
    }

    public void UpdatePlayerCurrentStatus()
    {
        playerHp.value = player.GetCurrentHp();
        playerExp.value = player.GetLevelInfo().CheckCurrentExp();
    }

    public void UpdatePlayerMaxStatus()
    {
        playerHp.maxValue = player.GetMaxHp();
        playerExp.maxValue = player.GetLevelInfo().CheckRequiredExp();
    }

    public void UpdateAugmentOptions(List<WeaponInfo> weaponInfos)
    {
        for (int i = 0; i < DEFAULT_OPTION_COUNT; i++)
        {
            weaponNames[i].text = weaponInfos[i].GetName();
        }

        SetActiveAugmentScreen(true);
    }

    public void SelectOption(int index)
    {
        WeaponManager.GetInstance().ReflectAugment(index);
    }
    
    public void SetActiveAugmentScreen(bool activeStatus)
    {
        augmentScreen.SetActive(activeStatus);

        if (activeStatus) GameManager.GetInstance().PauseGame();
        else
        {
            GameManager.GetInstance().ResumeGame();
        }
    }
}
