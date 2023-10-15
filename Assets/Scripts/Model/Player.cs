using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    private enum PlayerState
    {
        idle,
        move,
        attack,
        die,
    }
    
    // attributes
    private float pickupRange;
    private int critical;
    private PlayerState playerState;

    // associations
    private Inventory inventory;
    private PlayerLevel playerLevel;
    private Skill skill;
    
    // Start is called before the first frame update
    void Awake()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        ChangePlayerState();

        switch (characterState)
        {
            case CharacterState.alive:
                switch (playerState)
                {
                    case PlayerState.idle:
                        break;
                    
                    case PlayerState.move:
                        Move();
                        break;
                    
                    case PlayerState.attack:
                        Move();
                        Attack();
                        break;
                    
                    case PlayerState.die:
                        break;
                    
                    default:
                        break;
                }
                break;
            
            case CharacterState.dead:
                break;
            
            default:
                break;
        }
    }
    
    // methods
    public override void Init()
    {
        characterState = Character.CharacterState.alive;
        playerState = PlayerState.idle;
        animator = this.GetComponent<Animator>();

        this.maxHp = 1;
        this.hp = this.maxHp;
        moveSpeed = 10f;
    }

    protected override void Move()
    {
        this.transform.position += moveDirection * (moveSpeed * Time.deltaTime);
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.LookRotation(attackDirection), DEFAULT_ROTATE_SPEED * Time.deltaTime);
    }
    
    protected override void Attack() {}
    
    protected override void UpdateStatus() {}

    public override void TakeDamage(int damage)
    {
        this.hp -= damage;
    }
    
    private void PickUpItems() {}

    public void GainExp(int value) {}

    public void GainCoins(int value) {}

    public Inventory GetInventory() { return inventory; }

    public PlayerLevel GetLevelInfo() { return playerLevel; }

    private void ChangePlayerState()
    {
        switch (playerState)
        {
            case PlayerState.idle:
                // check  'is player dead' -> 'is player time to attack' -> 'is player moving'
                if (hp <= 0)
                {
                    playerState = PlayerState.die;
                    animator.SetBool("dead", true);
                    characterState = Character.CharacterState.dead;
                }
                // else if (is time to attack)
                // {
                //      playerState = PlayerState.attack;
                //      animator.SetBool("isAttacking", true);
                // }
                else if (moveDirection != Vector3.zero)
                {
                    playerState = PlayerState.move;
                    animator.SetBool("isMoving", true);
                }

                break;
            
            case PlayerState.move:
                // check 'is player dead' -> 'is player time to attack' -> 'is player does nothing'
                if (hp <= 0)
                {
                    playerState = PlayerState.die;
                    animator.SetBool("dead", true);
                    characterState = Character.CharacterState.dead;
                }
                // else if (is time to attack)
                // {
                //      playerState = PlayerState.attack;
                //      animator.SetBool("isAttacking", true);
                // }
                else if (moveDirection == Vector3.zero)
                {
                    playerState = PlayerState.idle;
                    animator.SetBool("isMoving", false);
                }
                break;
            
            case PlayerState.attack:
                // check 'is player dead' -> 'does attack ended { moving now' -> is player does nothing}'
                if (hp <= 0)
                {
                    playerState = PlayerState.die;
                    animator.SetBool("dead", true);
                    characterState = Character.CharacterState.dead;
                }
                //else if (attack ended)
                //{
                if (moveDirection != Vector3.zero)
                {
                    playerState = PlayerState.move;
                    animator.SetBool("isMoving", true);
                }
                else
                {
                    playerState = PlayerState.idle;
                    animator.SetBool("isMoving", false);
                }
                //}
                
                break;
            
            case PlayerState.die:
                // change Character State to be 'dead'

                break;
            
            default:
                break;
        }
    }
}
