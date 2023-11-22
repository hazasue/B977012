using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillInfo
{
    public string code;
    public string skillType;
    public float delay;
    public float duration;
    public string stat;
    public float value;
    public int damage;
    public int projectile;

    public SkillInfo(string code, string skillType, float delay, float duration, string stat, float value, int damage,
        int projectile)
    {
        this.code = code;
        this.skillType = skillType;
        this.delay = delay;
        this.duration = duration;
        this.stat = stat;
        this.value = value;
        this.damage = damage;
        this.projectile = projectile;
    }
}
