using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLevel : MonoBehaviour
{
    private static float DEFAULT_EXP_INCREASEMENT = 10f;
    
    // attibutes
    private float exp;
    private float requiredExp;
    private int level;

    private float expMultiple;

    private void Awake()
    {
        init();
    }

    private void init()
    {
        level = 1;
        exp = 0f;
        this.requiredExp = level * DEFAULT_EXP_INCREASEMENT;
    }
    
    // methods

    public void ApplyEnhanceOption(float value)
    {
        this.expMultiple = value;
    }
    public float CheckCurrentExp() { return exp; }

    public float CheckRequiredExp() { return requiredExp; }

    public void GainExp(int value)
    {
        this.exp += value * expMultiple;
        if (exp >= requiredExp) StartCoroutine(LevelUp());
    }

    public int GetLevel()
    {
        return level;
    }

    private IEnumerator LevelUp()
    {
        while (true)
        {
            if (exp < requiredExp) yield break;
            if (Time.timeScale == 0)
            {
                yield return new WaitForSeconds(Time.deltaTime);
                continue;
            }
            exp -= requiredExp;
            level++;
            requiredExp = level * DEFAULT_EXP_INCREASEMENT;
            UIManager.GetInstance().UpdatePlayerMaxStatus();
            UIManager.GetInstance().UpdatePlayerLevelStatus();
            UIManager.GetInstance().UpdateAugmentOptions(WeaponManager.GetInstance().SetAugmentOptions());
        }
    }
}
