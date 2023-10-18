using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
	public enum PlayerType
	{
		WARRIOR,
		WIZARD,
		HUNTER,
	}

    private enum PlayerState
    {
        IDLE,
        MOVE,
        ATTACK,
    }
    
    // attributes
    private float pickupRange;
    private int critical;
    private PlayerState playerState;

    // associations
    [SerializeField]
    private Inventory inventory;
    [SerializeField]
    private PlayerLevel playerLevel;
    private Skill skill;

    void Start()
    {
        inventory.AddWeapon(WeaponManager.GetInstance().GetWeaponInfo(JsonManager.GetInstance()
            .LoadJsonFile<CharacterData>(JsonManager.DEFAULT_CHARACTER_DATA_NAME).basicWeapon));
    }

    // Update is called once per frame
    void Update()
    {
        ChangePlayerState();

        switch (characterState)
        {
            case CharacterState.ALIVE:
                switch (playerState)
                {
                    case PlayerState.IDLE:
                        break;
                    
                    case PlayerState.MOVE:
                        Move();
                        break;
                    
                    case PlayerState.ATTACK:
                        Move();
                        Attack();
                        break;

                    default:
                        break;
                }
                break;
            
            case CharacterState.DEAD:
                break;
            
            default:
                break;
        }
    }
    
    // methods
    public override void Init(int maxHp, int damage, float moveSpeed, int armor)
    {
        characterState = Character.CharacterState.ALIVE;
        playerState = PlayerState.IDLE;
        animator = this.GetComponent<Animator>();

        this.maxHp = maxHp;
        this.hp = this.maxHp;
        this.damage = damage;
        this.moveSpeed = moveSpeed;
        this.armor = armor;
    }

    protected override void Move()
    {
        this.transform.position += moveDirection * (moveSpeed * Time.deltaTime);
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.LookRotation(attackDirection), DEFAULT_ROTATE_SPEED * Time.deltaTime);
    }

    protected override void Attack()
    {
        foreach (Weapon weapon in inventory.GetWeapons())
        {
            weapon.ActiveWeaponObject(attackDirection);
        }
    }
    
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
            case PlayerState.IDLE:
                // check  'is player time to ATTACK' -> 'is player moving'
                // else if (is time to ATTACK)
                // {
                //      playerState = PlayerState.ATTACK;
                //      animator.SetBool("isAttacking", true);
                // }
                if (moveDirection != Vector3.zero)
                {
                    playerState = PlayerState.MOVE;
                    animator.SetBool("isMoving", true);
                }

                break;
            
            case PlayerState.MOVE:
                // check 'is player time to ATTACK' -> 'is player does nothing'
                // else if (is time to ATTACK)
                // {
                //      playerState = PlayerState.ATTACK;
                //      animator.SetBool("isAttacking", true);
                // }
                if (moveDirection == Vector3.zero)
                {
                    playerState = PlayerState.IDLE;
                    animator.SetBool("isMoving", false);
                }
                break;
            
            case PlayerState.ATTACK:
                // check 'does ATTACK ended { moving now' -> is player does nothing}'
                //else if (ATTACK ended)
                //{
                if (moveDirection != Vector3.zero)
                {
                    playerState = PlayerState.MOVE;
                    animator.SetBool("isMoving", true);
                }
                else
                {
                    playerState = PlayerState.IDLE;
                    animator.SetBool("isMoving", false);
                }
                //}
                
                break;

            default:
                break;
        }

        switch (characterState)
        {
            case Character.CharacterState.ALIVE:
                if (hp <= 0)
                {
                    characterState = Character.CharacterState.DEAD;
                    animator.SetBool("dead", true);
                }
                break;
            
            case Character.CharacterState.DEAD:
                break;
            
            default:
                break;
        }
    }
}
