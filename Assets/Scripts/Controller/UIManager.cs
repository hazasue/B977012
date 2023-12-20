using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;

    private const int DEFAULT_UI_COUNT = 3;
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
    public TMP_Text[] weaponLvs = new TMP_Text[DEFAULT_OPTION_COUNT];
    public TMP_Text[] weaponDescriptions = new TMP_Text[DEFAULT_OPTION_COUNT];

    public TMP_Text[] coinCount = new TMP_Text[DEFAULT_UI_COUNT];
    public TMP_Text[] enemyCount = new TMP_Text[DEFAULT_UI_COUNT];

    private GameManager mGameManager;
    private WeaponManager mWeaponManager;
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
        mGameManager = GameManager.GetInstance();
        mWeaponManager = WeaponManager.GetInstance();
        player = mGameManager.GetPlayer();
        
        playerHp.maxValue = player.GetMaxHp();
        playerHp.value = player.GetCurrentHp();

        playerExp.maxValue = player.GetLevelInfo().CheckRequiredExp();
        playerExp.value = player.GetLevelInfo().CheckCurrentExp();

        playerSkill.maxValue = player.GetInventory().GetSkill().GetDelay();
        playerSkill.value = 0;
        StartCoroutine(updateSkillBar());
    }

    private void updateTime()
    {
        time += Time.deltaTime;
        timeText.text = ((int)(time / 60)).ToString("D2") + " : " + ((int)(time % 60)).ToString("D2");
    }

    private IEnumerator updateSkillBar()
    {
        yield return new WaitForSeconds(Time.deltaTime);
        if (playerSkill.value >= playerSkill.maxValue) yield break;

        playerSkill.value += Time.deltaTime;
        StartCoroutine(updateSkillBar());
    }

    public void ResetSkillBar()
    {
        playerSkill.value = 0f;
        StartCoroutine(updateSkillBar());
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

    public void UpdateCoinCount(int value)
    {
        for (int i = 0; i < DEFAULT_UI_COUNT; i++)
        {
            coinCount[i].text = value.ToString();
        }
    }

    public void UpdateKilledEnemyCount(int count)
    {
        for (int i = 0; i < DEFAULT_UI_COUNT; i++)
        {
            enemyCount[i].text = count.ToString();
        }
    }

    public void ActivateClearScreen() { clearScreen.SetActive(true); }

    public void ActivateFailScreen() { failScreen.SetActive(true); }

    public void UpdateAugmentOptions(List<WeaponInfo> weaponInfos)
    {
        Weapon selectedWeapon;
        WeaponUpgradeInfo upgradeInfo;

        for (int i = 0; i < DEFAULT_OPTION_COUNT; i++)
        {
            if (i < weaponInfos.Count)
            {
                if (!player.GetInventory().GetWeapons().TryGetValue(weaponInfos[i].GetCode(), out selectedWeapon))
                    selectedWeapon = null;

                weaponNames[i].text = weaponInfos[i].GetName();

                if (!selectedWeapon)
                {
                    weaponLvs[i].text = "Lv. 1";
                    weaponDescriptions[i].text = weaponInfos[i].GetDescription();
                }
                else
                {
                    if(selectedWeapon.GetUpgradeCount() < WeaponManager.MAX_UPGRADE_COUNT)
                        weaponLvs[i].text = "Lv. " + (selectedWeapon.GetUpgradeCount() + 1);
                    else
                    {
                        weaponLvs[i].text = "Lv. MAX";
                    }

                    upgradeInfo = mWeaponManager.GetWeaponUpgradeInfo(weaponInfos[i].GetCode());
                    if(upgradeInfo.option1 == "delay")
                        weaponDescriptions[i].text = "Decrease " + upgradeInfo.option1 + ": " + upgradeInfo.value1;
                    else
                    {
                        weaponDescriptions[i].text = "Increase " + upgradeInfo.option1 + ": " + upgradeInfo.value1;
                    }

                }
            }
            else
            {
                weaponNames[i].text = DEFAULT_AUGMENT_COIN;
                weaponLvs[i].text = "";
                weaponDescriptions[i].text = "Gain " + WeaponManager.DEFAULT_COIN_VALUE + " Coins.";
            }
        }

        SetActiveAugmentScreen(true);
    }

    public void SelectOption(int index)
    {
        mWeaponManager.ReflectAugment(index);
    }
    
    public void SetActiveAugmentScreen(bool activeStatus)
    {
        augmentScreen.SetActive(activeStatus);

        if (activeStatus) mGameManager.PauseGame();
        else
        {
            mGameManager.ResumeGame();
        }
    }
}
