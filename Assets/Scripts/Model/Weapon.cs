using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum WeaponType
    {
        MELEE,
        RANGED,
        TRACKING,
        CHAINING,
        BEAM,
        EXPLOSIVE,
    }

    private static int DEFAULT_OBJECT_COUNT = 200;
    
    // attributes
    private string code;
    private string name;
    private int damage;
    private float duration;
    private float delay;
    private int projectile;
    private float range;
    private float speed;
    private WeaponType weaponType;

    private bool enableToAttack;

    private Queue<WeaponObject> weaponObjects;

    private Transform instanceTransform;

    // methods
    public void Init(WeaponInfo weaponInfo)
    {
        code = weaponInfo.GetCode();
        name = weaponInfo.GetName();
        damage = weaponInfo.GetDamage();
        duration = weaponInfo.GetDuration();
        delay = weaponInfo.GetDelay();
        projectile = weaponInfo.GetProjectile();
        range = weaponInfo.GetRange();
        speed = weaponInfo.GetSpeed();
        weaponType = weaponInfo.GetType();

        enableToAttack = true;
        
        weaponObjects = new Queue<WeaponObject>();

        switch (weaponType)
        {
            case WeaponType.MELEE:
                instanceTransform = this.transform;
                break;
            
            case WeaponType.TRACKING:
                instanceTransform = GameObject.Find("Projectile Transform").transform;
                break;
            
            default:
                break;
        }

        InstantiateWeaponObjects();
        StartCoroutine(EnableToAttack());
    }
    
    public void UpgradeWeapon() {}

    private void InstantiateWeaponObjects()
    {
        WeaponObject tempObject;
        for (int i = 0; i < DEFAULT_OBJECT_COUNT; i++)
        {
            tempObject =
                Instantiate(Resources.Load<WeaponObject>("Prefabs/weapons/" + code + "_object"), instanceTransform, true);
            weaponObjects.Enqueue(tempObject);
            tempObject.gameObject.SetActive(false);
        }
    }

    public void ActiveWeaponObject(Vector3 attackDirection)
    {
        if (!enableToAttack) return;

        enableToAttack = false;

        WeaponObject tempObject = weaponObjects.Dequeue();
        tempObject.gameObject.SetActive(true);
        tempObject.Init(damage, speed, attackDirection, weaponType);
        tempObject.transform.position = this.transform.position;
        weaponObjects.Enqueue(tempObject);

        StartCoroutine(UnActiveWeaponObject(tempObject, duration));
        StartCoroutine(EnableToAttack());
    }

    private IEnumerator EnableToAttack()
    {
        yield return new WaitForSeconds(delay);

        this.transform.parent.parent.GetComponent<Player>().ControlAttackState(true);
        enableToAttack = true;
    }

    private IEnumerator UnActiveWeaponObject(WeaponObject weaponObject, float duration)
    {
        yield return new WaitForSeconds(duration);

        weaponObject.gameObject.SetActive(false);
    } 
}
