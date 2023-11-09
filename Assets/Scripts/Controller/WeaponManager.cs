using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    private static WeaponManager instance;
    
    private static int DEFAULT_OPTION_COUNT = 4;

    private Dictionary<string, WeaponInfo> weaponInfos;

    private List<WeaponInfo> augmentOptions;
    
    // Start is called before the first frame update
    void Awake()
    {
        init();
    }

    private void init()
    {
        weaponInfos = new Dictionary<string, WeaponInfo>();
        augmentOptions = new List<WeaponInfo>();
        
        Dictionary<string, WeaponInfo> tempWeaponInfos =
            JsonManager.LoadJsonFile<Dictionary<string, WeaponInfo>>(JsonManager.DEFAULT_WEAPON_DATA_NAME);

        foreach (KeyValuePair<string, WeaponInfo> data in tempWeaponInfos)
        {
            weaponInfos.Add(data.Key, data.Value);
        }
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

        foreach (WeaponInfo data in weaponInfos.Values)
        {
            optionCodes.Add(data.GetCode());
        }

        for (int i = 0; i < DEFAULT_OPTION_COUNT; i++)
        {
            augmentOptions.Add(weaponInfos[optionCodes[Random.Range(0, optionCodes.Count)]]);
        }

        return augmentOptions;
    }

    public void ReflectAugment(int index)
    {
        GameManager.GetInstance().GetPlayer().GetInventory().AddWeapon(augmentOptions[index]);
    }
}
