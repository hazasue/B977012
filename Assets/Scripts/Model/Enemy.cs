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
    
    public override void Init() {}
    
    protected override void Attack() {}
    
    private void DropItems() {}
}
