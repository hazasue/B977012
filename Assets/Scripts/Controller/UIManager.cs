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
    private static Vector3 DEFAULT_OPTION_POSITION = new Vector3(500f, 270f, 0f);
    private const float DEFAULT_OPTION_POSITION_GAP = 180f;

    private const string DEFAULT_PAUSE_SCREEN = "PAUSE";
    private const string DEFAULT_SETTING_SCREEN = "SETTINGS";
    private const string DEFAULT_EXIT_SCREEN = "EXIT";
    private const string DEFAULT_BOSS_WARNING_TEXT = "Boss Warning ";
    private const int DEFAULT_BOSS_WARNING_TEXT_LENGTH = 13;
    
    private const int DAMAGE_TEXT_COUNT = 800;
    private const float DAMAGE_TEXT_DURATION = 0.5f;
    private const float CONVERSION_CONSTANT_VALUE = 22.5f;
    private const float DEFAULT_SCREEN_HEIGHT = 1080f;
    private static float CONVERSTION_MULTIPLY_VALUE = 1f;

    private float time;

    public RectTransform canvas;
    
    public Slider playerHp;
    public Slider playerExp;
    public Slider playerSkill;
    private bool skillGageFull;

    public TMP_Text playerHpText;
    public GameObject skillText;
    public TMP_Text levelText;
    public TMP_Text timeText;

    public GameObject clearScreen;
    public GameObject failScreen;
    public GameObject pauseScreen;
    public GameObject settingScreen;
    public GameObject exitScreen;
    private Stack<GameObject> screens;

    public Image[] supplyImages = new Image[2];

    public GameObject bossWarningScreen;
    public TMP_Text bossWarningText;

    public GameObject optionBorder;
    
    public GameObject augmentScreen;
    public TMP_Text[] weaponNames = new TMP_Text[DEFAULT_OPTION_COUNT];
    public TMP_Text[] weaponLvs = new TMP_Text[DEFAULT_OPTION_COUNT];
    public TMP_Text[] weaponDescriptions = new TMP_Text[DEFAULT_OPTION_COUNT];

    public TMP_Text[] coinCount = new TMP_Text[DEFAULT_UI_COUNT];
    public TMP_Text[] enemyCount = new TMP_Text[DEFAULT_UI_COUNT];

    public TMP_Text damageText;
    public Transform damageTextTransform;
    private Queue<TMP_Text> allDamageTexts;
    private Dictionary<TMP_Text, Vector3> activatedTexts;
    private bool showDamage;

    public Transform playerAttackDirection;

    
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
        updateSkillBar();
        updateDamageTexts();
        applyKeyInput();

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
        
        playerHpText.text = $"{player.GetCurrentHp()} / {player.GetMaxHp()}";
        playerHp.maxValue = player.GetMaxHp();
        playerHp.value = player.GetCurrentHp();

        levelText.text = $"LV. {player.GetLevelInfo().GetLevel()}";
        playerExp.maxValue = player.GetLevelInfo().CheckRequiredExp();
        playerExp.value = player.GetLevelInfo().CheckCurrentExp();

        playerSkill.maxValue = player.GetInventory().GetSkill().GetDelay();
        playerSkill.value = 0;
        skillGageFull = false;

        CONVERSTION_MULTIPLY_VALUE = canvas.rect.height / DEFAULT_SCREEN_HEIGHT;

        allDamageTexts = new Queue<TMP_Text>();
        activatedTexts = new Dictionary<TMP_Text, Vector3>();
        TMP_Text tempText;
        for (int i = 0; i < DAMAGE_TEXT_COUNT; i++)
        {
            tempText = Instantiate(damageText, damageTextTransform, true);
            tempText.gameObject.SetActive(false);
            allDamageTexts.Enqueue(tempText);
        }

        screens = new Stack<GameObject>();
    }

    private void updateTime()
    {
        time += Time.deltaTime;
        timeText.text = ((int)(time / 60)).ToString("D2") + " : " + ((int)(time % 60)).ToString("D2");
    }

    private void updateSkillBar()
    {
        if (skillGageFull) return;
        playerSkill.value += Time.deltaTime;
        if (playerSkill.value >= playerSkill.maxValue)
        {
            skillText.SetActive(true);
            skillGageFull = true;
        }
    }

    private void updateDamageTexts()
    {
        Vector3 posDiff;
        foreach (KeyValuePair<TMP_Text, Vector3> data in activatedTexts)
        {
            posDiff = data.Value - GameManager.GetInstance().GetPlayer().transform.position;
            data.Key.transform.localPosition = convertPositionToRect(posDiff);
        }
    }

    public void ResetSkillBar()
    {
        playerSkill.value = 0f;
        skillText.SetActive(false);
        skillGageFull = false;
    }

    public void UpdatePlayerCurrentStatus()
    {
        levelText.text = $"LV. {player.GetLevelInfo().GetLevel()}";
        playerHpText.text = $"{player.GetCurrentHp()} / {player.GetMaxHp()}";
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
    
    private void applyKeyInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (screens.Count > 0)
                InactivateScreen();
            else
            {
                ActivateScreen(DEFAULT_PAUSE_SCREEN);
                AddToActivatedScreens(DEFAULT_PAUSE_SCREEN);
                GameManager.GetInstance().PauseGame();
            }
        }
    }

    public void ActivateScreen(string name)
    {
        if (screens.Count > 0) screens.Peek().SetActive(false);
        
        switch (name)
        {
            case DEFAULT_PAUSE_SCREEN:
                pauseScreen.SetActive(true);
                break;
            
            case DEFAULT_SETTING_SCREEN:
                settingScreen.SetActive(true);
                break;
            
            case DEFAULT_EXIT_SCREEN:
                exitScreen.SetActive(true);
                break;
            
            default:
                Debug.Log($"No any scene match with name ({name})");
                break;
        }
    }

    public void InactivateScreen()
    {
        GameObject screen = screens.Pop();
        screen.SetActive(false);
        if (screens.Count > 0) screens.Peek().SetActive(true);
        if(screen == pauseScreen) GameManager.GetInstance().ResumeGame();
    }

    public void AddToActivatedScreens(string name)
    {
        switch (name)
        {
            case DEFAULT_PAUSE_SCREEN:
                screens.Push(pauseScreen);
                break;
            
            case DEFAULT_SETTING_SCREEN:
                screens.Push(settingScreen);
                break;
            
            case DEFAULT_EXIT_SCREEN:
                screens.Push(exitScreen);
                break;
            
            default:
                Debug.Log($"No any scene match with name ({name})");
                break;
        }
    }

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
                    //weaponLvs[i].text = "Lv. 1";
                    weaponLvs[i].gameObject.SetActive(true);
                    weaponDescriptions[i].text = weaponInfos[i].GetDescription();
                }
                else
                {
                    weaponLvs[i].gameObject.SetActive(false);
                    if(selectedWeapon.GetUpgradeCount() < WeaponManager.MAX_UPGRADE_COUNT)
                    //    weaponLvs[i].text = "Lv. " + (selectedWeapon.GetUpgradeCount() + 1);
                    weaponNames[i].text += "  Lv. " + (selectedWeapon.GetUpgradeCount() + 1);
                    else
                    {
                        //weaponLvs[i].text = "Lv. MAX";
                        weaponNames[i].text += "  Lv. MAX";
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
        optionBorder.SetActive(false);
        augmentScreen.SetActive(activeStatus);

        if (activeStatus) mGameManager.PauseGame();
        else
        {
            mGameManager.ResumeGame();
        }
    }
    
    public void ShowDamageText(Vector3 pos, int damage)
    {
        if (!showDamage) return;

        TMP_Text damageText = allDamageTexts.Dequeue();
        allDamageTexts.Enqueue(damageText);

        Vector3 posDiff = pos - GameManager.GetInstance().GetPlayer().transform.position;
        damageText.gameObject.SetActive(true);
        damageText.transform.localPosition = convertPositionToRect(posDiff);
        damageText.text = damage.ToString();
        activatedTexts.Add(damageText, pos);
        StartCoroutine(moveDamageText(damageText, DAMAGE_TEXT_DURATION, posDiff));
        StartCoroutine(inactivateText(damageText, DAMAGE_TEXT_DURATION));
    }
    
    private Vector3 convertPositionToRect(Vector3 posDiff)
    {
        return new Vector3(posDiff.x * DEFAULT_SCREEN_HEIGHT / CONVERSION_CONSTANT_VALUE * CONVERSTION_MULTIPLY_VALUE,
            posDiff.z * DEFAULT_SCREEN_HEIGHT / CONVERSION_CONSTANT_VALUE * CONVERSTION_MULTIPLY_VALUE, 0f);
    }

    private IEnumerator inactivateText(TMP_Text text, float duration)
    {
        yield return new WaitForSeconds(duration);

        text.gameObject.SetActive(false);
        activatedTexts.Remove(text);
    }

    private IEnumerator moveDamageText(TMP_Text text, float duration, Vector3 direction) {   
        float time = 0f;
        Vector3 tempVector;
        Vector3 tempDirection = Vector3.zero;
        if (direction.x >= 0f) tempDirection.x += 2f;
        else {
            tempDirection.x -= 2f;
        }
        tempDirection.z += 2.4f;
        for (float i = 0f;  i <= duration;) {
            if(!activatedTexts.TryGetValue(text, out tempVector)) yield break;
            time = Time.deltaTime;
            i += time;
            activatedTexts[text] += tempDirection * time * 2f;
            tempDirection.z -= time * 10; 
            yield return new WaitForSeconds(time);
        }

    }

    public void HoverOption(int idx)
    {
        optionBorder.transform.localPosition = DEFAULT_OPTION_POSITION - new Vector3(0f, DEFAULT_OPTION_POSITION_GAP * idx, 0f);
    }

    public void ActivateOptionBorder()
    {
        if (optionBorder.activeSelf == true) return;
        
        optionBorder.SetActive(true);
    }

    public void InactivateOptionBorder()
    {
        if (optionBorder.activeSelf == false) return;
        
        optionBorder.SetActive(false);
    }

    public void ShowDamage(bool state)
    {
        showDamage = state;
    }

    public IEnumerator WarningBossStage(float delay, float duration){
        yield return new WaitForSeconds(delay);

        SoundManager.GetInstance().ChangeBGM("bossWarning", false);
        StartCoroutine(warningBossText(duration));
    }

    private IEnumerator warningBossText(float duration) {
        StartCoroutine(inactivateWarningText(duration));

        float delay;
        float textAddDelay = 0.033f;
        float timer = 0.033f;

        int textIdx = 0;
        Color textColor = bossWarningText.color;
        textColor.a = 0f;
        bool increaseAlphaValue = true;
        bossWarningScreen.SetActive(true);
        bossWarningText.text = "";
        bossWarningText.color = textColor;
        for (float i = duration; i >= 0f;){
            delay = Time.deltaTime;
            i -= delay;
            if(textIdx >= DEFAULT_BOSS_WARNING_TEXT_LENGTH) textIdx -= DEFAULT_BOSS_WARNING_TEXT_LENGTH;
            if(timer >= textAddDelay) {
                bossWarningText.text += DEFAULT_BOSS_WARNING_TEXT[textIdx++];
                timer = 0f;
            }
            timer += delay;

            if(increaseAlphaValue) {
                textColor.a += delay;
                if(textColor.a >= 1f) increaseAlphaValue = false;
            }
            else{
                textColor.a -= delay;
                if(textColor.a <= 0f) increaseAlphaValue = true;
            }

            bossWarningText.color = textColor;
            
            if(bossWarningScreen.activeSelf == false) yield break;
            yield return new WaitForSeconds(delay);
        }
    }

    private IEnumerator inactivateWarningText(float delay) {
        yield return new WaitForSeconds(delay);
        bossWarningScreen.SetActive(false);

    }

    public void UpdateSupplyInfos() {
        List<Supply.SupplyType> supplies = player.GetSupplyInfos();

        for(int i = 0; i < supplies.Count; i++) {
            switch(supplies[i]) {
                case Supply.SupplyType.healKit:
                supplyImages[i].sprite = Resources.Load<Sprite>("Sprites/Supplies/HealKit");
                break;
            case Supply.SupplyType.magnet:
                supplyImages[i].sprite = Resources.Load<Sprite>("Sprites/Supplies/Magnet");
                break;
            case Supply.SupplyType.bomb:
                supplyImages[i].sprite = Resources.Load<Sprite>("Sprites/Supplies/Bomb");
                break;
            case Supply.SupplyType.NONE:
                supplyImages[i].sprite = Resources.Load<Sprite>("Sprites/Supplies/NONE");
                break;
            default:
                return;
            }
        }
    }

}
