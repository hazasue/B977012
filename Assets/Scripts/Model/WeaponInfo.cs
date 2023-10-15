using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponInfo : MonoBehaviour
{
    private string code;
    private string name;
    private Weapon.WeaponType weaponType;
    private int damage;
    private float duration;
    private float delay;
    private int projectile;
    private float range;

    public WeaponInfo(string code, string name, string type, int damage, float duration, float delay, int projectile, float range)
    {
        this.code = code;
        this.name = name;
        this.damage = damage;
        this.duration = duration;
        this.delay = delay;
        this.projectile = projectile;
        this.range = range;

        switch (type)
        {
            case "MELEE":
                this.weaponType = Weapon.WeaponType.MELEE;
                break;

            case "RANGED":
                this.weaponType = Weapon.WeaponType.RANGED;
                break;
            
            case "TRACKING":
                this.weaponType = Weapon.WeaponType.TRACKING;
                break;
            
            case "CHAINING":
                this.weaponType = Weapon.WeaponType.CHAINING;
                break;
            
            case "BEAM":
                this.weaponType = Weapon.WeaponType.BEAM;
                break;
            
            case "EXPLOSIVE":
                this.weaponType = Weapon.WeaponType.EXPLOSIVE;
                break;
            
            default:
                Debug.Log("Invalid weapon type: " + type);
                break;

        }
    }

    public string GetCode() { return code; }

    public string GetName() { return name; }

    public Weapon.WeaponType GetType() { return new WeaponType(); }

    public int GetDamage() { return damage; }
    
    public float GetDuration() { return duration; }
    
    public float GetDelay() { return delay; }
    
    public int GetProjectile() { return projectile; }
    
    public float GetRange() { return range; }
}
