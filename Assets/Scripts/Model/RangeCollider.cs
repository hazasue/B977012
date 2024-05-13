using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeCollider : MonoBehaviour
{
    private static float DEFAULT_SCALE_Y = 1f;

    private List<Enemy> enemyList;
    private float range;
    private Enemy targetEnemy;
    private float distance;
    private float tempDistance;
    private bool isGrabWeapon;
    
    public void Init(float range, bool isGrabWeapon = false)
    {
        enemyList = new List<Enemy>();
        this.range = range;
        this.transform.localScale = new Vector3(range, DEFAULT_SCALE_Y, range);
        targetEnemy = null;
        distance = range;
        this.isGrabWeapon = isGrabWeapon;
    }
    
    public Enemy GetClosestEnemy()
    {
        if (enemyList.Count <= 0) return null;
        
        targetEnemy = null;
        distance = range;
        tempDistance = distance;
        
        for (int i = enemyList.Count - 1; i >= 0; i--)
        {
            if (enemyList[i] == null
                || !enemyList[i].gameObject.activeSelf) {
                if (isGrabWeapon) enemyList[i].SetGrabState(false);
                enemyList.RemoveAt(i);
            }
        }

        for (int i = 0; i < enemyList.Count; i++)
        {
            tempDistance = Vector3.Distance(this.transform.position, enemyList[i].transform.position);
            if (tempDistance >= distance) continue;
            targetEnemy = enemyList[i];
            distance = tempDistance;
        }

        return targetEnemy;
    }

    public List<Enemy> GetEnemies()
    {
        for (int i = enemyList.Count - 1; i >= 0; i--)
        {
            if (enemyList[i] == null) enemyList.RemoveAt(i);
        }
        return enemyList;
    }

    public Enemy GetClosestEnemy(Vector3 centralPosition, List<Enemy> enemies, float range)
    {
        if (enemyList.Count <= 0) return null;

        targetEnemy = null;
        distance = range;
        tempDistance = distance;
        
        for (int i = enemyList.Count - 1; i >= 0; i--)
        {
            if (enemyList[i] == null
                || !enemyList[i].gameObject.activeSelf) {
                if (isGrabWeapon) enemyList[i].SetGrabState(false);
                enemyList.RemoveAt(i);
            }
        }
        
        for (int i = 0; i < enemyList.Count; i++)
        {
            tempDistance = Vector3.Distance(this.transform.position, enemyList[i].transform.position);
            if (tempDistance >= distance
                || enemies.Contains(enemyList[i])) continue;
            targetEnemy = enemyList[i];
            distance = tempDistance;
        }

        if(targetEnemy != null) enemies.Add(targetEnemy);
        
        return targetEnemy;
    }

    public void OnTriggerEnter(Collider enemy)
    {
        if (!enemy.CompareTag("enemy")) return;

        Enemy tempEnemy = enemy.GetComponent<Enemy>();
        enemyList.Add(tempEnemy);
        if (isGrabWeapon) tempEnemy.SetGrabState(true);
    }

    public void OnTriggerExit(Collider enemy)
    {
        if (!enemy.CompareTag("enemy")) return;

        Enemy tempEnemy = enemy.GetComponent<Enemy>();
        enemyList.Remove(tempEnemy);
        if (isGrabWeapon) tempEnemy.SetGrabState(false);
    }
}
