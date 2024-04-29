using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public enum AttackType
    {
        ONE_OFF,
        CONTINUING
    }

    private const float DEFAULT_DAMAGE_DELAY = 0.25f;
    
    private int damage;
    private float speed;
    private Vector3 attackDirection;
    private AttackType attackType;

    private Player player;
    private float time;

    // Update is called once per frame
    void Update()
    {
        move();
        if (player != null
            && attackType == AttackType.CONTINUING)
            damagePlayer();
    }
    
    public void Init(int damage, float speed, Vector3 attackDirection, float duration, AttackType attackType)
    {
        player = null;
        time = 0f;
        this.damage = damage;
        this.speed = speed;
        this.attackDirection = attackDirection;
        this.attackType = attackType;
        Destroy(this.gameObject, duration);
    }
    
    private void move()
    {
        this.transform.position += attackDirection * (Time.deltaTime * speed);
    }
    
    public void OnTriggerEnter(Collider obj)
    {
        if (!obj.CompareTag("player")) return;

        player = obj.GetComponent<Player>();
        player.TakeDamage(damage);
    }

    public void OnTriggerExit(Collider obj)
    {
        if (!obj.CompareTag("player")) return;

        time = 0f;
        player = null;
    }

    private void damagePlayer()
    {
        time += Time.deltaTime;
        
        if (time >= DEFAULT_DAMAGE_DELAY)
        {
            time -= DEFAULT_DAMAGE_DELAY;
            player.TakeDamage(damage);
        }
    }
}
