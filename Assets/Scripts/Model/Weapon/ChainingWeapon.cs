using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainingWeapon : Weapon
{
    private static float DEFAULT_CHAINING_RANGE = 5f;
    private const float DEFAULT_EFFECT_WIDTH = 1.0f;
    private static float DEFAULT_TRANFER_DELAY = 0.05f;
    private static int DEFAULT_TARGET_ENEMY_COUNT = 0;
    private static int FIRST_ENEMY_POSITION_INDEX = 0;
    private static Vector3 DEFAULT_OBJECT_POS_Y = new Vector3(0f, 0.5f, 0f);
    
    private LineRenderer lineRenderer;
    private List<Vector3> enemyPositions;
    private List<Transform> enemyTransforms;
    
    public override void Init(WeaponInfo weaponInfo, RangeCollider rangeCollider, bool mainWeapon = false)
    {
        code = weaponInfo.GetCode();
        name = weaponInfo.GetName();
        damage = weaponInfo.GetDamage();
        duration = weaponInfo.GetDuration();
        delay = weaponInfo.GetDelay();
        projectile = weaponInfo.GetProjectile();
        range = weaponInfo.GetRange();
        speed = weaponInfo.GetSpeed();
        weaponType = Weapon.WeaponType.CHAINING;
        upgradeCount = 0;

        enableToAttack = false;

        this.rangeCollider = rangeCollider;
        this.rangeCollider.Init(range);

        lineRenderer = this.GetComponent<LineRenderer>();
        enemyPositions = new List<Vector3>();
        enemyTransforms = new List<Transform>();
        
        if (mainWeapon) StartCoroutine(EnableToAttack());
        else
        {
            StartCoroutine(ActivateWeaponObjectAuto());
        }
    }
    
    public override void UpgradeWeapon(WeaponUpgradeInfo upgradeInfo) {
        switch (upgradeInfo.option1)
        {
            case NONE_OPTION_STRING:
                break;
            
            case "damage":
                this.damage += (int)upgradeInfo.value1;
                break;
            
            case "duration":
                this.duration += upgradeInfo.value1;
                break;
            
            case "delay":
                this.delay -= upgradeInfo.value1;
                break;
            
            case "projectile":
                this.projectile += (int)upgradeInfo.value1;
                break;
            
            case "speed":
                this.speed += upgradeInfo.value1;
                break;
            
            default:
                Debug.Log("Unmatched upgrade option: " + this.code + " " + upgradeInfo.option1);
                break;
        }

        switch (upgradeInfo.option2)
        {
            case NONE_OPTION_STRING:
                break;
            
            case "damage":
                this.damage += (int)upgradeInfo.value2;
                break;
            
            case "duration":
                this.duration += upgradeInfo.value2;
                break;
            
            case "delay":
                this.delay -= upgradeInfo.value2;
                break;
            
            case "projectile":
                this.projectile += (int)upgradeInfo.value2;
                break;
            
            case "speed":
                this.speed += upgradeInfo.value2;
                break;
            
            default:
                Debug.Log("Unmatched upgrade option: " + this.code + " " + upgradeInfo.option2);
                break;
        }

        upgradeCount++;
        
    }

    protected override void InstantiateWeaponObjects() {}

    public override void ActivateWeaponObject(Vector3 attackDirection)
    {
        if (!enableToAttack) return;

        enableToAttack = false;
        StartCoroutine(EnableToAttack());
        
        //
        enemyTransforms.Clear();
        enemyPositions.Clear();
        lineRenderer.positionCount = 0;
        int count = 0;
        enemyTransforms.Add(this.transform);
        enemyPositions.Add(this.transform.position);
        lineRenderer.positionCount++;

        List<Enemy> enemyList = new List<Enemy>();
        Enemy enemy = rangeCollider.GetClosestEnemy(this.transform.position, enemyList, DEFAULT_CHAINING_RANGE);
        if (enemy == null) return;

        Vector3 centralPosition = Vector3.zero;
        float distance = Vector3.Distance(this.transform.position, enemy.transform.position);
        StartCoroutine(giveDelayToSkill(projectile, count, distance, centralPosition, enemyList, enemy));
        StartCoroutine(updateLine(Time.deltaTime));
    }
    
    public override IEnumerator ActivateWeaponObjectAuto()
    {
        yield return new WaitForSeconds(delay);
        
        StartCoroutine(ActivateWeaponObjectAuto());

        enemyTransforms.Clear();
        enemyPositions.Clear();
        lineRenderer.positionCount = 0;
        int count = 0;
        enemyTransforms.Add(this.transform);
        enemyPositions.Add(this.transform.position);
        lineRenderer.positionCount++;

        List<Enemy> enemyList = new List<Enemy>();
        Enemy enemy = rangeCollider.GetClosestEnemy(this.transform.position, enemyList, DEFAULT_CHAINING_RANGE);
        if (enemy == null) yield break;

        Vector3 centralPosition = Vector3.zero;
        float distance = Vector3.Distance(this.transform.position, enemy.transform.position);
        StartCoroutine(giveDelayToSkill(projectile, count, distance, centralPosition, enemyList, enemy));
        StartCoroutine(updateLine(Time.deltaTime));
    }
    
    private IEnumerator giveDelayToSkill(int leftProjectile, int count, float distance, Vector3 centralPosition, List<Enemy> enemyList, Enemy enemy)
    {
        if (leftProjectile <= 0 || distance > DEFAULT_CHAINING_RANGE)
        {
            StartCoroutine(removeLine(duration));
            yield break;
        }
        count++;
        enemyList.Add(enemy);
        if (enemy.gameObject.activeSelf)
        {
            enemy.TakeDamage(damage);
        }

        centralPosition = enemy.transform.position;
        enemyTransforms.Add(enemy.transform);
        enemyPositions.Clear();
        for (int i = 0; i < enemyTransforms.Count; i++)
        {
            if(enemyTransforms[i].position.y != DEFAULT_OBJECT_POS_Y.y)
                enemyPositions.Add(enemyTransforms[i].position + DEFAULT_OBJECT_POS_Y);
            else
            {
                enemyPositions.Add(enemyTransforms[i].position);
            }
        }
        lineRenderer.positionCount++;
        lineRenderer.SetPositions(enemyPositions.ToArray());
        StartCoroutine(removeLine(duration));
        Enemy nextEnemy = rangeCollider.GetClosestEnemy(enemy.transform.position, enemyList, DEFAULT_CHAINING_RANGE);
        if (nextEnemy == null)
        {
            StartCoroutine(removeLine(duration));
            yield break;
        }
        distance = Vector3.Distance(centralPosition, nextEnemy.transform.position);
        
        yield return new WaitForSeconds(DEFAULT_TRANFER_DELAY);
        StartCoroutine(giveDelayToSkill(leftProjectile - 1, count, distance, centralPosition, enemyList, nextEnemy));
    }

    private IEnumerator removeLine(float time)
    {
        yield return new WaitForSeconds(time);

        if (enemyTransforms.Count <= 0)
        {
            yield break;
        }
        enemyTransforms.RemoveAt(FIRST_ENEMY_POSITION_INDEX);
        enemyPositions.RemoveAt(FIRST_ENEMY_POSITION_INDEX);
        lineRenderer.positionCount--;
        lineRenderer.SetPositions(enemyPositions.ToArray());
    }

    private IEnumerator updateLine(float time)
    {
        enemyPositions.Clear();

        for (int i = enemyTransforms.Count - 1; i >= 0; i--)
        {
            if (!enemyTransforms[i]) enemyTransforms.RemoveAt(FIRST_ENEMY_POSITION_INDEX);
        }
        
        for (int i = 0; i < enemyTransforms.Count; i++)
        {
            if(enemyTransforms[i].position.y != DEFAULT_OBJECT_POS_Y.y)
                enemyPositions.Add(enemyTransforms[i].position + DEFAULT_OBJECT_POS_Y);
            else
            {
                enemyPositions.Add(enemyTransforms[i].position);
            }
        }
        lineRenderer.SetPositions(enemyPositions.ToArray());
        
        yield return new WaitForSeconds(time);
        if (enemyTransforms.Count <= DEFAULT_TARGET_ENEMY_COUNT) yield break;
        StartCoroutine(updateLine(Time.deltaTime));

    }
}
