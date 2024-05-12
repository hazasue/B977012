using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRangeCollider : MonoBehaviour
{
    private static float DEFAULT_SCALE_Y = 1f;

    private List<Enemy> enemyList;
    private Player targetPlayer;
    private ExplosiveEnemy enemy;

    public void Init(float range, ExplosiveEnemy enemy = null)
    {
        this.transform.localScale = new Vector3(range, DEFAULT_SCALE_Y, range);
        targetPlayer = null;
        this.enemy = enemy;
    }

    public void OnTriggerEnter(Collider player)
    {
        if (!player.CompareTag("player")) return;

        targetPlayer = player.GetComponent<Player>();
        if (enemy != null) enemy.TriggerPlayer();

    }

    public Player GetPlayer() { return targetPlayer; }

    public void OnTriggerExit(Collider player)
    {
        if (!player.CompareTag("player")) return;

        targetPlayer = null;
    }
}
