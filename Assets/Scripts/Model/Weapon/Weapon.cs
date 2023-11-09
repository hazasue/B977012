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
    }

    protected static int DEFAULT_OBJECT_COUNT = 200;
    
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

    protected bool enableToAttack;

    protected RangeCollider rangeCollider;
    
    protected Queue<WeaponObject> weaponObjects;

    protected Transform instanceTransform;

    // methods
    public abstract void Init(WeaponInfo weaponInfo, RangeCollider rangeCollider);

    public abstract void UpgradeWeapon();

    protected abstract void InstantiateWeaponObjects();

    public abstract void ActivateWeaponObject(Vector3 attackDirection);

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
}
