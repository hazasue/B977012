using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class LobbyManager : MonoBehaviour
{
    private const string DEFAULT_SHOP_NAME = "shop";
    private const string DEFAULT_INVENTORY_NAME = "inventory";
    private const string DEFAULT_ENHANCEMENT_NAME = "enhancement";
    private const string DEFAULT_PURPOSE_BUY = "buy";
    private const string DEFAULT_PURPOSE_EQUIP = "equip";
    private const string DEFAULT_PURPOSE_ENHANCEMENT = "enhancement";
    private const string DEFAULT_BUY_STATUS_LOW_ORDER = "lowOrder";
    private const string DEFAULT_BUY_STATUS_LOW_MONEY = "lowMoney";
    private const string DEFAULT_BUY_STATUS_ALREADY_HAVE = "alreadyHave";
    private const string DEFAULT_BUY_STATUS_BUYABLE = "buyable";

    private const string DEFAULT_NAME_STAGE_SCREEN = "stage";
    private const string DEFAULT_NAME_INFO_SCREEN = "info";

    private const string DEFAULT_RESET_ENHANCEMENT = "reset";
    private const string NULL_STRING = "";

    private const int MAX_ENHANCE_COUNT = 5;
    
    private static Color INACTIVE_COLOR = new Color32(128, 128, 128, 255);
    private static Color ACTIVE_COLOR = new Color32(255, 255, 255, 255);
    

    private Dictionary<string, CharacterData> characterDatas;
    private string characterCode;
    private Dictionary<string, EnhanceInfo> enhanceInfos;
    private Equipment currentEquipment;
    private Enhancement currentEnhancement;
    private string currentSelectedStat;

    private Stack<GameObject> panels;

    public GameObject selectStageScreen;
    public GameObject infoScreen;

    public Image shopButtonImage;
    public Image inventoryButtonImage;
    public Image enhanceButtonImage;

    public GameObject equipButton;
    public GameObject buyButton;
    public GameObject enhanceButton;

    public RawImage selectedImage;
    public TMP_Text selectedEquipStatus;
    public TMP_Text selectedCode;
    public TMP_Text selectedType;
    public TMP_Text selectedInfo;
    public TMP_Text selectedOccupation;
    public TMP_Text selectedEquipPrice;

    public Image equipButtonSprite;
    public Image buyButtonSprite;
    public Image enhanceButtonSprite;
    public Transform viewport;

    public TMP_Text coinCount;

    public Equipment equipment;
    public Enhancement enhancement;
    
    // Start is called before the first frame update
    void Start()
    {
        init();
    }

    void Update()
    {
        applyKeyInput();
    }

    private void init()
    {
        LoadDatas();
        ShowScreen(DEFAULT_INVENTORY_NAME);
        panels = new Stack<GameObject>();
    }

    private void applyKeyInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            closeScreen();
        }
    }

    public void LoadDatas()
    {
        characterDatas =
            JsonManager.LoadJsonFile<Dictionary<string, CharacterData>>(JsonManager.DEFAULT_CHARACTER_DATA_NAME);
        characterCode = JsonManager.LoadJsonFile<CurrentCharacterInfo>(JsonManager.DEFAULT_CURRENT_CHARACTER_DATA_NAME)
            .currentSelectedCode;
        enhanceInfos =
            JsonManager.LoadJsonFile<Dictionary<string, EnhanceInfo>>(JsonManager.DEFAULT_ENHANCEMENT_DATA_NAME);
    }

    public void ShowScreen(string name)
    {
        // shop contents: all job waepons - gained weapons
        // inventory contents: CharacterData.equipmentCodes
        equipButton.SetActive(false);
        buyButton.SetActive(false);
        enhanceButton.SetActive(false);

        shopButtonImage.color = INACTIVE_COLOR;
        inventoryButtonImage.color = INACTIVE_COLOR;
        enhanceButtonImage.color = INACTIVE_COLOR;
        
        switch (name)
        {
            case DEFAULT_SHOP_NAME:
                shopButtonImage.color = ACTIVE_COLOR;
                buyButton.SetActive(true);
                break;
            
            case DEFAULT_INVENTORY_NAME:
                inventoryButtonImage.color = ACTIVE_COLOR;
                equipButton.SetActive(true);
                break;
            
            case DEFAULT_ENHANCEMENT_NAME:
                enhanceButtonImage.color = ACTIVE_COLOR;
                currentEnhancement = null;
                currentSelectedStat = NULL_STRING;
                enhanceButton.SetActive(true);
                break;
            
            default:
                Debug.Log("Invalid screen name: " + name);
                break;
        }
        
        initContents(name);
    }

    private void initContents(string name)
    {
        // Clear contents
        currentEquipment = null;
        coinCount.text = characterDatas[characterCode].coin.ToString();
        for (int i = viewport.childCount - 1; i >= 0; i--)
        {
            Destroy(viewport.GetChild(i).gameObject);
        }

        switch (name)
        {
            case DEFAULT_SHOP_NAME:
                List<WeaponInfo> unbuyableWeapons = new List<WeaponInfo>();
                string tempCode;
                foreach (WeaponInfo info in WeaponManager.GetInstance().GetEquiptableWeapons())
                {
                    tempCode = info.GetCode();
                    if (getBuyStatus(tempCode) != DEFAULT_BUY_STATUS_BUYABLE)
                    {
                        unbuyableWeapons.Add(info);
                        continue;
                    }
                    
                    Equipment tempEquipment = Instantiate(equipment, viewport, true);
                    tempEquipment.GetComponent<RawImage>().texture =
                        Resources.Load<Texture>("Sprites/weapons/" + info.GetCode());
                    tempEquipment.Init(tempCode, DEFAULT_PURPOSE_BUY, DEFAULT_BUY_STATUS_BUYABLE, info.GetPrice());
                    tempEquipment.GetComponent<Button>().onClick.AddListener(ClickEquipment);

                    if (!currentEquipment)
                    {
                        currentEquipment = tempEquipment;
                        updateSelectedEquipData(currentEquipment.GetPurpose());
                        updateButton(currentEquipment.GetPurpose());
                    }
                }

                foreach (WeaponInfo info in unbuyableWeapons)
                {
                    tempCode = info.GetCode();
                    Equipment tempEquipment = Instantiate(equipment, viewport, true);
                    tempEquipment.GetComponent<RawImage>().texture =
                        Resources.Load<Texture>("Sprites/weapons/" + info.GetCode());
                    tempEquipment.Init(tempCode, DEFAULT_PURPOSE_BUY, getBuyStatus(tempCode), info.GetPrice());
                    tempEquipment.GetComponent<Button>().onClick.AddListener(ClickEquipment);
                    tempEquipment.GetComponent<RawImage>().color = INACTIVE_COLOR;
                    
                    if (!currentEquipment)
                    {
                        currentEquipment = tempEquipment;
                        updateSelectedEquipData(currentEquipment.GetPurpose());
                        updateButton(currentEquipment.GetPurpose());
                    }
                }
                break;
            
            case DEFAULT_INVENTORY_NAME:
                foreach (string code in characterDatas[characterCode].equipmentCodes)
                {
                    Equipment tempEquipment = Instantiate(equipment, viewport, true);
                    tempEquipment.GetComponent<RawImage>().texture =
                        Resources.Load<Texture>("Sprites/weapons/" + code);
                    tempEquipment.Init(code, DEFAULT_PURPOSE_EQUIP);
                    tempEquipment.GetComponent<Button>().onClick.AddListener(ClickEquipment);

                    if (code == characterDatas[characterCode].basicWeapon)
                    {
                        currentEquipment = tempEquipment;
                        updateSelectedEquipData(currentEquipment.GetPurpose());
                        updateButton(currentEquipment.GetPurpose());
                    }
                }
                break;
            
            case DEFAULT_ENHANCEMENT_NAME:
                Enhancement tempEnhancement;
                foreach (EnhanceInfo data in enhanceInfos.Values)
                {
                    tempEnhancement = Instantiate(enhancement, viewport, true);
                    tempEnhancement.GetComponent<RawImage>().texture =
                        Resources.Load<Texture>("Sprites/Enhancements/" + data.stat);
                    tempEnhancement.Init(data.stat, data.enhanceCount, data.value, data.price);
                    tempEnhancement.GetComponent<Button>().onClick.AddListener(ClickEnhancement);
                    if (currentSelectedStat == NULL_STRING)
                    {
                        currentEnhancement = tempEnhancement;
                        currentSelectedStat = tempEnhancement.GetStat();
                        updateSelectedEnhanceData();
                        updateButton(DEFAULT_PURPOSE_ENHANCEMENT);
                    }
                    else if (currentSelectedStat == tempEnhancement.GetStat())
                    {
                        currentEnhancement = tempEnhancement;
                        updateSelectedEnhanceData();
                        updateButton(DEFAULT_PURPOSE_ENHANCEMENT);
                    }
                }
                
                tempEnhancement = Instantiate(enhancement, viewport, true);
                tempEnhancement.Init(DEFAULT_RESET_ENHANCEMENT, 0, 0f, 0);
                tempEnhancement.GetComponent<RawImage>().texture =
                    Resources.Load<Texture>("Sprites/Enhancements/reset");
                tempEnhancement.GetComponent<Button>().onClick.AddListener(ClickEnhancement);
                break;
            
            default:
                Debug.Log("Invalid screen name: " + name);
                break;
        }
        
    }

    public void ClickEnhancement()
    {
        currentEnhancement = EventSystem.current.currentSelectedGameObject.transform.GetComponent<Enhancement>();
        currentSelectedStat = currentEnhancement.GetStat();
        updateSelectedEnhanceData();
        updateButton(DEFAULT_PURPOSE_ENHANCEMENT);
    }

    private void updateSelectedEnhanceData()
    {
        selectedImage.texture = Resources.Load<Texture>("Sprites/Enhancements/" + currentEnhancement.GetStat());
        selectedCode.text = currentEnhancement.GetStat();
        selectedType.text = "";
        if (currentEnhancement.GetEnhanceCount() >= 5)
        {
            selectedInfo.text = "Value: " + (currentEnhancement.GetValue() * currentEnhancement.GetEnhanceCount()).ToString();
            selectedOccupation.text = "Enhance Count: MAX";
            selectedEquipStatus.text = "";
            selectedEquipPrice.text = "Price: None";
        }
        else
        {
            selectedInfo.text = "Value: " + (currentEnhancement.GetValue() * (currentEnhancement.GetEnhanceCount() + 1)).ToString();
            selectedOccupation.text = "Enhance Count: " + currentEnhancement.GetEnhanceCount().ToString();
            selectedEquipStatus.text = "";
            selectedEquipPrice.text = "Price: " + 
                (currentEnhancement.GetPrice() * (currentEnhancement.GetEnhanceCount() + 1)).ToString();
        }

    }

    private void resetEnhancement()
    {
        enhanceInfos =
            JsonManager.LoadJsonFile<Dictionary<string, EnhanceInfo>>(JsonManager.DEFAULT_ENHANCEMENT_BACKUP_DATA_NAME);
        JsonManager.CreateJsonFile(JsonManager.DEFAULT_ENHANCEMENT_DATA_NAME, enhanceInfos);
        initContents(DEFAULT_ENHANCEMENT_NAME);
    }

    public void ClickEquipment()
    {
        currentEquipment = EventSystem.current.currentSelectedGameObject.transform.GetComponent<Equipment>();
        updateSelectedEquipData(currentEquipment.GetPurpose());
        updateButton(currentEquipment.GetPurpose());
    }

    private void updateSelectedEquipData(string purpose)
    {
        // show infos
        switch (currentEquipment.GetType())
        {
            case Equipment.EquipmentType.ARMOR:
                break;

            case Equipment.EquipmentType.WEAPON:
                WeaponInfo weaponInfo = WeaponManager.GetInstance().GetWeaponInfo(currentEquipment.GetCode());
                selectedImage.texture = Resources.Load<Texture>("Sprites/weapons/" + weaponInfo.GetCode());
                selectedCode.text = weaponInfo.GetCode();
                selectedType.text = weaponInfo.GetType();
                selectedInfo.text = weaponInfo.GetDamage() + " / " + weaponInfo.GetDuration() + " / " +
                                    weaponInfo.GetDelay() + " / " + weaponInfo.GetProjectile();
                selectedOccupation.text = weaponInfo.GetOccupation() + " " + weaponInfo.GetOrder();
                break;

            default:
                Debug.Log("Invalid equipment type: " + currentEquipment.GetType());
                break;
        }
        
        switch (purpose)
        {
            case DEFAULT_PURPOSE_BUY:
                selectedEquipStatus.text = currentEquipment.GetBuyStatus();
                selectedEquipPrice.text = "Price: " + currentEquipment.GetPrice().ToString();
                break;
            
            case DEFAULT_PURPOSE_EQUIP:
                selectedEquipStatus.text = "";
                selectedEquipPrice.text = "";
                break;
            
            default:
                Debug.Log("Invalid purpose: " + purpose);
                break;
        }
    }

    public void EquipEquipment()
    {
        if (!currentEquipment) return;
        if (characterDatas[characterCode].basicWeapon == currentEquipment.GetCode()) return;
        
        switch (currentEquipment.GetType())
        {
            case Equipment.EquipmentType.ARMOR:
                break;

            case Equipment.EquipmentType.WEAPON:
                characterDatas[characterCode].basicWeapon = currentEquipment.GetCode();
                break;

            default:
                Debug.Log("Invalid equipment type: " + currentEquipment.GetType());
                break;
        }
        
        updateButton(currentEquipment.GetPurpose());

        JsonManager.CreateJsonFile(JsonManager.DEFAULT_CHARACTER_DATA_NAME, characterDatas);
    }

    public void BuyEquipment()
    {
        if (!currentEquipment) return;
        if (characterDatas[characterCode].coin < currentEquipment.GetPrice()) return;
        foreach (string equipment in characterDatas[characterCode].equipmentCodes)
        {
            if (equipment == currentEquipment.GetCode()) return;
        }

        characterDatas[characterCode].coin -=
            WeaponManager.GetInstance().GetWeaponInfo(currentEquipment.GetCode()).GetPrice();
        characterDatas[characterCode].equipmentCodes.Add(currentEquipment.GetCode());

        JsonManager.CreateJsonFile(JsonManager.DEFAULT_CHARACTER_DATA_NAME, characterDatas);
        
        initContents(DEFAULT_SHOP_NAME);
    }

    public void EnhanceStat()
    {
        if (!currentEnhancement) return;
        if (currentEnhancement.GetEnhanceCount() >= MAX_ENHANCE_COUNT) return;
        if (characterDatas[characterCode].coin < currentEnhancement.GetPrice() * (currentEnhancement.GetEnhanceCount() + 1)) return;
        if (currentEnhancement.GetStat() == DEFAULT_RESET_ENHANCEMENT)
        {
            resetEnhancement();
            return;
        }

        characterDatas[characterCode].coin -=
            currentEnhancement.GetPrice() * (currentEnhancement.GetEnhanceCount() + 1);

        enhanceInfos[currentEnhancement.GetStat()].enhanceCount++;

        JsonManager.CreateJsonFile(JsonManager.DEFAULT_CHARACTER_DATA_NAME, characterDatas);
        JsonManager.CreateJsonFile(JsonManager.DEFAULT_ENHANCEMENT_DATA_NAME, enhanceInfos);

        initContents(DEFAULT_ENHANCEMENT_NAME);
    }

    private void updateButton(string purpose)
    {
        switch (purpose)
        {
            case DEFAULT_PURPOSE_BUY:
                if (currentEquipment.GetBuyStatus() == DEFAULT_BUY_STATUS_BUYABLE)
                    buyButtonSprite.color = ACTIVE_COLOR;
                else
                {
                    buyButtonSprite.color = INACTIVE_COLOR;
                }
                break;

            case DEFAULT_PURPOSE_EQUIP:
                if (currentEquipment.GetCode() == characterDatas[characterCode].basicWeapon)
                    equipButtonSprite.color = INACTIVE_COLOR;
                else
                {
                    equipButtonSprite.color = ACTIVE_COLOR;
                }
                break;
            
            case DEFAULT_PURPOSE_ENHANCEMENT:
                if (currentEnhancement.GetEnhanceCount() >= MAX_ENHANCE_COUNT)
                    enhanceButtonSprite.color = INACTIVE_COLOR;
                else
                {
                    enhanceButtonSprite.color = ACTIVE_COLOR;
                }
                break;
        }
    }

    private string getBuyStatus(string code)
    {
        WeaponInfo tempWeapon = WeaponManager.GetInstance().GetWeaponInfo(code);
        if (tempWeapon.GetOrder() > characterDatas[characterCode].order) return DEFAULT_BUY_STATUS_LOW_ORDER;
        if (tempWeapon.GetPrice() > characterDatas[characterCode].coin) return DEFAULT_BUY_STATUS_LOW_MONEY;
        foreach (string data in characterDatas[characterCode].equipmentCodes)
        {
            if (tempWeapon.GetCode() == data) return DEFAULT_BUY_STATUS_ALREADY_HAVE;
        }

        return DEFAULT_BUY_STATUS_BUYABLE;
    }

    public void ActivateScreen(string name)
    {
        switch (name)
        {
            case DEFAULT_NAME_STAGE_SCREEN:
                selectStageScreen.SetActive(true);
                panels.Push(selectStageScreen);
                break;
            
            case DEFAULT_NAME_INFO_SCREEN:
                infoScreen.SetActive(true);
                panels.Push(infoScreen);
                break;
            
            default:
                Debug.Log("Invalid screen name: " + name);
                break;
        }
    }

    public void InactivateScreen(string name)
    {
        switch (name)
        {
            case DEFAULT_NAME_STAGE_SCREEN:
                selectStageScreen.SetActive(false);
                break;
            
            case DEFAULT_NAME_INFO_SCREEN:
                infoScreen.SetActive(false);
                break;
            
            default:
                Debug.Log("Invalid screen name: " + name);
                break;
        }
    }

    private void closeScreen()
    {
        if (panels.Count <= 0) return;
        
        panels.Peek().SetActive(false);
        panels.Pop();
    }
}
