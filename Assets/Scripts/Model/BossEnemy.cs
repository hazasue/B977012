using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemy : Enemy
{
    public enum BossEnemyState
    {
        MOVE,
        RUSH,
        ATTACK,
        USESKILL,
    }

    private static float DEFAULT_ATTACK_RANGE = 3f;
    private static float DEFAULT_ATTACK_DURATION = 0.8f;

    private BossEnemyState bossEnemyState;
    private bool isAttacking;
    
    void Update()
    {
        switch (characterState)
        {
            case Character.CharacterState.ALIVE:
                updateBossState();
                switch (bossEnemyState)
                {
                    case BossEnemyState.MOVE:
                        move();
                        break;

                    case BossEnemyState.RUSH:
                        break;
                    
                    case BossEnemyState.ATTACK:
                        attack();
                        break;

                    case BossEnemyState.USESKILL:
                        break;
                }
                break;
            case Character.CharacterState.DEAD:
                break;
        }
    }

    public void InitBoss()
    {
        bossEnemyState = BossEnemyState.MOVE;
        isAttacking = false;
    }

    private void attack()
    {
        
    }

    private void updateBossState()
    {
        switch (bossEnemyState)
        {
            case BossEnemyState.MOVE:
                if (Vector3.Distance(this.transform.position, target.position) <= DEFAULT_ATTACK_RANGE)
                {
                    animator.SetBool("isAttack", true);
                    bossEnemyState = BossEnemyState.ATTACK;
                    isAttacking = true;
                    StartCoroutine(inactivateAttack());
                }
                break;
            
            case BossEnemyState.RUSH:
                break;
            
            case BossEnemyState.ATTACK:
                if (isAttacking) break;
                if (Vector3.Distance(this.transform.position, target.position) > DEFAULT_ATTACK_RANGE)
                {
                    animator.SetBool("isAttack", false);
                    bossEnemyState = BossEnemyState.MOVE;
                }
                break;
            
            case BossEnemyState.USESKILL:
                break;
        }
    }

    private IEnumerator inactivateAttack()
    {
        yield return new WaitForSeconds(DEFAULT_ATTACK_DURATION);
        if (Vector3.Distance(this.transform.position, target.position) <= DEFAULT_ATTACK_RANGE)
            StartCoroutine(inactivateAttack());
        else
        {
            isAttacking = false;
        }
    }

}
