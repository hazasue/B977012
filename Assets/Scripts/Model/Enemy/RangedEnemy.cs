using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : Enemy
{
    public enum RangedEnemyState
    {
        CHASE,
        ATTACK
    }

    private const float DEFAULT_PROJECTILE_SPEED = 6f;
    private const float DEFAULT_PROJECTILE_DURATION = 7f;
    private const float DEFAULT_ATTACK_RANGE = 10f;

    private RangedEnemyState enemyState;
    
    // Update is called once per frame
    void Update()
    {
        switch (characterState)
        {
            case Character.CharacterState.ALIVE:
                setDirections(target.position - this.transform.position);
                updateEnemyState();
                switch (enemyState)
                {
                    case RangedEnemyState.CHASE:
                        move();
                        break;
                    
                    case RangedEnemyState.ATTACK:
                        if (canAttack)
                        {
                            canAttack = false;
                            attack();
                            StartCoroutine(timeToAttack());
                        }
                        break;
                }
                break;
            
            case Character.CharacterState.DEAD:
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
        currentDamage = 0;
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

        enemyState = RangedEnemyState.CHASE;

        canAttack = true;

        this.target = target;

        this.key = key;
    }
    
    protected override void move()
    {
        this.transform.position += Time.deltaTime * moveSpeed * moveDirection;
    }

    protected override void attack()
    {
        EnemyProjectile projectile = Instantiate(Resources.Load<EnemyProjectile>("prefabs/enemies/enemyProjectile"),
            this.transform.position, Quaternion.identity, EnemyManager.GetInstance().transform);
        projectile.Init(damage, DEFAULT_PROJECTILE_SPEED, attackDirection, DEFAULT_PROJECTILE_DURATION, EnemyProjectile.AttackType.ONE_OFF);
    }
    
    protected override void setDirections(Vector3 direction)
    {
        moveDirection = direction.normalized;
        attackDirection = moveDirection;
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.LookRotation(direction), DEFAULT_ROTATE_SPEED * Time.deltaTime);
    }
    
    public override void TakeDamage(int damage)
    {
        if (damage <= armor) return;
        
        this.hp -= damage - armor;
        currentDamage = damage - armor;
        updateState();
    }
    
    protected override void updateState()
    {
        switch (characterState)
        {
            case Character.CharacterState.ALIVE:
                if (hp <= 0)
                {
                    die();
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

    private void updateEnemyState()
    {
        switch (enemyState)
        {
            case RangedEnemyState.CHASE:
                if (Vector3.Distance(target.position, this.transform.position) <= DEFAULT_ATTACK_RANGE) 
                    enemyState = RangedEnemyState.ATTACK;
                    break;
            
            case RangedEnemyState.ATTACK:
                if (Vector3.Distance(target.position, this.transform.position) >= 1.5f * DEFAULT_ATTACK_RANGE) 
                    enemyState = RangedEnemyState.CHASE;
                break;
            
            default:
                break;
        }
    }
}
