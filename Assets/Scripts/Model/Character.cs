using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    public enum CharacterState
    {
        ALIVE,
        DEAD,
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
    public abstract void Init(int maxHp, int damage, float speed, int armor);
    
    protected abstract void Move();
    
    protected abstract void Attack();
    
    protected abstract void UpdateStatus();
   
    public abstract void TakeDamage(int damage);

    public void SetDirections(Vector3 direction)
    {
        this.moveDirection = direction;
        if (direction != Vector3.zero) this.attackDirection = direction;
    }

    public CharacterState GetCharacterState()
    {
        return characterState;
    }
}
