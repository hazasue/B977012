using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardEnemy : Enemy
{
    public enum GuardState
    {
        CHASE,
        RUSH,
    }

    private const float DEFAULT_RUSH_RANGE = 5f;
    private const float DEFAULT_RUSH_COOLDOWN = 5f;
    private const float DEFAULT_RUSH_DELAY = 1f;
    private const float DEFAULT_RUSH_DURATION = 0.5f;

    private GuardState guardState;
    private float time;
    private bool canRush;
    private Transform warningTileBG;
    private Transform warningTile;
    private bool isWarningTileActivate;
    
    // Update is called once per frame
    void Update()
    {
        switch (characterState)
        {
            case Character.CharacterState.ALIVE:
                updateGuardState();
                switch (guardState) {
                    case GuardState.CHASE:
                        setDirections(target.position - this.transform.position);
                        if (!isGrabbed) move();
                        break;
                    
                    case GuardState.RUSH:
                        rush();
                        if(isWarningTileActivate) updateWarningTile();
                        break;
                    
                    default:
                        break;
                }
                break;
            
            case Character.CharacterState.DEAD:
                knockBack();
                invisible();
                break;
            
            default:
                break;
        }
    }
    
    public override void Init(int maxHp, int damage, float speed, int armor) {}
    
    public override void Init(EnemyInfo enemyInfo, Transform target, int key, Vector3? moveDirection = null)
    {
        itemInfos = new List<ItemInfo>();
        
        characterState = Character.CharacterState.ALIVE;
        animator = this.GetComponent<Animator>();

        this.guardState = GuardState.CHASE;

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
        isGrabbed = false;
        time = 0f;
        canRush = true;
        warningTileBG = this.transform.GetChild(2).transform;
        warningTile = this.transform.GetChild(3).transform;
        isWarningTileActivate = false;
        
        audioSource = this.GetComponent<AudioSource>();
        SoundManager.GetInstance().AddToSfxList(audioSource);
        audioSource.volume = SoundManager.GetInstance().audioSourceSfx.volume;
        audioSource.clip = Resources.Load<AudioClip>($"Sfxs/enemies/hit_sound");
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
        this.transform.position += Time.deltaTime * moveSpeed * moveDirection;
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.LookRotation(target.position - this.transform.position), DEFAULT_ROTATE_SPEED * Time.deltaTime);
    }
    
    protected override void attack() {}

    private void rush() {
        time += Time.deltaTime;
        if (time <= DEFAULT_RUSH_DELAY) {

        }
        else if (time <= DEFAULT_RUSH_DELAY + DEFAULT_RUSH_DURATION) {
            animator.SetBool("wait", false);
            this.transform.position += Time.deltaTime * moveSpeed * moveDirection * 6f;
        }
    }

    private void updateWarningTile() {
        if (time <= DEFAULT_RUSH_DELAY) {
            warningTile.localScale = new Vector3(time, 2f * moveSpeed, 1f);
        }
        else {
            isWarningTileActivate = false;
            warningTile.gameObject.SetActive(false);
            warningTileBG.gameObject.SetActive(false);
        }
    }
    
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
        if (currentDamage > 0)
        {
            updateState();
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

    private void updateGuardState() {
        switch(guardState) {
            case GuardState.CHASE:
                if (Vector3.Distance(target.position, this.transform.position) <= DEFAULT_RUSH_RANGE
                    && canRush) {
                    guardState = GuardState.RUSH;
                    time = 0f;
                    canRush = false;
                    StartCoroutine(ableToRush(DEFAULT_RUSH_COOLDOWN));
                    activateWarningTile();
                    animator.SetBool("wait", true);
                }
                break;

            case GuardState.RUSH:
                if (time > DEFAULT_RUSH_DELAY + DEFAULT_RUSH_DURATION) {
                    guardState = GuardState.CHASE;
                }
                break;
            
            default:
                break;
        }
    }

    private void activateWarningTile() {
        warningTile.gameObject.SetActive(true);
        warningTileBG.gameObject.SetActive(true);
        warningTileBG.localPosition = new Vector3(0f, 0.1f, moveSpeed);
        warningTile.localPosition = new Vector3(0f, 0.11f, moveSpeed);
        warningTileBG.localScale = new Vector3(1f, 2f * moveSpeed, 1f); 
        warningTile.localScale = new Vector3(0f, 2f * moveSpeed, 1f); 
        isWarningTileActivate = true;
    }

    private IEnumerator ableToRush(float delay) {
        yield return new WaitForSeconds(delay);

        canRush = true;
    }
}
