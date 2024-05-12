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
    private const int MAX_SUPPLY_COUNT = 2;
    private const int DEFAULT_HEAL_VALUE = 50;
    private const float DEFAULT_MAGNET_DURATION = 3f;
    private const float DEFAULT_BOMB_DURATION = 2f;
    private const float DEFAULT_HEAL_DURATION = 1.5f;
    
    // attributes
    private float pickupRange;
    private int critical;
    private bool timeToAttack;
    private PlayerState playerState;

    private float coinMultiple;
    private float expMultiple;
    private float damageMultiple;

    private AudioSource audioSource;
    private AudioClip expClip;
    private AudioClip damageClip;

    private Rigidbody rigidBody;

    private List<Supply.SupplyType> supplies;
    private Supply.SupplyType tempSupply;
    private int supplyCount;

    public GameObject bomb;
    public GameObject magnet;
    public GameObject heal;

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
                applyKeyInput();
                switch (playerState)
                {
                    case PlayerState.IDLE:
                        break;
                    
                    case PlayerState.MOVE:
                        move();
                        break;
                    
                    case PlayerState.ATTACK:
                        move();
                        attack();
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
        this.coinMultiple = 1f;
        this.expMultiple = 1f;
        this.damageMultiple = 1f;

        this.attackDirection = new Vector3(1f, 0f, 0f);

        supplies = new List<Supply.SupplyType>();
        supplies.Add(Supply.SupplyType.NONE);
        supplies.Add(Supply.SupplyType.NONE);
        supplyCount = 0; 

        audioSource = this.GetComponent<AudioSource>();
        audioSource.volume = SoundManager.GetInstance().audioSourceSfx.volume;
        SoundManager.GetInstance().AddToSfxList(audioSource);
        expClip = Resources.Load<AudioClip>($"Sfxs/players/exp_sound");
        damageClip = Resources.Load<AudioClip>($"Sfxs/players/damage_sound");

        rigidBody = this.GetComponent<Rigidbody>();

        timeToAttack = false;
    }

    protected override void move()
    {
        this.transform.position += moveDirection * (moveSpeed * Time.deltaTime);
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.LookRotation(attackDirection), DEFAULT_ROTATE_SPEED * Time.deltaTime);
        inventory.transform.rotation = Quaternion.Euler(0f, -this.transform.rotation.y, 0f);
    }

    protected override void attack()
    {
        foreach (KeyValuePair<string, Weapon> weapon in inventory.GetWeapons())
        {
            weapon.Value.ActivateWeaponObject(attackDirection);
        }
    }
    
    protected override void updateState() {}

    private void applyKeyInput()
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
            StartCoroutine(inventory.GetSkill().UseSkill(inventory.GetWeapons()[WeaponManager.GetInstance().GetBasicWeaponCode()]));
        }

        if (Input.GetKeyDown(KeyCode.A) && supplies[0] != Supply.SupplyType.NONE) {
            tempSupply = supplies[0];
            useSupply(ref tempSupply);
            supplies[0] = tempSupply;
            UIManager.GetInstance().UpdateSupplyInfos();
        }

        if (Input.GetKeyDown(KeyCode.S) && supplies[1] != Supply.SupplyType.NONE) {
            tempSupply = supplies[1];
            useSupply(ref tempSupply);
            supplies[1] = tempSupply;
            UIManager.GetInstance().UpdateSupplyInfos();
        }
        if(direction == Vector3.zero) {rigidBody.velocity = new Vector3(0f, 0f, 0f);}
        direction = direction.normalized;
        setDirections(direction);
    }

    protected override void setDirections(Vector3 direction)
    {
        this.moveDirection = direction;
        if (this.moveDirection != Vector3.zero
        && !Input.GetKey(KeyCode.Z)) this.attackDirection = this.moveDirection;

    }
    
    public override void TakeDamage(int damage)
    {
        if (damage <= armor) return;
        
        this.hp -= damage - armor;
        audioSource.clip = damageClip;
        audioSource.Play();
        UIManager.GetInstance().UpdatePlayerCurrentStatus();
    }

    public void Heal(int value)
    {
        hp += value;
        if (hp > maxHp) hp = maxHp;
        UIManager.GetInstance().UpdatePlayerCurrentStatus();
    }

    public void ApplyEnhancedOptions(Dictionary<string, EnhanceInfo> enhanceInfos)
    {
        foreach (KeyValuePair<string, EnhanceInfo> data in enhanceInfos)
        {
            switch (data.Key)
            {
                case "HP":
                    this.maxHp += (int)(this.maxHp * data.Value.enhanceCount * data.Value.value / 100f);
                    this.hp = this.maxHp;
                    break;
                
                case"Damage":
                    this.damageMultiple += data.Value.enhanceCount * data.Value.value / 100f;
                    inventory.GetSkill().ApplyEnhanceOption("Damage", data.Value.enhanceCount * data.Value.value / 100f);
                    break;
                
                case "MoveSpeed":
                    this.moveSpeed += this.moveSpeed * data.Value.enhanceCount * data.Value.value / 100f;
                    break;

                case "Coin":
                    this.coinMultiple += data.Value.enhanceCount * data.Value.value / 100f;
                    break;
                
                case "Exp":
                    this.expMultiple += data.Value.enhanceCount * data.Value.value / 100f;
                    break;

                case "SkillDelay":
                    inventory.GetSkill().ApplyEnhanceOption("SkillDelay", data.Value.enhanceCount * data.Value.value / 100f);
                    break;
            }
        }

        inventory.ApplyEnhanceOptions(this.damageMultiple, this.coinMultiple);
        playerLevel.ApplyEnhanceOption(expMultiple);
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

    private void useSupply(ref Supply.SupplyType type) {
        switch(type) {
            case Supply.SupplyType.healKit:
                Heal(DEFAULT_HEAL_VALUE);
                heal.SetActive(true);
                StartCoroutine(inactivateGameObject(heal, DEFAULT_HEAL_DURATION));
                break;
            case Supply.SupplyType.magnet:
                ItemManager.GetInstance().Magnet();
                StartCoroutine(ItemManager.GetInstance().StopMagnetSound(DEFAULT_MAGNET_DURATION- 0.1f));
                magnet.SetActive(true);
                StartCoroutine(inactivateGameObject(magnet, DEFAULT_MAGNET_DURATION));
                break;
            case Supply.SupplyType.bomb:
                StartCoroutine(EnemyManager.GetInstance().Bomb());
                bomb.SetActive(true);
                StartCoroutine(inactivateGameObject(bomb, DEFAULT_BOMB_DURATION));
                break;
            default:
                break;
        }

        supplyCount--;
        type = Supply.SupplyType.NONE;
    }

    public List<Supply.SupplyType> GetSupplyInfos() { return supplies; }

    private IEnumerator inactivateGameObject(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);

        obj.SetActive(false);
    }

    public void OnTriggerEnter(Collider obj)
    {
        switch (obj.tag)
        {
            case "exp":
                playerLevel.GainExp(obj.GetComponent<Item>().GetValue());
                UIManager.GetInstance().UpdatePlayerCurrentStatus();
                obj.gameObject.SetActive(false);
                audioSource.clip = expClip;
                audioSource.Play();
                break;
            
            case "coin":
                inventory.GainCoins(obj.GetComponent<Item>().GetValue());
                obj.gameObject.SetActive(false);
                break;
            
            case "supply":
                if(supplyCount >= MAX_SUPPLY_COUNT) break;

                Supply.SupplyType type = obj.GetComponent<Supply>().GetSupplyType();
                if(supplies[0] == Supply.SupplyType.NONE) supplies[0] = type;
                else {
                    supplies[1] = type;
                }

                UIManager.GetInstance().UpdateSupplyInfos();

                supplyCount++;
                Destroy(obj.gameObject);
                break;
        }
    }
}
