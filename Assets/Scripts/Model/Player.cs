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

    private static float DEFAULT_ATTACK_ANIM_DURATION = 0.5f;
    
    // attributes
    private float pickupRange;
    private int critical;
    private bool timeToAttack;
    private PlayerState playerState;

    // associations
    [SerializeField]
    private Inventory inventory;
    [SerializeField]
    private PlayerLevel playerLevel;
    private Skill skill;

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 0) return;
        
        ChangePlayerState();

        switch (characterState)
        {
            case CharacterState.ALIVE:
                ApplyKeyInput();
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

        this.attackDirection = new Vector3(1f, 0f, 0f);

        timeToAttack = false;
    }

    protected override void Move()
    {
        this.transform.position += moveDirection * (moveSpeed * Time.deltaTime);
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.LookRotation(attackDirection), DEFAULT_ROTATE_SPEED * Time.deltaTime);
        inventory.transform.rotation = Quaternion.Euler(0f, -this.transform.rotation.y, 0f);
    }

    protected override void Attack()
    {
        foreach (KeyValuePair<string, Weapon> weapon in inventory.GetWeapons())
        {
            weapon.Value.ActivateWeaponObject(attackDirection);
        }
    }
    
    protected override void UpdateState() {}

    private void ApplyKeyInput()
    {
        Vector3 direction = Vector3.zero;
        if (Input.GetKey(KeyCode.UpArrow))
        {
            direction += Vector3.forward;
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            direction += Vector3.back;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            direction += Vector3.left;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            direction += Vector3.right;
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            inventory.GetSkill().UseSkill(inventory.GetWeapons()[WeaponManager.GetInstance().GetBasicWeaponCode()]);
        }
        
        direction = direction.normalized;
        SetDirections(direction);
    }

    protected override void SetDirections(Vector3 direction)
    {
        this.moveDirection = direction;
        if (this.moveDirection != Vector3.zero) this.attackDirection = this.moveDirection;
    }
    
    public override void TakeDamage(int damage)
    {
        if (damage <= armor) return;
        
        this.hp -= damage - armor;
        UIManager.GetInstance().UpdatePlayerCurrentStatus();
    }
    
    private void PickUpItems() {}

    public Inventory GetInventory() { return inventory; }

    public PlayerLevel GetLevelInfo() { return playerLevel; }

    public int GetMaxHp() { return maxHp; }
    
    public int GetCurrentHp() { return hp;}

    private void ChangePlayerState()
    {
        switch (playerState)
        {
            case PlayerState.IDLE:
                // check  'is player time to ATTACK' -> 'is player moving'
                if (timeToAttack)
                {
                    playerState = PlayerState.ATTACK;
                    animator.SetBool("isAttacking", true);
                    StartCoroutine(StopAttackAnim());
                }
                else if (moveDirection != Vector3.zero)
                {
                    playerState = PlayerState.MOVE;
                    animator.SetBool("isMoving", true);
                }

                break;
            
            case PlayerState.MOVE:
                // check 'is player time to ATTACK' -> 'is player does nothing'
                if (timeToAttack)
                {
                    playerState = PlayerState.ATTACK;
                    animator.SetBool("isAttacking", true);
                    StartCoroutine(StopAttackAnim());
                }
                else if (moveDirection == Vector3.zero)
                {
                    playerState = PlayerState.IDLE;
                    animator.SetBool("isMoving", false);
                }
                break;
            
            case PlayerState.ATTACK:
                // check 'does ATTACK ended { moving now' -> is player does nothing}'
                if (timeToAttack) break;

                animator.SetBool("isAttacking", false);
                
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
                    die();
                }
                break;
            
            case Character.CharacterState.DEAD:
                break;
            
            default:
                break;
        }
    }

    private void die()
    {
        characterState = Character.CharacterState.DEAD;
        animator.SetBool("dead", true);
        GameManager.GetInstance().FailGame();
        GameManager.GetInstance().AddReward();
    }

    public void ControlAttackState(bool timeToAttack)
    {
        this.timeToAttack = timeToAttack;
    }

    private IEnumerator StopAttackAnim()
    {
        yield return new WaitForSeconds(DEFAULT_ATTACK_ANIM_DURATION);

        timeToAttack = false;
    }

    public void OnTriggerEnter(Collider obj)
    {
        switch (obj.tag)
        {
            case "exp":
                playerLevel.GainExp(obj.GetComponent<Item>().GetValue());
                UIManager.GetInstance().UpdatePlayerCurrentStatus();
                obj.gameObject.SetActive(false);
                break;
            
            case "coin":
                inventory.GainCoins(obj.GetComponent<Item>().GetValue());
                obj.gameObject.SetActive(false);
                break;
            
            case "itemType":
                break;
        }
    }
}
