using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyInfo
{
    public string enemyCode;
    public string enemyType;
    public string enemyGrade;
    public int maxHp;
    public int damage;
    public float speed;
    public int armor;
    public float tickTime;
    public int exp;
    public bool canRangeAttack;
    public bool canUseSkill;
    public float spawnDelay;

    public EnemyInfo(string enemyCode, string enemyType, string enemyGrade, int maxHp, int damage, float speed,
        int armor, float tickTime, int exp, bool canRangeAttack, bool canUseSkill, float spawnDelay)
    {
        this.enemyCode = enemyCode;
        this.maxHp = maxHp;
        this.damage = damage;
        this.speed = speed;
        this.armor = armor;
        this.tickTime = tickTime;
        this.exp = exp;
        this.enemyType = enemyType;
        this.enemyGrade = enemyGrade;
        this.canRangeAttack = canRangeAttack;
        this.canUseSkill = canUseSkill;
        this.spawnDelay = spawnDelay;
    }

    public string GetCode() { return enemyCode; }
    
    public string GetType() { return enemyType; }
    
    public string GetGrade() { return enemyGrade; }
    
    public int GetMaxHp() { return maxHp; }
    
    public int GetDamage() { return damage; }
    
    public float GetSpeed() { return speed; }
    
    public int GetArmor() { return armor; }
    
    public float GetTickTime() { return tickTime; }
    
    public int GetExp() { return exp; }
}
