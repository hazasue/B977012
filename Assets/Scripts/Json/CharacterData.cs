using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterData
{
    public string playerCode;
    public string basicWeapon;
    public string playerType;
    public int maxHp;
    public int damage;
    public float speed;
    public int armor;
    public bool dataExist; 

    public CharacterData()
    {
        this.playerType = "";
        this.basicWeapon = "";
        dataExist = false;
    }
    
    public CharacterData(string playerCode, string basicWeapon, string playerType, int maxHp, int damage, float speed, int armor)
    {
        this.playerCode = playerCode;
        this.basicWeapon = basicWeapon;
        this.playerType = playerType;
        this.maxHp = maxHp;
        this.damage = damage;
        this.speed = speed;
        this.armor = armor;
        dataExist = true;
    }

    public bool Exist()
    {
        if (dataExist) return true;
        else
        {
            return false;
        }
    }
}