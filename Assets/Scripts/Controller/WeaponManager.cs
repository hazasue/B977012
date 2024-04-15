using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WeaponManager : MonoBehaviour
{
    private static WeaponManager instance;
    
    public const int DEFAULT_OPTION_COUNT = 4;
    public const int MAX_WEAPON_COUNT = 5;
    public const int MAX_UPGRADE_COUNT = 5;
    public const string DEFAULT_AUGMENT_COIN = "coin";
    public const int DEFAULT_COIN_VALUE = 10;

    private Dictionary<string, WeaponInfo> weaponInfos;
    private Dictionary<string, WeaponUpgradeInfo> weaponUpgradeInfos;
    private List<string> removedWeaponCodes;

    private List<WeaponInfo> augmentOptions;
    private string playerType;
    private string basicWeaponCode;
    private Inventory inventory;
    
    // Start is called before the first frame update
    void Awake()
    {
        init();
    }

    void Start()
    {
        if (SceneManager.GetActiveScene().name == "InGame")
            inventory = GameManager.GetInstance().GetPlayer().GetInventory();
    }

    private void init()
    {
        weaponInfos = new Dictionary<string, WeaponInfo>();
        weaponUpgradeInfos = new Dictionary<string, WeaponUpgradeInfo>();
        augmentOptions = new List<WeaponInfo>();
        removedWeaponCodes = new List<string>();
        
        Dictionary<string, WeaponInfo> tempWeaponInfos =
            JsonManager.LoadJsonFile<Dictionary<string, WeaponInfo>>(JsonManager.DEFAULT_WEAPON_DATA_NAME);
        Dictionary<string, WeaponUpgradeInfo> tempWeaponUpgradeInfo =
            JsonManager.LoadJsonFile<Dictionary<string, WeaponUpgradeInfo>>(
                JsonManager.DEFAULT_WEAPON_UPGRADE_DATA_NAME);

        foreach (KeyValuePair<string, WeaponInfo> data in tempWeaponInfos)
        {
            weaponInfos.Add(data.Key, data.Value);
        }

        foreach (KeyValuePair<string, WeaponUpgradeInfo> data in tempWeaponUpgradeInfo)
        {
            weaponUpgradeInfos.Add(data.Key, data.Value);
        }

        playerType =
            JsonManager.LoadJsonFile<Dictionary<string, CharacterData>>(JsonManager.DEFAULT_CHARACTER_DATA_NAME)
                [JsonManager.LoadJsonFile<CurrentCharacterInfo>(JsonManager.DEFAULT_CURRENT_CHARACTER_DATA_NAME).currentSelectedCode]
                .playerType;

        basicWeaponCode =
            JsonManager.LoadJsonFile<Dictionary<string, CharacterData>>(JsonManager.DEFAULT_CHARACTER_DATA_NAME)
                [JsonManager.LoadJsonFile<CurrentCharacterInfo>(JsonManager.DEFAULT_CURRENT_CHARACTER_DATA_NAME).currentSelectedCode]
                .basicWeapon;
    }
    
    public static WeaponManager GetInstance()
    {
        if (instance != null) return instance;
        instance = FindObjectOfType<WeaponManager>();
        if (instance == null) Debug.Log("There's no active WeaponManager object");
        return instance;
    }

    public string GetBasicWeaponCode()
    {
        return basicWeaponCode;
    }

    public WeaponInfo GetWeaponInfo(string code)
    {
        if (weaponInfos.ContainsKey(code)) return weaponInfos[code];
        else
        {
            Debug.Log("Invalid weapon code: " + code);
            return null;
        }
    }
    
    public WeaponUpgradeInfo GetWeaponUpgradeInfo(string code)
    {
        if (weaponUpgradeInfos.ContainsKey(code)) return weaponUpgradeInfos[code];
        else
        {
            Debug.Log("Invalid weapon code: " + code);
            return null;
        }
    }

    public Dictionary<string, WeaponInfo> GetAllWeaponInfos() { return weaponInfos; }

    public List<WeaponInfo> SetAugmentOptions()
    {
        List<string> optionCodes = new List<string>();
        augmentOptions.Clear();
        Dictionary<string, Weapon> weapons = inventory.GetWeapons();
        int weaponCount = weapons.Count;

        // 기본 직업 무기
        optionCodes.Add(basicWeaponCode);
        
        // 공용 무기, 합성 무기 체크
        foreach (WeaponInfo data in weaponInfos.Values)
        {
            if (data.GetOccupation() == "synthesis"
                && weapons.ContainsKey(data.ingr1)
                && weapons.ContainsKey(data.ingr2)
                && weapons[data.ingr1].GetUpgradeCount() >= MAX_UPGRADE_COUNT
                && weapons[data.ingr2].GetUpgradeCount() >= MAX_UPGRADE_COUNT)
            {
                optionCodes.Add(data.GetCode());
                continue;
            }

            if (data.GetOccupation() == "common")
            {
                if(weaponCount < MAX_WEAPON_COUNT) optionCodes.Add(data.GetCode());
                else if (weapons.ContainsKey(data.GetCode()))
                {
                    optionCodes.Add(data.GetCode());
                }
            }
        }
        

        // 업그레이드 횟수 초과한 무기 리스트에서 제거
        foreach (Weapon weapon in weapons.Values)
        {
            if (weapon.GetUpgradeCount() >= MAX_UPGRADE_COUNT) optionCodes.Remove(weapon.GetCode());
        }

        // 합성에 사용된 무기 리스트에서 제거
        foreach (string code in removedWeaponCodes)
        {
            if (optionCodes.Contains(code)) optionCodes.Remove(code);
        }

        int idx;

        for (int i = 0; i < DEFAULT_OPTION_COUNT; i++)
        {
            if (optionCodes.Count <= 0) break;
            idx = Random.Range(0, optionCodes.Count);
            augmentOptions.Add(weaponInfos[optionCodes[idx]]);
            optionCodes.RemoveAt(idx);
        }

        return augmentOptions;
    }

    public void ReflectAugment(int index)
    {
        if (index >= augmentOptions.Count)
        {
            inventory.GainCoins(DEFAULT_COIN_VALUE);
            return;
        }
        
        Weapon selectedWeapon;
        if (!inventory.GetWeapons().TryGetValue(augmentOptions[index].GetCode(), out selectedWeapon))
            selectedWeapon = null;
        
        if(!selectedWeapon
           && augmentOptions[index].GetOccupation() == "common")
            inventory.AddWeapon(augmentOptions[index]);
        else if (!selectedWeapon
                 && augmentOptions[index].GetOccupation() == "synthesis")
        {
            inventory.AddWeapon(augmentOptions[index]);
            inventory.RemoveWeapon(augmentOptions[index].ingr1);
            inventory.RemoveWeapon(augmentOptions[index].ingr2);

            removedWeaponCodes.Add(augmentOptions[index].ingr1);
            removedWeaponCodes.Add(augmentOptions[index].ingr2);
        }
        else
        {
            selectedWeapon.UpgradeWeapon(weaponUpgradeInfos[augmentOptions[index].GetCode()]);
        }
    }

    public List<WeaponInfo> GetEquiptableWeapons()
    {
        List<WeaponInfo> infos = new List<WeaponInfo>();
        foreach (WeaponInfo info in weaponInfos.Values)
        {
            if (info.GetOccupation() == playerType) infos.Add(info);
        }

        return infos;
    }

    // test code
    public void UpgradeBasicWeaponFull()
    {
        GameManager.GetInstance().GetPlayer().GetLevelInfo().GainExp(100);
    }
}
