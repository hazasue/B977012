using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeWeapon : Weapon
{
    public Queue<WeaponObject> effects;
    
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
        weaponType = Weapon.WeaponType.GRENADE;

        audioSource = this.GetComponent<AudioSource>();
        audioClip = Resources.Load<AudioClip>($"Sfxs/weapons/{code}_sound");
        SoundManager.GetInstance().AddToSfxList(audioSource);
        audioSource.volume = SoundManager.GetInstance().audioSourceSfx.volume;
        audioSource.clip = audioClip;

        switch(weaponInfo.GetOccupation()){
            case "WARRIOR":
                weaponOccupation = Weapon.WeaponOccupation.WARRIOR;
                break;

            case "WIZARD":
                weaponOccupation = Weapon.WeaponOccupation.WIZARD;
                break;

            case "common":
                weaponOccupation = Weapon.WeaponOccupation.COMMON;
                break;

            case "synthesis":
                weaponOccupation = Weapon.WeaponOccupation.SYNTHESIS;
                break;

            default:
                Debug.Log($"Invalid Weapon Occupation: {weaponInfo.GetOccupation()}");
                break;
        }

        upgradeCount = 1;

        enableToAttack = false;
        
        weaponObjects = new Queue<WeaponObject>();
        effects = new Queue<WeaponObject>();

        instanceTransform = GameObject.Find("Projectile Transform").transform;

        this.rangeCollider = rangeCollider;
        this.rangeCollider.Init(range);
        
        InstantiateWeaponObjects();
        instantiateEffects();
        
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

    private void instantiateEffects()
    {
        WeaponObject tempObject;
        for (int i = 0; i < DEFAULT_OBJECT_COUNT; i++)
        {
            tempObject =
                Instantiate(Resources.Load<WeaponObject>("Prefabs/weapons/" + code + "_effect"), instanceTransform, true);
            effects.Enqueue(tempObject);
            tempObject.gameObject.SetActive(false);
        }
    }

    public override void ActivateWeaponObject(Vector3 attackDirection)
    {
        if (!enableToAttack) return;

        enableToAttack = false;
        
        WeaponObject tempObject = weaponObjects.Dequeue();
        tempObject.gameObject.SetActive(true);
        tempObject.Init(damage, speed, attackDirection, weaponType, weaponOccupation, audioClip);
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
        weaponObjects.Enqueue(tempObject);
        Vector3 targetPosition;

        if (tempEnemy) targetPosition = tempEnemy.transform.position - this.transform.position;
        else
        {
            targetPosition = new Vector3(Random.Range(-range, range), 0f, Random.Range(-range, range));
        }

        tempObject.gameObject.SetActive(true);
        tempObject.transform.position = this.transform.position + DEFAULT_OBJECT_POS_Y;
        tempObject.Init(0, targetPosition.magnitude, targetPosition.normalized, weaponType, weaponOccupation, null);

        StartCoroutine(instantiateEffect(tempObject.transform, DEFAULT_EFFECT_DELAY));
        StartCoroutine(InactivateWeaponObject(tempObject, DEFAULT_EFFECT_DELAY));
        StartCoroutine(ActivateWeaponObjectAuto());
    }

    private IEnumerator instantiateEffect(Transform pos, float delay)
    {
        yield return new WaitForSeconds(delay);

        WeaponObject tempEffect = effects.Dequeue();
        effects.Enqueue(tempEffect);
        tempEffect.gameObject.SetActive(true);
        tempEffect.transform.position = pos.position;
        tempEffect.Init(damage, 0f, Vector3.zero, weaponType, weaponOccupation, null);

        audioSource.Play();

        StartCoroutine(InactivateWeaponObject(tempEffect, duration));
    }
}