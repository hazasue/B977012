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
    private const string DEFAULT_UPGRADE_NAME = "upgrade";
    private const string DEFAULT_PURPOSE_BUY = "buy";
    private const string DEFAULT_PURPOSE_EQUIP = "equip";
    private const string DEFAULT_BUY_STATUS_LOW_ORDER = "lowOrder";
    private const string DEFAULT_BUY_STATUS_LOW_MONEY = "lowMoney";
    private const string DEFAULT_BUY_STATUS_ALREADY_HAVE = "alreadyHave";
    private const string DEFAULT_BUY_STATUS_BUYABLE = "buyable";
    
    private static Color INACTIVE_COLOR = new Color32(128, 128, 128, 255);
    private static Color ACTIVE_COLOR = new Color32(255, 255, 255, 255);
    

    private Dictionary<string, CharacterData> characterDatas;
    private string characterCode;
    private Equipment currentEquipment;

    public Image shopButtonImage;
    public Image inventoryButtonImage;
    public Image enhanceButtonImage;

    public GameObject equipButton;
    public GameObject buyButton;

    public Image selectedImage;
    public TMP_Text selectedEquipStatus;
    public TMP_Text selectedCode;
    public TMP_Text selectedType;
    public TMP_Text selectedInfo;
    public TMP_Text selectedOccupation;
    public TMP_Text selectedEquipPrice;

    public Image equipButtonSprite;
    public Image buyButtonSprite;
    public Transform viewport;

    public Equipment equipment;
    
    // Start is called before the first frame update
    void Start()
    {
        init();
    }

    private void init()
    {
        characterDatas =
            JsonManager.LoadJsonFile<Dictionary<string, CharacterData>>(JsonManager.DEFAULT_CHARACTER_DATA_NAME);
        characterCode = JsonManager.LoadJsonFile<CurrentCharacterInfo>(JsonManager.DEFAULT_CURRENT_CHARACTER_DATA_NAME)
            .currentSelectedCode;
        ShowScreen(DEFAULT_INVENTORY_NAME);
    }

    public void ShowScreen(string name)
    {
        // shop contents: all job waepons - gained weapons
        // inventory contents: CharacterData.equipmentCodes
        equipButton.SetActive(false);
        buyButton.SetActive(false);

        shopButtonImage.color = ACTIVE_COLOR;
        inventoryButtonImage.color = ACTIVE_COLOR;
        enhanceButtonImage.color = ACTIVE_COLOR;
        
        switch (name)
        {
            case DEFAULT_SHOP_NAME:
                shopButtonImage.color = INACTIVE_COLOR;
                buyButton.SetActive(true);
                break;
            
            case DEFAULT_INVENTORY_NAME:
                inventoryButtonImage.color = INACTIVE_COLOR;
                equipButton.SetActive(true);
                break;
            
            case DEFAULT_UPGRADE_NAME:
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
        for (int i = viewport.childCount - 1; i >= 0; i--)
        {
            Destroy(viewport.GetChild(i).gameObject);
        }

        switch (name)
        {
            case DEFAULT_SHOP_NAME:
                List<WeaponInfo> ownWeapons = new List<WeaponInfo>();
                string tempCode;
                foreach (WeaponInfo info in WeaponManager.GetInstance().GetEquiptableWeapons())
                {
                    tempCode = info.GetCode();
                    if (getBuyStatus(tempCode) != DEFAULT_BUY_STATUS_BUYABLE)
                    {
                        ownWeapons.Add(info);
                        continue;
                    }
                    
                    Equipment tempEquipment = Instantiate(equipment, viewport, true);
                    tempEquipment.Init(tempCode, DEFAULT_PURPOSE_BUY, DEFAULT_BUY_STATUS_BUYABLE, info.GetPrice());
                    tempEquipment.GetComponent<Button>().onClick.AddListener(ClickEquipment);

                    if (!currentEquipment)
                    {
                        currentEquipment = tempEquipment;
                        updateSelectedEquipData(currentEquipment.GetPurpose());
                        updateButton(currentEquipment.GetPurpose());
                    }
                }

                foreach (WeaponInfo info in ownWeapons)
                {
                    tempCode = info.GetCode();
                    Equipment tempEquipment = Instantiate(equipment, viewport, true);
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
            
            case DEFAULT_UPGRADE_NAME:
                break;
            
            default:
                Debug.Log("Invalid screen name: " + name);
                break;
        }
        
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
                selectedImage.sprite = Resources.Load<Sprite>("Images/weapons/" + weaponInfo.GetCode());
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
}
