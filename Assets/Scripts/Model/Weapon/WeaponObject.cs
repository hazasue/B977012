using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponObject : MonoBehaviour
{
    private int damage;
    private float speed;
    private Vector3 attackDirection;
    private Weapon.WeaponType weaponType;
    
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
                this.speed -= 10 * Time.deltaTime;
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

    public void Init(int damage, float speed, Vector3 attackDirection, Weapon.WeaponType weaponType)
    {
        this.damage = damage;
        this.speed = speed;
        this.attackDirection = attackDirection;
        this.weaponType = weaponType;
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
                break;
            
            case "SupplyBox":
                obj.gameObject.GetComponent<SupplyBox>().DestroySupplyBox();
                break;
            
            default:
                break;
        }
    }
}
