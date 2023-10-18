using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    private static WeaponManager instance;
    
    private static string CSV_FILENAME_WEAPON = "DataTable_Weapon";

    private Dictionary<string, WeaponInfo> weaponInfos;
    
    // Start is called before the first frame update
    void Awake()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Init()
    {
        weaponInfos = new Dictionary<string, WeaponInfo>();

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
}
