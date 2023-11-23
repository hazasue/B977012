using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;

    private static int DEFAULT_OPTION_COUNT = 4;
    private static string DEFAULT_AUGMENT_COIN = "coin";

    private float time;

    public Slider playerHp;
    public Slider playerExp;
    public Slider playerSkill;

    public TMP_Text timeText;

    public GameObject clearScreen;
    public GameObject failScreen;
    
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
        updateSkillBar();
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

        playerSkill.maxValue = player.GetInventory().GetSkill().GetDelay();
        playerSkill.value = 0;
    }

    private void updateTime()
    {
        time += Time.deltaTime;
        timeText.text = ((int)(time / 60)).ToString("D2") + " : " + ((int)(time % 60)).ToString("D2");
    }

    private void updateSkillBar()
    {
        if (playerSkill.value >= playerSkill.maxValue) return;
        playerSkill.value += Time.deltaTime;
    }

    public void ResetSkillBar()
    {
        playerSkill.value = 0f;
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

    public void ActivateClearScreen() { clearScreen.SetActive(true); }

    public void ActivateFailScreen() { failScreen.SetActive(true); }

    public void UpdateAugmentOptions(List<WeaponInfo> weaponInfos)
    {
        for (int i = 0; i < DEFAULT_OPTION_COUNT; i++)
        {
            if (i < weaponInfos.Count)
                weaponNames[i].text = weaponInfos[i].GetName();
            else
            {
                weaponNames[i].text = DEFAULT_AUGMENT_COIN;
            }
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
