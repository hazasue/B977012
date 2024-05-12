using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterData
{
    public string basicWeapon;
    public string basicSkill;
    public string basicSkillName;
    public string playerType;
    public int maxHp;
    public int damage;
    public float speed;
    public int armor;
    public int order;
    public List<string> equipmentCodes;
    public List<bool> clearStages;
    public int currentStage;
    public int coin;

    public CharacterData() { }
    
    public CharacterData(string basicWeapon, string basicSkill, string basicSkillName, string playerType, int maxHp, int damage, float speed, int armor, int order, List<string> equipmentCodes, List<bool> clearStages, int currentStage = 0, int coin = 0)
    {
        this.basicWeapon = basicWeapon;
        this.basicSkill = basicSkill;
        this.basicSkillName = basicSkillName;
        this.playerType = playerType;
        this.maxHp = maxHp;
        this.damage = damage;
        this.speed = speed;
        this.armor = armor;
        this.order = order;
        this.equipmentCodes = equipmentCodes;
        this.clearStages = clearStages;
        this.currentStage = currentStage;
        this.coin = coin;
    }
}
