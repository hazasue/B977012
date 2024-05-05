using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : Character
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
        GROUP,
        GUARD,
        ELITE,
        BOSS,
    }

    protected static string DEFAULT_ITEM_TYPE_EXP = "EXP";
    protected static string DEFAULT_ITEM_TYPE_COIN = "COIN";
    protected static Vector3 DEFAULT_ITEM_POS_Y = new Vector3(0f, 0.5f, 0f);
    protected const float DEFAULT_HIT_DURATION = 0.1f;
    protected const float DEFAULT_KNOCK_BACK_DURATION = 0.3f;

    // attributes
    protected EnemyType enemyType;
    protected EnemyGrade enemyGrade;

    protected float tickTime;
    protected bool canAttack;
    protected int currentDamage;
    protected bool canRangeAttack;
    protected bool canUseSkill;

    protected List<ItemInfo> itemInfos;

    protected int key;

    protected AudioSource audioSource;
    public Renderer renderer;
    public Material basicMaterial;
    public Material hitMaterial;

    // associations
    protected List<Item> items;
    protected Transform target;

    public abstract void Init(EnemyInfo enemyInfo, Transform target, int key, Vector3? moveDirection = null);

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

    protected void die()
    {
        characterState = Character.CharacterState.DEAD;
        animator.SetBool("dead", true);
        DropItems();
        StartCoroutine(inactivateEnemy(DEFAULT_KNOCK_BACK_DURATION));
        
    }

    public int GetCurrentDamage() { return currentDamage; }

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

    protected IEnumerator changeMaterialBack(float delay) {
        yield return new WaitForSeconds(delay);

        renderer.material = basicMaterial;
    }

    protected void knockBack() {
        this.transform.position += -attackDirection * moveSpeed * 2f * Time.deltaTime;
    }

    protected void invisible() {
        Color materialColor = renderer.material.color;
        materialColor.a -= 1f / DEFAULT_KNOCK_BACK_DURATION * Time.deltaTime;
        renderer.material.color = materialColor;
    }

    protected IEnumerator inactivateEnemy(float delay) {
        yield return new WaitForSeconds(delay);

        this.gameObject.SetActive(false);
    }

    public int GetKey() { return key; }
}
