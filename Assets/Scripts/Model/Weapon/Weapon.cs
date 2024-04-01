using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public enum WeaponType
    {
        MELEE,
        RANGED,
        TRACKING,
        CHAINING,
        BEAM,
        BARRIER,
        EXPLOSIVE,
        BOOMERANG
    }

    protected static int DEFAULT_OBJECT_COUNT = 200;
    protected const string NONE_OPTION_STRING = "";
    protected static Vector3 DEFAULT_OBJECT_POS_Y = new Vector3(0f, 0.5f, 0f);
    
    // attributes
    protected string code;
    protected string name;
    protected int damage;
    protected float duration;
    protected float delay;
    protected int projectile;
    protected float range;
    protected float speed;
    protected WeaponType weaponType;
    protected bool mainWeapon;

    protected float damageMultiple;

    protected int upgradeCount;

    protected bool enableToAttack;

    protected RangeCollider rangeCollider;
    
    protected Queue<WeaponObject> weaponObjects;

    protected Transform instanceTransform;

    // methods
    public abstract void Init(WeaponInfo weaponInfo, RangeCollider rangeCollider,  float damageMultiple, bool mainWeapon = false);

    public abstract void UpgradeWeapon(WeaponUpgradeInfo upgradeInfo);

    protected abstract void InstantiateWeaponObjects();

    public abstract void ActivateWeaponObject(Vector3 attackDirection);

    public abstract IEnumerator ActivateWeaponObjectAuto();

    public IEnumerator ApplyBuffSkill(string stat, float value, float duration)
    {
        switch (stat)
        {
            case "damage":
                this.damage += (int)value;
                break;
            
            case "duration":
                this.duration += value;
                break;
            
            case "delay":
                this.delay -= value;
                break;
            
            case "projectile":
                this.projectile += (int)value;
                break;
            
            case "speed":
                this.speed += value;
                break;
            
            default:
                Debug.Log("Unmatched buff stat: " + this.code + " " + stat);
                break;
        }

        yield return new WaitForSeconds(duration);
        
        switch (stat)
        {
            case "damage":
                this.damage -= (int)value;
                break;
            
            case "duration":
                this.duration -= value;
                break;
            
            case "delay":
                this.delay += value;
                break;
            
            case "projectile":
                this.projectile -= (int)value;
                break;
            
            case "speed":
                this.speed -= value;
                break;
            
            default:
                Debug.Log("Unmatched buff stat: " + this.code + " " + stat);
                break;
        }
    }

    protected IEnumerator EnableToAttack()
    {
        yield return new WaitForSeconds(delay);

        enableToAttack = true;
        this.transform.parent.parent.GetComponent<Player>().ControlAttackState(true);
    }

    protected IEnumerator InactivateWeaponObject(WeaponObject weaponObject, float duration)
    {
        yield return new WaitForSeconds(duration);

        weaponObject.gameObject.SetActive(false);
    }

    public int GetUpgradeCount() { return upgradeCount; }

    public string GetCode() { return code; }
}
