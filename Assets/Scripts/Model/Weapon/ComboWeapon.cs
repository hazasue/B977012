using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboWeapon : Weapon
{
    private const float DEFAULT_DELAY = 0.5f;

    private Queue<WeaponObject> effects;

    private string code1;
    private string code2;

    private AudioClip audioClip1;
    private AudioClip audioClip2;
    
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
        weaponType = Weapon.WeaponType.COMBO;

        audioSource = this.GetComponent<AudioSource>();
        audioSource.volume = SoundManager.GetInstance().audioSourceSfx.volume;
        SoundManager.GetInstance().AddToSfxList(audioSource);
        audioSource.clip = audioClip1;

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

        code1 = weaponInfo.ingr1;
        code2 = weaponInfo.ingr2;

        audioClip1 = Resources.Load<AudioClip>($"Sfxs/weapons/{code1}_sound");
        audioClip2 = Resources.Load<AudioClip>($"Sfxs/weapons/{code2}_sound");

        instanceTransform = this.transform;

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
            for (int idx = 0; idx < projectile - 1; idx++)
            {
                tempObject =
                    Instantiate(Resources.Load<WeaponObject>("Prefabs/weapons/" + code1 + "_object"), instanceTransform, true);
                weaponObjects.Enqueue(tempObject);
                tempObject.gameObject.SetActive(false);
            }
            tempObject =
                Instantiate(Resources.Load<WeaponObject>("Prefabs/weapons/" + code2 + "_object"), instanceTransform, true);
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
                Instantiate(Resources.Load<WeaponObject>("Prefabs/weapons/" + code2 + "_effect"), instanceTransform, true);
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
        tempObject.transform.position += DEFAULT_OBJECT_POS_Y;
        
        weaponObjects.Enqueue(tempObject);
        
        StartCoroutine(InactivateWeaponObject(tempObject, duration));
        StartCoroutine(EnableToAttack());
    }
    
    public override IEnumerator ActivateWeaponObjectAuto()
    {
        yield return new WaitForSeconds(delay);
     
        WeaponObject tempObject;
        for (int i = 0; i < projectile - 1; i++)
        {
            tempObject = weaponObjects.Dequeue();
            weaponObjects.Enqueue(tempObject);
            tempObject.gameObject.SetActive(true);
            tempObject.Init(damage, speed, Vector3.zero, weaponType, weaponOccupation, audioClip1);
            tempObject.transform.position = this.transform.position;
            tempObject.transform.position += DEFAULT_OBJECT_POS_Y;
            tempObject.transform.rotation = player.rotation * Quaternion.Euler(new Vector3(0f, 0f, 180f * i));
            StartCoroutine(InactivateWeaponObject(tempObject, DEFAULT_DELAY));
            yield return new WaitForSeconds(DEFAULT_DELAY / 2);
        }
        
        tempObject = weaponObjects.Dequeue();
        weaponObjects.Enqueue(tempObject);
        tempObject.gameObject.SetActive(true);
        tempObject.Init(damage, speed, Vector3.zero, weaponType, weaponOccupation, audioClip2);
        tempObject.transform.position = this.transform.position;
        tempObject.transform.position += DEFAULT_OBJECT_POS_Y;
        tempObject.transform.rotation = player.rotation * Quaternion.Euler(new Vector3(0f, 0f, 90f));
        
        StartCoroutine(instantiateEffect(tempObject.transform, DEFAULT_DELAY));
        StartCoroutine(InactivateWeaponObject(tempObject, DEFAULT_DELAY + duration));
        StartCoroutine(ActivateWeaponObjectAuto());
    }
    
    private IEnumerator instantiateEffect(Transform pos, float delay)
    {
        yield return new WaitForSeconds(delay);

        WeaponObject tempEffect = effects.Dequeue();
        effects.Enqueue(tempEffect);
        tempEffect.gameObject.SetActive(true);
        tempEffect.transform.position = pos.position;
        tempEffect.transform.rotation = pos.rotation;
        tempEffect.Init(damage, 0f, Vector3.zero, Weapon.WeaponType.DELAYMELEE, weaponOccupation, null);

        StartCoroutine(InactivateWeaponObject(tempEffect, duration));
    }
}
