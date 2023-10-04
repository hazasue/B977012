using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public enum CharacterState
    {
        alive,
        dead,
    }

    protected static float DEFAULT_ROTATE_SPEED = 10f;

    // attributes
    protected CharacterState characterState;
    protected int maxHp;
    protected int hp;
    protected int armor;
    protected float damage;
    protected float moveSpeed;
    protected float attackSpeed;
    protected Vector3 moveDirection;
    protected Vector3 attackDirection;
    protected Animator animator;
    
    // methods
    public virtual void Init() {}
    
    protected virtual void Move() {}
    
    protected virtual void Attack() {}
    
    protected void UpdateStatus() {}

    public void SetDirections(Vector3 direction)
    {
        this.moveDirection = direction;
        if (direction != Vector3.zero) this.attackDirection = direction;
    }

    public CharacterState GetCharacterState()
    {
        return characterState;
    }
    
    public void TakeDamage() {}

}
