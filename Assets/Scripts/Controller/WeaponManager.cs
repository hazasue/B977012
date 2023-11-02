using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    private static WeaponManager instance;
    
    private static string CSV_FILENAME_WEAPON = "DataTable_Weapon";
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

        List<Dictionary<string, object>> WeaponDB = CSVReader.Read(CSV_FILENAME_WEAPON);
        foreach (Dictionary<string, object> weaponInfo in WeaponDB)
        {
            weaponInfos.Add(weaponInfo["WeaponCode"].ToString(),
                new WeaponInfo(weaponInfo["WeaponCode"].ToString(),
                    weaponInfo["WeaponName"].ToString(),
                    weaponInfo["WeaponType"].ToString(),
                    (int)weaponInfo["Damage"],
                    float.Parse(weaponInfo["Duration"].ToString()),
                    float.Parse(weaponInfo["Delay"].ToString()),
                    (int)weaponInfo["Projectile"],
                    float.Parse(weaponInfo["Range"].ToString()),
                    float.Parse(weaponInfo["Speed"].ToString())
                ));
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
