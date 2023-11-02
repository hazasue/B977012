using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLevel : MonoBehaviour
{
    private static int DEFAULT_EXP_INCREASEMENT = 10;
    
    // attibutes
    private int exp;
    private int requiredExp;
    private int level;

    private void Awake()
    {
        init();
    }

    private void init()
    {
        level = 1;
        exp = 0;
        this.requiredExp = level * DEFAULT_EXP_INCREASEMENT;
    }
    
    // methods
    public int CheckCurrentExp() { return exp; }

    public int CheckRequiredExp() { return requiredExp; }

    public void GainExp(int value)
    {
        this.exp += value;
        if (exp >= requiredExp) LevelUp();
    }

    public int GetLevel()
    {
        return level;
    }

    private void LevelUp()
    {
        exp -= requiredExp;
        level++;
        requiredExp = level * DEFAULT_EXP_INCREASEMENT;
        UIManager.GetInstance().UpdatePlayerMaxStatus();
        UIManager.GetInstance().UpdateAugmentOptions(WeaponManager.GetInstance().SetAugmentOptions());
    }
}
