using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterData
{
    public string basicWeapon;
    public string playerType;
    public int maxHp;
    public int damage;
    public float speed;
    public int armor;

    public CharacterData() { }
    
    public CharacterData(string basicWeapon, string playerType, int maxHp, int damage, float speed, int armor)
    {
        this.basicWeapon = basicWeapon;
        this.playerType = playerType;
        this.maxHp = maxHp;
        this.damage = damage;
        this.speed = speed;
        this.armor = armor;
    }
}
