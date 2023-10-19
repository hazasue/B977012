using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    private enum EnemyType
    {
        basic,
        elite,
        boss,
    }
    
    // attributes
    private EnemyType enemyType;
    
    // associations
    private List<Item> items;

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public override void Init(int maxHp, int damage, float speed, int armor) {}
    
    protected override void Move() {}
    
    protected override void Attack() {}
    
    protected override void SetDirections(Vector3 direction) {}
    
    protected override void UpdateStatus() {}
    
    public override void TakeDamage(int damage) {}
    
    private void DropItems() {}
}
