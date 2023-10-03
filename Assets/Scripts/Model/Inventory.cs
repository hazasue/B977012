using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
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
    private void Init() {}

    public List<Weapon> GetWeapons()
    {
        return weapons;
    }
    
    public void AddWeapon(Weapon weapon) {}

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
