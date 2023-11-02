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

    private static string DEFAULT_ITEM_TYPE_EXP = "EXP";
    private static string DEFAULT_ITEM_TYPE_COIN = "COIN";
    private static Vector3 DEFAULT_ITEM_POS_Y = new Vector3(0f, 0.5f, 0f); 
    
    // attributes
    private EnemyType enemyType;
    private EnemyGrade enemyGrade;

    private float tickTime;
    private bool canAttack;

    private List<ItemInfo> itemInfos;

    private int key;

    // associations
    private List<Item> items;
    private Transform target;

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
        
        this.enemyType = enemyInfo.GetType();
        this.enemyGrade = enemyInfo.GetGrade();
        this.maxHp = enemyInfo.GetMaxHp();
        this.hp = this.maxHp;
        this.damage = enemyInfo.GetDamage();
        this.moveSpeed = enemyInfo.GetSpeed();
        this.armor = enemyInfo.GetArmor();
        this.tickTime = enemyInfo.GetTickTime();
        if (enemyInfo.GetExp() > 0) itemInfos.Add(new ItemInfo(DEFAULT_ITEM_TYPE_EXP, enemyInfo.GetExp()));
        // if (enemyInfo.GetCoin() > 0) itemInfos.Add(new ItemInfo(DEFAULT_ITEM_TYPE_COIN, enemyInfo.GetCoin()));
        
        canAttack = true;

        this.target = target;

        this.key = key;
    }

    protected override void Move()
    {
        moveDirection = (target.position - this.transform.position).normalized;
        this.transform.position += Time.deltaTime * moveSpeed * moveDirection;
    }
    
    protected override void Attack() {}
    
    protected override void SetDirections(Vector3 direction) {}

    public override void TakeDamage(int damage)
    {
        if (damage <= armor) return;
        
        this.hp -= damage - armor;
        UpdateState();
    }

    private void DropItems()
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

    private void die()
    {
        characterState = Character.CharacterState.DEAD;
        animator.SetBool("dead", true);
        DropItems();
        EnemyManager.GetInstance().UpdateEnemyStatus(key);
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

    private IEnumerator timeToAttack()
    {
        yield return new WaitForSeconds(tickTime);
        canAttack = true;
    }
}
