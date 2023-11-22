using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponInfo
{
    public string code;
    public string name;
    public string weaponType;
    public int damage;
    public float duration;
    public float delay;
    public int projectile;
    public float range;
    public float speed;
    public string occupation;
    public int order;
    public int price;

    public WeaponInfo(string code, string name, string type, int damage, float duration, float delay, int projectile, float range, float speed, string occupation, int order, int price)
    {
        this.code = code;
        this.name = name;
        this.damage = damage;
        this.duration = duration;
        this.delay = delay;
        this.projectile = projectile;
        this.range = range;
        this.speed = speed;
        this.weaponType = type;
        this.occupation = occupation;
        this.order = order;
        this.price = price;
    }

    public string GetCode() { return code; }

    public string GetName() { return name; }

    public string GetType() { return weaponType; }

    public int GetDamage() { return damage; }
    
    public float GetDuration() { return duration; }
    
    public float GetDelay() { return delay; }
    
    public int GetProjectile() { return projectile; }
    
    public float GetRange() { return range; }

    public float GetSpeed() { return speed; }

    public string GetOccupation() { return occupation; }

    public int GetOrder() { return order; }

    public int GetPrice() { return price; }
}
