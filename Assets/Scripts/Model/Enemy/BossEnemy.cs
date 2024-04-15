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

    private const float DEFAULT_PROJECTILE_SPEED = 8f;
    private const float DEFAULT_PROJECTILE_DURATION = 7f;
    private static float DEFAULT_MELEE_ATTACK_RANGE = 3f;
    private static float DEFAULT_MELEE_ATTACK_DURATION = 0.8f;
    private static float MIN_RANGED_ATTACK_RANGE = 8f;
    private static float MAX_RANGED_ATTACK_RANGE = 12f;
    private static float DEFAULT_RANGED_ATTACK_DURATION = 0.8f;
    private const float DEFAULT_SKILL_DELAY = 15f;
    private const float DEFAULT_SKILL_DURATION = 6.9f;

    private BossEnemyState bossEnemyState;
    private bool isAttacking;
    private bool usingSkill;
    private bool skillUsable;
    private bool isRanged;

    void Update()
    {
        switch (characterState)
        {
            case Character.CharacterState.ALIVE:
                updateBossState();
                setDirections(target.position - this.transform.position);
                switch (bossEnemyState)
                {
                    case BossEnemyState.MOVE:
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
        this.canRangeAttack = enemyInfo.canRangeAttack;
        this.canUseSkill = enemyInfo.canUseSkill;
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
        skillUsable = false;
        isRanged = false;
        DEFAULT_MELEE_ATTACK_RANGE = GetComponent<CapsuleCollider>().radius - 0.1f;
        StartCoroutine(skillDelay());
    }

    protected override void move()
    {
        this.transform.position += Time.deltaTime * moveSpeed * moveDirection;
    }

    protected override void attack() {}

    protected void skill()
    {
        if (!skillUsable) return;
        skillUsable = false;
        EnemyProjectile projectile = Instantiate(Resources.Load<EnemyProjectile>("prefabs/enemies/enemyBreath"),
            this.transform.position, Quaternion.identity, this.transform);//EnemyManager.GetInstance().transform);
        projectile.transform.localPosition += new Vector3(0f, 0f, 2f);
        projectile.transform.localRotation = Quaternion.identity;
        projectile.Init(damage, 0f, attackDirection, DEFAULT_PROJECTILE_DURATION, EnemyProjectile.AttackType.CONTINUING);
    }

    protected override void setDirections(Vector3 direction)
    {
        moveDirection = direction.normalized;
        attackDirection = moveDirection;
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation,
            Quaternion.LookRotation(target.position - this.transform.position), DEFAULT_ROTATE_SPEED * Time.deltaTime);
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
                if (Vector3.Distance(this.transform.position, target.position) <= DEFAULT_MELEE_ATTACK_RANGE
                    || (Vector3.Distance(this.transform.position, target.position) >= MIN_RANGED_ATTACK_RANGE
                        && Vector3.Distance(this.transform.position, target.position) <= MAX_RANGED_ATTACK_RANGE
                        && canRangeAttack))
                {
                    animator.SetBool("isAttack", true);
                    bossEnemyState = BossEnemyState.ATTACK;
                    isAttacking = true;
                    canAttack = true;
                    changeAttackType();
                    instantiateAttackProjectile();
                    StartCoroutine(inactivateAttack());
                }
                else if (skillUsable && canUseSkill)
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
                if (Vector3.Distance(this.transform.position, target.position) > DEFAULT_MELEE_ATTACK_RANGE
                    && (Vector3.Distance(this.transform.position, target.position) < MIN_RANGED_ATTACK_RANGE
                        || Vector3.Distance(this.transform.position, target.position) > MAX_RANGED_ATTACK_RANGE
                        || !canRangeAttack))
                {
                    animator.SetBool("isAttack", false);
                    animator.SetBool("isRanged", false);
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
        yield return new WaitForSeconds(DEFAULT_MELEE_ATTACK_DURATION);
        if (Vector3.Distance(this.transform.position, target.position) <= DEFAULT_MELEE_ATTACK_RANGE
            || (Vector3.Distance(this.transform.position, target.position) >= MIN_RANGED_ATTACK_RANGE
                && Vector3.Distance(this.transform.position, target.position) <= MAX_RANGED_ATTACK_RANGE
                && canRangeAttack))
        {
            canAttack = true;
            changeAttackType();
            instantiateAttackProjectile();
            StartCoroutine(inactivateAttack());
        }
        else
        {
            animator.SetBool("isRanged", false);
            isAttacking = false;
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

        skillUsable = true;
    }

    private void changeAttackType()
    {
        if (Vector3.Distance(this.transform.position, target.position) <= DEFAULT_MELEE_ATTACK_RANGE) 
            isRanged = false;
        else if(Vector3.Distance(this.transform.position, target.position) >= MIN_RANGED_ATTACK_RANGE
                && Vector3.Distance(this.transform.position, target.position) <= MAX_RANGED_ATTACK_RANGE)
        {
            if (canRangeAttack) isRanged = true;
            else
            {
                isRanged = false;
            }
        }

        animator.SetBool("isRanged", isRanged);
    }

    private void instantiateAttackProjectile()
    {
        if (isRanged) rangedAttack();
        else
        {

        }
    }

    private void rangedAttack()
    {
        EnemyProjectile projectile = Instantiate(Resources.Load<EnemyProjectile>("prefabs/enemies/dragonProjectile"),
            this.transform.position, Quaternion.identity, EnemyManager.GetInstance().transform);
        projectile.transform.localRotation = Quaternion.identity;
        projectile.Init(damage, DEFAULT_PROJECTILE_SPEED, attackDirection, DEFAULT_PROJECTILE_DURATION, EnemyProjectile.AttackType.ONE_OFF);
    }
}
