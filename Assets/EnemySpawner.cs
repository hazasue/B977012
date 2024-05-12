using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public EnemyRangeCollider rangeCollider;
    public Transform warningTile;
    public Transform warningTileBG;

    private float time;
    private float spawnDelay;
    private float range;
    private Transform parent;
    private Transform player;
    private Enemy enemy;
    private EnemyInfo enemyInfo;
    private int key;

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        updateWarningTile();
        if (time >= spawnDelay) spawnEnemy();
    }

    public void Init(float spawnDelay, float range, Transform parent, Transform target, Enemy enemy, EnemyInfo enemyInfo, int key)
    {
        time = 0f;
        this.range = range;
        this.spawnDelay = spawnDelay;
        this.parent = parent;
        this.player = target;
        this.enemy = enemy;
        this.enemyInfo = enemyInfo;
        this.key = key;

        this.transform.localScale = new Vector3(range, 1f, range);
        warningTile.localScale = new Vector3(0f, 1f, 0f);
        rangeCollider.Init(1f);
    }

    private void updateWarningTile()
    {
        warningTile.localScale = new Vector3(time / spawnDelay, time / spawnDelay, 1f);
    }

    private void spawnEnemy()
    {
        Enemy tempEnemy = Instantiate(enemy, this.transform.position, Quaternion.identity, parent);
        tempEnemy.Init(enemyInfo, player, key);
        EnemyManager.GetInstance().AddToActiveEnemies(key, tempEnemy);
        if (rangeCollider.GetPlayer() != null) rangeCollider.GetPlayer().TakeDamage(enemyInfo.GetDamage() * 2);

        Destroy(this.gameObject);
    }
}
