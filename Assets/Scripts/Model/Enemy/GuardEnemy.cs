using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardEnemy : Enemy
{
    public enum GuardType
    {
        STAY,
        CHASE_PLAYER,
        CHASE_LEFT,
        CHASE_RIGHT,
    }
    
    // Update is called once per frame
    void Update()
    {
        switch (characterState)
        {
            case Character.CharacterState.ALIVE:
                move();
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
        
        this.moveDirection = (Vector3)moveDirection;
        
        canAttack = true;

        this.target = target;

        this.key = key;
    }
    
    protected override void move()
    {
        this.transform.position += Time.deltaTime * moveSpeed * moveDirection;
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.LookRotation(target.position - this.transform.position), DEFAULT_ROTATE_SPEED * Time.deltaTime);
    }
    
    protected override void attack() {}
    
    protected override void setDirections(Vector3 direction)
    {
        moveDirection = direction.normalized;
    }

    public override void TakeDamage(int damage)
    {
        if (damage <= armor) return;
        if (this.hp <= 0f) return;

        this.hp -= damage - armor;
        currentDamage = damage - armor;
        if (currentDamage > 0) updateState();
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
}
