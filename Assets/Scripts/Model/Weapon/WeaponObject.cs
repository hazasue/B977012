using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponObject : MonoBehaviour
{
    private int damage;
    private float speed;
    private Vector3 attackDirection;
    private Weapon.WeaponType weaponType;
    private Weapon.WeaponOccupation weaponOccupation;

    public RangeCollider rangeCollider;

    private AudioSource audioSource;
    private AudioClip hitClip;
    
    // Update is called once per frame
    void Update()
    {
        switch (weaponType)
        {
            case Weapon.WeaponType.MELEE:
                break;
            
            case Weapon.WeaponType.RANGED:
                break;
            
            case Weapon.WeaponType.TRACKING:
                move();
                break;
            
            case Weapon.WeaponType.CHAINING:
                break;
            
            case Weapon.WeaponType.BEAM:
                break;
            
            case Weapon.WeaponType.BARRIER:
                spin();
                break;
            
            case Weapon.WeaponType.EXPLOSIVE:
                break;
            
            case Weapon.WeaponType.BOOMERANG:
                move();
                spin();
                if(weaponOccupation == Weapon.WeaponOccupation.SYNTHESIS) grab();
                this.speed -= 15 * Time.deltaTime;
                break;
            
            case Weapon.WeaponType.GRENADE:
                move();
                break;
            
            case Weapon.WeaponType.DELAYMELEE:
                break;
            
            case Weapon.WeaponType.COMBO:
                break;
            
            default:
                Debug.Log("Invalid weapontype: " + weaponType);
                break;
        }
    }

    public void Init(int damage, float speed, Vector3 attackDirection, Weapon.WeaponType weaponType, Weapon.WeaponOccupation weaponOccupation, AudioClip audioClip = null)
    {
        this.damage = damage;
        this.speed = speed;
        this.attackDirection = attackDirection;
        this.weaponType = weaponType;
        this.weaponOccupation = weaponOccupation;

        this.audioSource = this.GetComponent<AudioSource>();
        audioSource.volume = SoundManager.GetInstance().audioSourceSfx.volume;
        SoundManager.GetInstance().AddToSfxList(audioSource);
        hitClip = Resources.Load<AudioClip>($"Sfxs/weapons/hit_sound");

        if (audioClip == null) audioSource.clip = hitClip;
        else {
            this.audioSource.clip = audioClip;
        }

        if (weaponType == Weapon.WeaponType.BOOMERANG && weaponOccupation == Weapon.WeaponOccupation.SYNTHESIS) rangeCollider.Init(1.2f);

        if(audioClip != null) audioSource.Play();
    }

    private void move()
    {
        if (speed == 0f) return;
        this.transform.position += attackDirection * (Time.deltaTime * speed);
    }

    private void spin()
    {
        transform.Rotate(new Vector3(0f, -90f, 0f) * (speed * Time.deltaTime), Space.World);
    }

    private void grab() {
        foreach(Enemy enemy in rangeCollider.GetEnemies()) {
            enemy.transform.position -= (enemy.transform.position - this.transform.position).normalized * 0.05f;
        }
    }

    private void attack()
    {
        
    }

    public void OnTriggerEnter(Collider obj)
    {
        switch (obj.tag)
        {
            case "enemy":
                Enemy enemy = obj.gameObject.GetComponent<Enemy>();
                enemy.TakeDamage(damage);
                if (audioSource.clip == hitClip) audioSource.Play();
                if(weaponType == Weapon.WeaponType.DELAYMELEE){
                    enemy.transform.position += (enemy.transform.position - this.transform.position).normalized;
                }
                break;
            
            case "SupplyBox":
                obj.gameObject.GetComponent<SupplyBox>().DestroySupplyBox();
                break;
            
            default:
                break;
        }
    }
}
