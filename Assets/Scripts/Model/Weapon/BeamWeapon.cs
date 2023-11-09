using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamWeapon : Weapon
{
    public override void Init(WeaponInfo weaponInfo, RangeCollider rangeCollider)
    {
        code = weaponInfo.GetCode();
        name = weaponInfo.GetName();
        damage = weaponInfo.GetDamage();
        duration = weaponInfo.GetDuration();
        delay = weaponInfo.GetDelay();
        projectile = weaponInfo.GetProjectile();
        range = weaponInfo.GetRange();
        speed = weaponInfo.GetSpeed();
        weaponType = Weapon.WeaponType.BEAM;

        enableToAttack = true;
        
        weaponObjects = new Queue<WeaponObject>();

        instanceTransform = GameObject.Find("Projectile Transform").transform;

        this.rangeCollider = rangeCollider;
        this.rangeCollider.Init(range);
        
        InstantiateWeaponObjects();
        StartCoroutine(EnableToAttack());
    }
    
    public override void UpgradeWeapon() {}

    protected override void InstantiateWeaponObjects()
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

    public override void ActivateWeaponObject(Vector3 attackDirection)
    {
        if (!enableToAttack) return;
        StartCoroutine(EnableToAttack());

        enableToAttack = false;

        WeaponObject tempObject = weaponObjects.Dequeue();

        Enemy targetEnemy = rangeCollider.GetClosestEnemy();
        if (targetEnemy == null) return;
        tempObject.transform.rotation =
            Quaternion.LookRotation(this.gameObject.transform.position - targetEnemy.transform.position);

        tempObject.gameObject.SetActive(true);
        tempObject.Init(damage, speed, attackDirection, weaponType);
        tempObject.transform.position = this.transform.position;

        weaponObjects.Enqueue(tempObject);

        StartCoroutine(InactivateWeaponObject(tempObject, duration));
        
    }
}
