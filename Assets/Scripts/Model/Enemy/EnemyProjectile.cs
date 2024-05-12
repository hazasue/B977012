using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public enum AttackType
    {
        ONE_OFF,
        CONTINUING,
        EXPLOSION
    }

    private const float DEFAULT_DAMAGE_DELAY = 0.25f;
    
    private int damage;
    private float speed;
    private Vector3 attackDirection;
    private AttackType attackType;

    private Player player;
    private List<Enemy> enemies;
    private float time;

    // Update is called once per frame
    void Update()
    {
        move();
        if (attackType == AttackType.CONTINUING)
            damageCharacters();
    }
    
    public void Init(int damage, float speed, Vector3 attackDirection, float duration, AttackType attackType)
    {
        player = null;
        enemies = new List<Enemy>();
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
        switch (obj.tag)
        {
            case "player":
                player = obj.GetComponent<Player>();
                player.TakeDamage(damage);
                break;
            
            case "enemy":
                if (attackType == AttackType.ONE_OFF) return;
                Enemy enemy = obj.GetComponent<Enemy>();
                enemies.Add(enemy);
                enemy.TakeDamage(damage);
                break;
            
            default:
                break;
            
        }
    }

    public void OnTriggerExit(Collider obj)
    {
        switch (obj.tag)
        {
            case "player":
                time = 0f;
                player = null;
                break;
            
            case "enemy":
                if (attackType == AttackType.ONE_OFF) return;
                Enemy enemy = obj.GetComponent<Enemy>();
                enemies.Remove(enemy);
                break;
            
            default:
                break;
            
        }
    }

    private void damageCharacters()
    {
        time += Time.deltaTime;
        
        if (time >= DEFAULT_DAMAGE_DELAY)
        {
            time -= DEFAULT_DAMAGE_DELAY;
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                if (!enemies[i].gameObject.activeSelf) enemies.RemoveAt(i);
            }
            if (player != null) player.TakeDamage(damage);

            foreach (Enemy enemy in enemies)
            {
                enemy.TakeDamage(damage);
            }
            
        }
    }
}
