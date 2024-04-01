using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierWeapon : Weapon
{
    private const int DEFAULT_BASIC_COLLIDER_OBJECT_COUNT = 3;
    
    public override void Init(WeaponInfo weaponInfo, RangeCollider rangeCollider, float damageMultiple, bool mainWeapon = false)
    {
        this.damageMultiple = damageMultiple;
        code = weaponInfo.GetCode();
        name = weaponInfo.GetName();
        damage = (int)(weaponInfo.GetDamage() * damageMultiple);
        duration = weaponInfo.GetDuration();
        delay = weaponInfo.GetDelay();
        projectile = weaponInfo.GetProjectile();
        range = weaponInfo.GetRange();
        speed = weaponInfo.GetSpeed();
        weaponType = Weapon.WeaponType.BARRIER;
        upgradeCount = 1;

        enableToAttack = false;
        
        weaponObjects = new Queue<WeaponObject>();

        instanceTransform = this.transform;

        this.rangeCollider = rangeCollider;
        this.rangeCollider.Init(range);
        
        InstantiateWeaponObjects();

        if (mainWeapon) StartCoroutine(EnableToAttack());
        else
        {
            StartCoroutine(ActivateWeaponObjectAuto());
        }
    }

    public override void UpgradeWeapon(WeaponUpgradeInfo upgradeInfo)
    {
        switch (upgradeInfo.option1)
        {
            case NONE_OPTION_STRING:
                break;
            
            case "damage":
                this.damage += (int)(upgradeInfo.value1 * damageMultiple);
                break;
            
            case "duration":
                this.duration += upgradeInfo.value1;
                break;
            
            case "delay":
                this.delay -= upgradeInfo.value1;
                break;
            
            case "projectile":
                this.projectile += (int)upgradeInfo.value1;
                break;
            
            case "speed":
                this.speed += upgradeInfo.value1;
                break;
            
            default:
                Debug.Log("Unmatched upgrade option: " + this.code + " " + upgradeInfo.option1);
                break;
        }

        switch (upgradeInfo.option2)
        {
            case NONE_OPTION_STRING:
                break;
            
            case "damage":
                this.damage += (int)(upgradeInfo.value2 * damageMultiple);
                break;
            
            case "duration":
                this.duration += upgradeInfo.value2;
                break;
            
            case "delay":
                this.delay -= upgradeInfo.value2;
                break;
            
            case "projectile":
                this.projectile += (int)upgradeInfo.value2;
                break;
            
            case "speed":
                this.speed += upgradeInfo.value2;
                break;
            
            default:
                Debug.Log("Unmatched upgrade option: " + this.code + " " + upgradeInfo.option2);
                break;
        }

        upgradeCount++;
    }

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

        enableToAttack = false;
        
        WeaponObject tempObject = weaponObjects.Dequeue();
        tempObject.gameObject.SetActive(true);
        tempObject.Init(damage, speed, attackDirection, weaponType);
        tempObject.transform.position = this.transform.position;
        
        weaponObjects.Enqueue(tempObject);

        StartCoroutine(InactivateWeaponObject(tempObject, duration));
        StartCoroutine(EnableToAttack());
    }

    public override IEnumerator ActivateWeaponObjectAuto()
    {
        float angle = 360f / DEFAULT_BASIC_COLLIDER_OBJECT_COUNT;

        WeaponObject tempObject;
        for (int i = 0; i < DEFAULT_BASIC_COLLIDER_OBJECT_COUNT; i++)
        {
            tempObject = weaponObjects.Dequeue();
            weaponObjects.Enqueue(tempObject);
            tempObject.gameObject.SetActive(true);
            tempObject.transform.localRotation = Quaternion.Euler(new Vector3(90f, angle * i, 0f));
            tempObject.transform.position = this.transform.position + DEFAULT_OBJECT_POS_Y;
            tempObject.Init(damage, speed, Vector3.zero, weaponType);
            StartCoroutine(InactivateWeaponObject(tempObject, duration));
        }

        yield return new WaitForSeconds(delay);
        StartCoroutine(ActivateWeaponObjectAuto());
    }
}
