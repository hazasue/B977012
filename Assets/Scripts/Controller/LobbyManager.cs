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
    private static Color INACTIVE_COLOR = new Color32(128, 128, 128, 255);
    private static Color ACTIVE_COLOR = new Color32(255, 255, 255, 255);
    

    private Dictionary<string, CharacterData> characterDatas;
    private string characterCode;
    private Equipment currentEquipment;

    public Image selectedImage;
    public TMP_Text selectedCode;
    public TMP_Text selectedType;
    public TMP_Text selectedInfo;

    public Image equipButtonSprite;
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
        switch (name)
        {
            case DEFAULT_SHOP_NAME:
                break;
            
            case DEFAULT_INVENTORY_NAME:
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
        for (int i = viewport.childCount - 1; i >= 0; i--)
        {
            Destroy(viewport.GetChild(i).gameObject);
        }

        foreach (string code in characterDatas[characterCode].equipmentCodes)
        {
            Equipment tempEquipment = Instantiate(equipment, viewport, true);
            tempEquipment.Init(code);
            tempEquipment.GetComponent<Button>().onClick.AddListener(ClickEquipment);

            if (code == characterDatas[characterCode].basicWeapon)
            {
                currentEquipment = tempEquipment;
                updateSelectedEquipData();
                updateEquipButton();
            }
        }
    }

    public void ClickEquipment()
    {
        currentEquipment = EventSystem.current.currentSelectedGameObject.transform.GetComponent<Equipment>();
        updateSelectedEquipData();
        updateEquipButton();
    }

    private void updateSelectedEquipData()
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
                break;

            default:
                Debug.Log("Invalid equipment type: " + currentEquipment.GetType());
                break;
        }
    }

    public void EquipEquipment()
    {
        if (!currentEquipment) return;
        
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

        updateEquipButton();

        JsonManager.CreateJsonFile(JsonManager.DEFAULT_CHARACTER_DATA_NAME, characterDatas);
    }

    private void updateEquipButton()
    {
        if (currentEquipment.GetCode() == characterDatas[characterCode].basicWeapon)
            equipButtonSprite.color = INACTIVE_COLOR;
        else
        {
            equipButtonSprite.color = ACTIVE_COLOR;
        }
    }
}
