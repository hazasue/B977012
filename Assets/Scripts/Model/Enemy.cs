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

    // attributes
    private EnemyType enemyType;
    private EnemyGrade enemyGrade;

    private float tickTime;
    private bool canAttack;
    private int exp;

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
        this.exp = enemyInfo.GetExp();
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
    
    private void DropItems() {}

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
