using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    public enum EnemyType
    {
        MELEE,
        RANGED,
        EXPLOSIVE,
    }
    
    public enum EnemyGrade
    {
        NORMAL,
        ELITE,
        BOSS,
    }

    protected static string DEFAULT_ITEM_TYPE_EXP = "EXP";
    protected static string DEFAULT_ITEM_TYPE_COIN = "COIN";
    protected static Vector3 DEFAULT_ITEM_POS_Y = new Vector3(0f, 0.5f, 0f);

    // attributes
    protected EnemyType enemyType;
    protected EnemyGrade enemyGrade;

    protected float tickTime;
    protected bool canAttack;

    protected List<ItemInfo> itemInfos;

    protected int key;

    // associations
    protected List<Item> items;
    protected Transform target;

    // Update is called once per frame
    void Update()
    {
        switch (characterState)
        {
            case Character.CharacterState.ALIVE:
                Move();
                break;
            
            case Character.CharacterState.DEAD:
                break;
        }
    }

    public override void Init(int maxHp, int damage, float speed, int armor) { }

    public void Init(EnemyInfo enemyInfo, Transform target, int key)
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
            
            case "ELITE":
                this.enemyGrade = Enemy.EnemyGrade.ELITE;
                break;
            
            case "BOSS":
                this.enemyGrade = Enemy.EnemyGrade.BOSS;
                break;
            
            default:
                Debug.Log("Invalid enemy grade: " + enemyGrade);
                break;
        }
        
        canAttack = true;

        this.target = target;

        this.key = key;
    }

    protected override void Move()
    {
        moveDirection = (target.position - this.transform.position).normalized;
        this.transform.position += Time.deltaTime * moveSpeed * moveDirection;
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.LookRotation(target.position - this.transform.position), DEFAULT_ROTATE_SPEED * Time.deltaTime);
    }
    
    protected override void Attack() {}
    
    protected override void SetDirections(Vector3 direction) {}

    public override void TakeDamage(int damage)
    {
        if (damage <= armor) return;
        
        this.hp -= damage - armor;
        UpdateState();
    }

    protected void DropItems()
    {
        Item tempItem;
        foreach (ItemInfo itemInfo in itemInfos)
        {
            tempItem = ItemManager.GetInstance().GetNewItem(itemInfo.GetItemType());
            tempItem.Init(itemInfo.GetItemType(), itemInfo.GetValue());
            tempItem.transform.position = this.transform.position + DEFAULT_ITEM_POS_Y;
        }
    }

    protected override void UpdateState()
    {
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
                Debug.Log("Invalid character state: " + characterState);
                break;
        }
    }

    protected void die()
    {
        characterState = Character.CharacterState.DEAD;
        animator.SetBool("dead", true);
        DropItems();
        EnemyManager.GetInstance().UpdateEnemyStatus(enemyGrade, key);
        this.gameObject.SetActive(false);
    }

    public void OnTriggerEnter(Collider obj)
    {
        if (!canAttack) return;
        if (!obj.CompareTag("player")) return;

        canAttack = false;

        Player player = obj.GetComponent<Player>();
        player.TakeDamage(damage);

        StartCoroutine(timeToAttack());
    }

    protected IEnumerator timeToAttack()
    {
        yield return new WaitForSeconds(tickTime);
        canAttack = true;
    }
}
