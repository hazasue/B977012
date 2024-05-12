using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveEnemy : Enemy
{
    private const float DEFAULT_WARNING_DURATION = 2f;
    private const float DEFAULT_EXPLOSION_DURATION = 1.5f;
    private const int DEFAULT_BOOM_ATTACK_DAMAGE_MAGNIFICATION = 5;
    
    private bool playerTriggerd;
    private float time;

    [SerializeField]
    private Transform warningTile;
    [SerializeField]
    private Transform warningTileBG;

    [SerializeField] 
    private EnemyProjectile boomEffect;


    [SerializeField] 
    private EnemyRangeCollider rangeCollider;

    private AudioClip hitClip;
    private AudioClip explosionClip;
    
    // Update is called once per frame
    void Update()
    {
        switch (characterState)
        {
            case Character.CharacterState.ALIVE:
                setDirections(target.position - this.transform.position);
                if (!isGrabbed) move();
                if (playerTriggerd)
                {
                    updateWarningRange();
                    boom();
                }
                break;
            
            case Character.CharacterState.DEAD:
                knockBack();
                invisible();
                break;
        }
    }
    
    public override void Init(int maxHp, int damage, float speed, int armor) {}
    
    public override void Init(EnemyInfo enemyInfo, Transform target, int key, Vector3? moveDirection = null)
    {
        itemInfos = new List<ItemInfo>();
        
        characterState = Character.CharacterState.ALIVE;
        animator = this.GetComponent<Animator>();

        this.maxHp = enemyInfo.GetMaxHp();
        this.hp = this.maxHp;
        this.damage = enemyInfo.GetDamage();
        this.moveSpeed = enemyInfo.GetSpeed();
        this.armor = enemyInfo.GetArmor();
        this.tickTime = enemyInfo.GetTickTime();
        this.canRangeAttack = enemyInfo.canRangeAttack;
        this.canUseSkill = enemyInfo.canUseSkill;
        if (enemyInfo.GetExp() > 0) itemInfos.Add(new ItemInfo(DEFAULT_ITEM_TYPE_EXP, enemyInfo.GetExp()));
        warningTile.localScale = new Vector3(1f, 0f, 1f);
        warningTileBG.localScale = new Vector3(this.moveSpeed * 2f, this.moveSpeed * 2f ,1f);
        rangeCollider.Init(this.moveSpeed * 2f, this);
        
        hitClip = Resources.Load<AudioClip>($"Sfxs/enemies/hit_sound");
        explosionClip = Resources.Load<AudioClip>($"Sfxs/enemies/enemy_explosion_sound");
        audioSource = this.GetComponent<AudioSource>();
        SoundManager.GetInstance().AddToSfxList(audioSource);
        audioSource.volume = SoundManager.GetInstance().audioSourceSfx.volume;
        currentDamage = 0;
        isGrabbed = false;
        playerTriggerd = false;
        // if (enemyInfo.GetCoin() > 0) itemInfos.Add(new ItemInfo(DEFAULT_ITEM_TYPE_COIN, enemyInfo.GetCoin()));

        switch (enemyInfo.GetType())
        {
            case "MELEE":
                this.enemyType = Enemy.EnemyType.MELEE;
                break;
            
            case "RANGED":
                this.enemyType = Enemy.EnemyType.RANGED;
                break;
            
            case "EXPLOSIVE":
                this.enemyType = Enemy.EnemyType.EXPLOSIVE;
                break;
            
            default:
                Debug.Log("Invalid enemy type: " + enemyType);
                break;
        }

        switch (enemyInfo.GetGrade())
        {
            case "NORMAL":
                this.enemyGrade = Enemy.EnemyGrade.NORMAL;
                break;
            
            case "GROUP":
                this.enemyGrade = Enemy.EnemyGrade.GROUP;
                break;
            
            case "GUARD":
                this.enemyGrade = Enemy.EnemyGrade.GUARD;
                break;
            
            case "ELITE":
                this.enemyGrade = Enemy.EnemyGrade.ELITE;
                break;
            
            case "BOSS":
                this.enemyGrade = Enemy.EnemyGrade.BOSS;
                break;
            
            default:
                Debug.Log("Invalid enemy grade: " + enemyInfo.GetGrade());
                break;
        }

        canAttack = true;

        this.target = target;

        this.key = key;
    }
    
    protected override void move()
    {
        moveDirection = (target.position - this.transform.position).normalized;
        this.transform.position += Time.deltaTime * moveSpeed * moveDirection;
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.LookRotation(target.position - this.transform.position), DEFAULT_ROTATE_SPEED * Time.deltaTime);
    }
    
    protected override void attack() {}
    
    protected override void setDirections(Vector3 direction)
    {
        moveDirection = direction.normalized;
        attackDirection = moveDirection;
    }

    public override void TakeDamage(int damage)
    {
        if (damage <= armor) return;
        if (this.hp <= 0f) return;

        this.hp -= damage - armor;
        currentDamage = damage - armor;
        if (currentDamage > 0)
        {
            updateState();
            audioSource.clip = hitClip;
            audioSource.Play();
        }
        if(this.hp > 0f) {
            renderer.material = hitMaterial;
            StartCoroutine(changeMaterialBack(DEFAULT_HIT_DURATION));
        }
    }
    
    protected override void updateState()
    {
        switch (characterState)
        {
            case Character.CharacterState.ALIVE:
                if (hp <= 0)
                {
                    die();
                    warningTileBG.gameObject.SetActive(false);
                    warningTile.gameObject.SetActive(false);
                }
                EnemyManager.GetInstance().UpdateEnemyStatus(enemyGrade, key);
                break;
            
            case Character.CharacterState.DEAD:
                break;
            
            default:
                Debug.Log("Invalid character state: " + characterState);
                break;
        }
    }

    public void TriggerPlayer()
    {
        if (playerTriggerd) return;
        playerTriggerd = true;
        time = 0f;
        warningTileBG.gameObject.SetActive(true);
        warningTile.gameObject.SetActive(true);
    }

    private void updateWarningRange()
    {
        time += Time.deltaTime;
        float warningRange = time * moveSpeed;
        warningTile.localScale = new Vector3(warningRange, warningRange, 1f);
    }

    private void boom()
    {
        if (time < DEFAULT_WARNING_DURATION) return;
        EnemyProjectile effect = Instantiate(boomEffect, this.transform.position, Quaternion.identity, this.transform.parent);
        effect.transform.localScale = new Vector3(moveSpeed, 1f, moveSpeed);
        effect.Init(damage * DEFAULT_BOOM_ATTACK_DAMAGE_MAGNIFICATION, 0f, Vector3.zero, DEFAULT_EXPLOSION_DURATION, EnemyProjectile.AttackType.EXPLOSION);
        audioSource.clip = explosionClip;
        audioSource.Play();
        die();
        EnemyManager.GetInstance().UpdateEnemyStatus(enemyGrade, key);

        StartCoroutine(inactivateEnemy(DEFAULT_EXPLOSION_DURATION));
    }
}
