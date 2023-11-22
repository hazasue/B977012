using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    public enum SkillType
    {
        BUFF,
        DEALING,
    }

    private SkillType skillType;
    private float delay;
    private float duration;
    private string stat;
    private float value;
    private int damage;
    private int projectile;

    public void Init(SkillInfo skillInfo)
    {
        
    }

    public void UseSkill()
    {
        
    }
}
