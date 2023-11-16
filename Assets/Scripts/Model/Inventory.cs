using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private static string DEFAULT_WEAPON_OBJECT = "Basic Weapon";
    
    // attributes
    private int coin;
    public RangeCollider rangeCollider;
    
    // associations
    private Dictionary<string, Weapon> weapons;
    private Dictionary<string, Accessory> accessories;
    
    void Awake()
    {
        Init();
    }

    // methods
    private void Init()
    {
        weapons = new Dictionary<string, Weapon>();
        accessories = new Dictionary<string, Accessory>();
    }

    public Dictionary<string, Weapon> GetWeapons()
    {
        return weapons;
    }

    public void AddWeapon(WeaponInfo weaponInfo, bool mainWeapon = false)
    {
        if (weaponInfo == null) return;

        GameObject tempWeapon = Instantiate(Resources.Load<GameObject>("Prefabs/weapons/" + DEFAULT_WEAPON_OBJECT), this.transform, true);
        tempWeapon.transform.localPosition = Vector3.zero;
        switch (weaponInfo.GetType())
        {
            case "MELEE":
                tempWeapon.AddComponent<MeleeWeapon>();
                break;
            
            case "RANGED":
                break;
            
            case "TRACKING":
                tempWeapon.AddComponent<TrackingWeapon>();
                break;
            
            case "CHAINING":
                tempWeapon.AddComponent<ChainingWeapon>();
                break;
            
            case "BEAM":
                tempWeapon.AddComponent<BeamWeapon>();
                break;
            
            case "BARRIER":
                tempWeapon.AddComponent<BarrierWeapon>();
                break;
            
            case "EXPLOSIVE":
                break;
            
            default:
                break;
        }
        
        RangeCollider tempRangeCollider = Instantiate(rangeCollider, tempWeapon.transform, true);
        tempRangeCollider.transform.localPosition = Vector3.zero;
        Weapon tempWeaponScript = tempWeapon.GetComponent<Weapon>();
        tempWeaponScript.Init(weaponInfo, tempRangeCollider, mainWeapon);
        weapons.Add(tempWeaponScript.GetCode(), tempWeaponScript);
    }

    public Dictionary<string, Accessory> GetAccessories()
    {
        return accessories;
    }
    
    public void AddAccessory(Accessory accessory) {}

    public int CheckCoins()
    {
        return coin;
    }
    
    public void GainCoins(int value) {}
}
