using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    // attributes
    private float pickupRange;
    private int critical;

    // associations
    private Inventory inventory;
    private PlayerLevel playerLevel;
    private Skill skill;
    
    // Start is called before the first frame update
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    // methods
    public override void Init() {}
    
    protected override void Attack() {}
    
    private void PickUpItems() {}

    public void GainExp(int value) {}

    public void GainCoins(int value) {}

    public Inventory GetInventory() { return inventory; }

    public PlayerLevel GetLevelInfo() { return playerLevel; }
}
