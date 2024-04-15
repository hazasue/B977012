using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomerangWeapon : Weapon
{
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
        weaponType = Weapon.WeaponType.BOOMERANG;
        upgradeCount = 1;

        enableToAttack = false;
        
        weaponObjects = new Queue<WeaponObject>();

        instanceTransform = GameObject.Find("Projectile Transform").transform;

        this.rangeCollider = rangeCollider;
        this.rangeCollider.Init(range);
        
        InstantiateWeaponObjects();
        
        if (mainWeapon) StartCoroutine(EnableToAttack());
        else
        {
            StartCoroutine(ActivateWeaponObjectAuto());
        }
    }
    
    public override void UpgradeWeapon(WeaponUpgradeInfo upgradeInfo) {
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
        tempObject.transform.rotation = Quaternion.LookRotation(-attackDirection);
        
        weaponObjects.Enqueue(tempObject);

        StartCoroutine(InactivateWeaponObject(tempObject, duration));
        StartCoroutine(EnableToAttack());
    }
    
    public override IEnumerator ActivateWeaponObjectAuto()
    {
        yield return new WaitForSeconds(delay);
        
        Enemy tempEnemy = rangeCollider.GetClosestEnemy();

        WeaponObject tempObject = weaponObjects.Dequeue();
        tempObject.gameObject.SetActive(true);
        tempObject.transform.position = this.transform.position + DEFAULT_OBJECT_POS_Y;

        if(tempEnemy) tempObject.Init(damage, speed, (tempEnemy.transform.position - this.transform.position).normalized, weaponType);
        else
        {
            tempObject.Init(damage, speed, new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized, weaponType);
        }
        
        weaponObjects.Enqueue(tempObject);

        StartCoroutine(InactivateWeaponObject(tempObject, duration));
        StartCoroutine(ActivateWeaponObjectAuto());
    }
}
