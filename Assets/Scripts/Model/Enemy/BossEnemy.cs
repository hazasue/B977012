using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemy : Enemy
{
    public enum BossEnemyState
    {
        SPAWN,
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
    private static float DEFAULT_RANGED_ATTACK_DURATION = 1.0f;
    private static float DEFAULT_RANGED_ATTACK_DELAY = 0.4f;
    private const float DEFAULT_SKILL_DELAY = 15f;
    private const float DEFAULT_SKILL_DURATION = 7.0f;
    private const float BOSS_ROTATE_SPEED = 5f;

    private BossEnemyState bossEnemyState;
    private bool isAttacking;
    private bool usingSkill;
    private bool skillUsable;
    private bool isRanged;
    private float spawnDelay;

    private AudioClip meleeAttackClip;
    private AudioClip rangedAttackClip;
    private AudioClip skillClip;

    public Transform skillTransform;

    void Update()
    {
        switch (characterState)
        {
            case Character.CharacterState.ALIVE:
                updateBossState();
                setDirections(target.position - this.transform.position);
                switch (bossEnemyState)
                {
                    case BossEnemyState.SPAWN:
                        move();
                        break;
                    
                    case BossEnemyState.MOVE:
                        if (!isGrabbed) move();
                        break;

                    case BossEnemyState.RUSH:
                        break;

                    case BossEnemyState.ATTACK:
                        break;

                    case BossEnemyState.USESKILL:
                        skill();
                        break;
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

        this.key = key;

        this.maxHp = enemyInfo.GetMaxHp();
        this.hp = this.maxHp;
        this.damage = enemyInfo.GetDamage();
        this.moveSpeed = enemyInfo.GetSpeed();
        this.armor = enemyInfo.GetArmor();
        this.tickTime = enemyInfo.GetTickTime();
        this.canRangeAttack = enemyInfo.canRangeAttack;
        this.canUseSkill = enemyInfo.canUseSkill;
        this.spawnDelay = enemyInfo.spawnDelay;
        if (enemyInfo.GetExp() > 0) itemInfos.Add(new ItemInfo(DEFAULT_ITEM_TYPE_EXP, enemyInfo.GetExp()));
        currentDamage = 0;
        isGrabbed = false;

        audioSource = this.GetComponent<AudioSource>();
        SoundManager.GetInstance().AddToSfxList(audioSource);
        audioSource.volume = SoundManager.GetInstance().audioSourceSfx.volume;
        meleeAttackClip = Resources.Load<AudioClip>($"Sfxs/enemies/{enemyInfo.GetCode()}_melee_attack_sound");
        rangedAttackClip = Resources.Load<AudioClip>($"Sfxs/enemies/{enemyInfo.GetCode()}_ranged_attack_sound");;
        skillClip = Resources.Load<AudioClip>($"Sfxs/enemies/{enemyInfo.GetCode()}_skill_sound");;

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
        bossEnemyState = BossEnemyState.SPAWN;
        isAttacking = false;
        usingSkill = false;
        skillUsable = false;
        isRanged = false;
        DEFAULT_MELEE_ATTACK_RANGE = GetComponent<CapsuleCollider>().radius - 0.1f;
        StartCoroutine(skillDelay());
        StartCoroutine(giveSpawnDelay(spawnDelay));
    }

    protected override void move()
    {
        this.transform.position += Time.deltaTime * moveSpeed * moveDirection;
    }

    protected override void attack() {
        if (isRanged) {
            StartCoroutine(rangedAttack(DEFAULT_RANGED_ATTACK_DELAY));
            audioSource.clip = rangedAttackClip;
            audioSource.Play();
        }
        else
        {
            audioSource.clip = meleeAttackClip;
            audioSource.Play();
        }
    }

    protected void skill()
    {
        if (!skillUsable) return;
        skillUsable = false;
        EnemyProjectile projectile = Instantiate(Resources.Load<EnemyProjectile>("prefabs/enemies/enemyBreath"),
            this.transform.position, Quaternion.identity, skillTransform);//EnemyManager.GetInstance().transform);
        projectile.transform.localPosition = new Vector3(0f, 0f, 0f);
        projectile.transform.localRotation = Quaternion.identity;
        projectile.transform.localScale = projectile.transform.localScale * 100f;
        projectile.Init(damage, 0f, attackDirection, DEFAULT_PROJECTILE_DURATION, EnemyProjectile.AttackType.CONTINUING);
        audioSource.clip = skillClip;
        audioSource.Play();
    }

    protected override void setDirections(Vector3 direction)
    {
        moveDirection = direction.normalized;
        attackDirection = moveDirection;
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation,
            Quaternion.LookRotation(target.position - this.transform.position), BOSS_ROTATE_SPEED * Time.deltaTime);
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
                    attack();
                    if (isRanged) StartCoroutine(inactivateAttack(DEFAULT_RANGED_ATTACK_DURATION));
                    else
                    {
                        StartCoroutine(inactivateAttack(DEFAULT_MELEE_ATTACK_DURATION));
                    }
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

    private IEnumerator inactivateAttack(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (Vector3.Distance(this.transform.position, target.position) <= DEFAULT_MELEE_ATTACK_RANGE
            || (Vector3.Distance(this.transform.position, target.position) >= MIN_RANGED_ATTACK_RANGE
                && Vector3.Distance(this.transform.position, target.position) <= MAX_RANGED_ATTACK_RANGE
                && canRangeAttack))
        {
            canAttack = true;
            changeAttackType();
            attack();
            if (isRanged) StartCoroutine(inactivateAttack(DEFAULT_RANGED_ATTACK_DURATION));
			else
			{
                StartCoroutine(inactivateAttack(DEFAULT_MELEE_ATTACK_DURATION));
            }
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

    private IEnumerator rangedAttack(float delay)
    {
        yield return new WaitForSeconds(delay);
        EnemyProjectile projectile = Instantiate(Resources.Load<EnemyProjectile>("prefabs/enemies/dragonProjectile"),
            this.transform.position, Quaternion.identity, EnemyManager.GetInstance().transform);
        projectile.transform.localRotation = Quaternion.identity;
        projectile.Init(damage, DEFAULT_PROJECTILE_SPEED, attackDirection, DEFAULT_PROJECTILE_DURATION, EnemyProjectile.AttackType.ONE_OFF);
    }

    private IEnumerator giveSpawnDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        bossEnemyState = BossEnemyState.MOVE;
    }
}
