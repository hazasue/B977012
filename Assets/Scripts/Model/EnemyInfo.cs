using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInfo
{
    private string enemyCode;
    private Enemy.EnemyType enemyType;
    private Enemy.EnemyGrade enemyGrade;
    private int maxHp;
    private int damage;
    private float speed;
    private int armor;
    private float tickTime;
    private int exp;

    public EnemyInfo(string enemyCode, string enemyType, string enemyGrade, int maxHp, int damage, float speed,
        int armor, float tickTime, int exp)
    {
        this.enemyCode = enemyCode;
        this.maxHp = maxHp;
        this.damage = damage;
        this.speed = speed;
        this.armor = armor;
        this.tickTime = tickTime;
        this.exp = exp;

        switch (enemyType)
        {
            case "MELEE":
                this.enemyType = Enemy.EnemyType.MELEE;
                break;
            
            case "RANGED":
                this.enemyType = Enemy.EnemyType.RANGED;
                break;
            
            case "EXPLOSIVE":
                this.enemyType = Enemy.EnemyType.EXPLOSIVE;
                break;
            
            default:
                Debug.Log("Invalid enemy type: " + enemyType);
                break;
        }

        switch (enemyGrade)
        {
            case "NORMAL":
                this.enemyGrade = Enemy.EnemyGrade.NORMAL;
                break;
            
            case "ELITE":
                this.enemyGrade = Enemy.EnemyGrade.ELITE;
                break;
            
            case "BOSS":
                this.enemyGrade = Enemy.EnemyGrade.BOSS;
                break;
            
            default:
                Debug.Log("Invalid enemy grade: " + enemyGrade);
                break;
        }
    }

    public string GetCode() { return enemyCode; }
    
    public Enemy.EnemyType GetType() { return enemyType; }
    
    public Enemy.EnemyGrade GetGrade() { return enemyGrade; }
    
    public int GetMaxHp() { return maxHp; }
    
    public int GetDamage() { return damage; }
    
    public float GetSpeed() { return speed; }
    
    public int GetArmor() { return armor; }
    
    public float GetTickTime() { return tickTime; }
    
    public int GetExp() { return exp; }
}
