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
                Move();
                break;
            
            case Weapon.WeaponType.CHAINING:
                break;
            
            case Weapon.WeaponType.BEAM:
                break;
            
            case Weapon.WeaponType.EXPLOSIVE:
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

    private void Move()
    {
        this.transform.position += attackDirection * (Time.deltaTime * speed);
    }

    private void Attack()
    {
        
    }
}
