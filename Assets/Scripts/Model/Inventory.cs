using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private static string DEFAULT_WEAPON_OBJECT = "Basic Weapon";
    
    // attributes
    private int coin;
    
    // associations
    private List<Weapon> weapons;
    private List<Accessory> accessories;
    
    void Awake()
    {
        Init();
    }

    // methods
    private void Init()
    {
        weapons = new List<Weapon>();
        accessories = new List<Accessory>();
    }

    public List<Weapon> GetWeapons()
    {
        return weapons;
    }

    public void AddWeapon(WeaponInfo weaponInfo)
    {
        if (weaponInfo == null) return;

        Weapon tempWeapon = Instantiate(Resources.Load<Weapon>("Prefabs/weapons/" + DEFAULT_WEAPON_OBJECT), this.transform, true);
        tempWeapon.Init(weaponInfo);
        weapons.Add(tempWeapon);
    }

    public List<Accessory> GetAccessories()
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
