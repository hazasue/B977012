using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemy : Enemy
{
    public enum BossEnemyState
    {
        MOVE,
        RUSH,
        ATTACK,
        USESKILL,
    }

    private const float DEFAULT_PROJECTILE_SPEED = 6f;
    private const float DEFAULT_PROJECTILE_DURATION = 7f;
    private static float DEFAULT_ATTACK_RANGE = 3f;
    private static float DEFAULT_ATTACK_DURATION = 0.8f;
    private const float DEFAULT_SKILL_DELAY = 5f;
    private const float DEFAULT_SKILL_DURATION = 1f;

    private BossEnemyState bossEnemyState;
    private bool isAttacking;
    private bool usingSkill;
    private bool canUseSkill;

    void Update()
    {
        switch (characterState)
        {
            case Character.CharacterState.ALIVE:
                updateBossState();
                switch (bossEnemyState)
                {
                    case BossEnemyState.MOVE:
                        setDirections(target.position - this.transform.position);
                        move();
                        break;

                    case BossEnemyState.RUSH:
                        break;

                    case BossEnemyState.ATTACK:
                        attack();
                        break;

                    case BossEnemyState.USESKILL:
                        skill();
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

        this.key = key;

        this.maxHp = enemyInfo.GetMaxHp();
        this.hp = this.maxHp;
        this.damage = enemyInfo.GetDamage();
        this.moveSpeed = enemyInfo.GetSpeed();
        this.armor = enemyInfo.GetArmor();
        this.tickTime = enemyInfo.GetTickTime();
        if (enemyInfo.GetExp() > 0) itemInfos.Add(new ItemInfo(DEFAULT_ITEM_TYPE_EXP, enemyInfo.GetExp()));
        currentDamage = 0;

        this.target = target;
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
    }

    public void InitBoss()
    {
        bossEnemyState = BossEnemyState.MOVE;
        isAttacking = false;
        usingSkill = false;
        canUseSkill = false;
        DEFAULT_ATTACK_RANGE = GetComponent<CapsuleCollider>().radius - 0.1f;
        StartCoroutine(skillDelay());
    }

    protected override void move()
    {
        this.transform.position += Time.deltaTime * moveSpeed * moveDirection;
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation,
            Quaternion.LookRotation(target.position - this.transform.position), DEFAULT_ROTATE_SPEED * Time.deltaTime);
    }

    protected override void attack() {}

    protected void skill()
    {
        if (!canUseSkill) return;
        canUseSkill = false;
        EnemyProjectile projectile = Instantiate(Resources.Load<EnemyProjectile>("prefabs/enemies/dragonProjectile"),
            this.transform.position, Quaternion.identity, EnemyManager.GetInstance().transform);
        projectile.Init(damage, DEFAULT_PROJECTILE_SPEED, attackDirection, DEFAULT_PROJECTILE_DURATION);
    }

    protected override void setDirections(Vector3 direction)
    {
        moveDirection = direction.normalized;
        attackDirection = moveDirection;
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

    private void updateBossState()
    {
        switch (bossEnemyState)
        {
            case BossEnemyState.MOVE:
                if (Vector3.Distance(this.transform.position, target.position) <= DEFAULT_ATTACK_RANGE)
                {
                    animator.SetBool("isAttack", true);
                    bossEnemyState = BossEnemyState.ATTACK;
                    isAttacking = true;
                    canAttack = true;
                    StartCoroutine(inactivateAttack());
                }
                else if (canUseSkill)
                {
                    animator.SetBool("isSkill", true);
                    bossEnemyState = BossEnemyState.USESKILL;
                    usingSkill = true;
                    StartCoroutine(stopUsingSkill());
                }

                break;

            case BossEnemyState.RUSH:
                break;

            case BossEnemyState.ATTACK:
                if (isAttacking) break;
                if (Vector3.Distance(this.transform.position, target.position) > DEFAULT_ATTACK_RANGE)
                {
                    animator.SetBool("isAttack", false);
                    bossEnemyState = BossEnemyState.MOVE;
                }

                break;

            case BossEnemyState.USESKILL:
                if (!usingSkill)
                {
                    animator.SetBool("isSkill", false);
                    bossEnemyState = BossEnemyState.MOVE;
                    StartCoroutine(skillDelay());
                }
                break;
        }
    }

    private IEnumerator inactivateAttack()
    {
        yield return new WaitForSeconds(DEFAULT_ATTACK_DURATION);
        if (Vector3.Distance(this.transform.position, target.position) > DEFAULT_ATTACK_RANGE)
            isAttacking = false;
        else
        {
            canAttack = true;
            StartCoroutine(inactivateAttack());
        }
    }
    
    private IEnumerator stopUsingSkill()
    {
        yield return new WaitForSeconds(DEFAULT_SKILL_DURATION);
        usingSkill = false;
    }

    private IEnumerator skillDelay()
    {
        yield return new WaitForSeconds(DEFAULT_SKILL_DELAY);

        canUseSkill = true;
    }

}
