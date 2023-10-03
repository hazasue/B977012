using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private enum WeaponType
    {
        melee,
        ranged,
        tracking,
        chaining,
        beam,
        explosive,
    }
    
    // attributes
    private float damage;
    private float duration;
    private float delay;
    private int projectile;
    private float range;
    private WeaponType weaponType;

    // methods
    public void Init() {}
    
    public void UpgradeWeapon() {}
    
    private void InstantiateWeaponObjects() {}

    private IEnumerator ActiveWeaponObject()
    {
        yield break;
    }
}
