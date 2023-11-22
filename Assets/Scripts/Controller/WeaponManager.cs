using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    private static WeaponManager instance;
    
    private static int DEFAULT_OPTION_COUNT = 4;
    private static int MAX_UPGRADE_COUNT = 5;

    private Dictionary<string, WeaponInfo> weaponInfos;
    private Dictionary<string, WeaponUpgradeInfo> weaponUpgradeInfos;

    private List<WeaponInfo> augmentOptions;
    private string playerType;
    private string basicWeaponCode;
    
    // Start is called before the first frame update
    void Awake()
    {
        init();
    }

    private void init()
    {
        weaponInfos = new Dictionary<string, WeaponInfo>();
        weaponUpgradeInfos = new Dictionary<string, WeaponUpgradeInfo>();
        augmentOptions = new List<WeaponInfo>();
        
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

    public WeaponInfo GetWeaponInfo(string code)
    {
        if (weaponInfos.ContainsKey(code)) return weaponInfos[code];
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

        // 기본 직업 무기
        optionCodes.Add(basicWeaponCode);
        
        // 공용 무기
        foreach (WeaponInfo data in weaponInfos.Values)
        {
            if (data.GetOccupation() != "common") continue;
            optionCodes.Add(data.GetCode());
        }

        // 업그레이드 횟수 초과한 무기 리스트에서 제거
        foreach (Weapon weapon in GameManager.GetInstance().GetPlayer().GetInventory().GetWeapons().Values)
        {
            if (weapon.GetUpgradeCount() >= MAX_UPGRADE_COUNT) optionCodes.Remove(weapon.GetCode());
        }

        int idx;
        
        for (int i = 0; i < DEFAULT_OPTION_COUNT; i++)
        {
            idx = Random.Range(0, optionCodes.Count);
            augmentOptions.Add(weaponInfos[optionCodes[idx]]);
            optionCodes.RemoveAt(idx);
        }

        return augmentOptions;
    }

    public void ReflectAugment(int index)
    {
        Weapon selectedWeapon;
        if (!GameManager.GetInstance().GetPlayer().GetInventory().GetWeapons().TryGetValue(augmentOptions[index].GetCode(), out selectedWeapon))
            selectedWeapon = null;
        
        if(!selectedWeapon)
            GameManager.GetInstance().GetPlayer().GetInventory().AddWeapon(augmentOptions[index]);
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
        GameManager.GetInstance().GetPlayer().GetInventory().GetWeapons()[basicWeaponCode]
            .UpgradeWeapon(weaponUpgradeInfos[basicWeaponCode]);
    }
}
