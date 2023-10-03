using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLevel : MonoBehaviour
{
    // attibutes
    private int exp;
    private int requiredExp;
    private int level;
    
    // methods
    public int CheckExp()
    {
        return exp;
    }
    
    public void GainExp() {}

    public int GetLevel()
    {
        return level;
    }
    
    private void LevelUp() {}
}
