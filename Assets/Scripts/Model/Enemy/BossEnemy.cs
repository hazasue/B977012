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

    private static float DEFAULT_ATTACK_RANGE = 3f;
    private static float DEFAULT_ATTACK_DURATION = 0.8f;

    private BossEnemyState bossEnemyState;
    private bool isAttacking;

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
    }

    public void InitBoss()
    {
        bossEnemyState = BossEnemyState.MOVE;
        isAttacking = false;
    }

    protected override void move()
    {
        this.transform.position += Time.deltaTime * moveSpeed * moveDirection;
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation,
            Quaternion.LookRotation(target.position - this.transform.position), DEFAULT_ROTATE_SPEED * Time.deltaTime);
    }

    protected override void attack() {}

    protected override void setDirections(Vector3 direction)
    {
        moveDirection = direction.normalized;
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
                    StartCoroutine(inactivateAttack());
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
                break;
        }
    }

    private IEnumerator inactivateAttack()
    {
        yield return new WaitForSeconds(DEFAULT_ATTACK_DURATION);
        if (Vector3.Distance(this.transform.position, target.position) <= DEFAULT_ATTACK_RANGE)
            StartCoroutine(inactivateAttack());
        else
        {
            isAttacking = false;
        }
    }

}
