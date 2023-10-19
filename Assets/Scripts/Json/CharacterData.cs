using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterData
{
    public List<string> playerCode;
    public List<string> basicWeapon;
    public List<string> playerType;
    public List<int> maxHp;
    public List<int> damage;
    public List<float> speed;
    public List<int> armor;

    public CharacterData()
    {
        this.playerCode = new List<string>();
        this.basicWeapon = new List<string>();
        this.playerType = new List<string>();
        this.maxHp = new List<int>();
        this.damage = new List<int>();
        this.speed = new List<float>();
        this.armor = new List<int>();
    }
    
    public CharacterData(string playerCode, string basicWeapon, string playerType, int maxHp, int damage, float speed, int armor)
    {
        this.playerType = new List<string>();
        this.basicWeapon = new List<string>();
        this.playerType = new List<string>();
        this.maxHp = new List<int>();
        this.damage = new List<int>();
        this.speed = new List<float>();
        this.armor = new List<int>();
        
        this.playerCode.Add(playerCode);
        this.basicWeapon.Add(basicWeapon);
        this.playerType.Add(playerType);
        this.maxHp.Add(maxHp);
        this.damage.Add(damage);
        this.speed.Add(speed);
        this.armor.Add(armor);
    }

    public void AddData(string playerCode, string basicWeapon, string playerType, int maxHp, int damage, float speed,
        int armor)
    {
        this.playerCode.Add(playerCode);
        this.basicWeapon.Add(basicWeapon);
        this.playerType.Add(playerType);
        this.maxHp.Add(maxHp);
        this.damage.Add(damage);
        this.speed.Add(speed);
        this.armor.Add(armor);
    }

    public void RemoveData(int slot)
    {
        if (slot > this.playerCode.Count) return;

        this.playerCode.RemoveAt(slot);
        this.basicWeapon.RemoveAt(slot);
        this.playerType.RemoveAt(slot);
        this.maxHp.RemoveAt(slot);
        this.damage.RemoveAt(slot);
        this.speed.RemoveAt(slot);
        this.armor.RemoveAt(slot);
    }

    public bool Exist(int slotNum)
    {
        if (slotNum < playerCode.Count) return true;
        else
        {
            return false;
        }
    }
}
