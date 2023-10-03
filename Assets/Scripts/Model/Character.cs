using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public enum CharacterStatus
    {
        alive,
        dead,
    }

    // attributes
    protected CharacterStatus characterStatus;
    protected int maxHp;
    protected int hp;
    protected int armor;
    protected float damage;
    protected float moveSpeed;
    protected float attackSpeed;
    protected Vector3 moveDirection;
    protected Vector3 attackDirection;
    
    // methods
    public virtual void Init() {}
    
    protected void Move() {}
    
    protected virtual void Attack() {}
    
    protected void UpdateStatus() {}

    public CharacterStatus GetCharStatus()
    {
        return characterStatus;
    }
    
    public void TakeDamage() {}

}
