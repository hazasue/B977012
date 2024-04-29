using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    private const float DEFAULT_SPEED = 50f;
    
    public enum ItemType
    {
        EXP,
        COIN,
        ITEMBOX,
    }
    
    // attributes
    private ItemType itemType;
    private int value;
    private bool isTracking;
    private Transform player;

    void Update()
    {
        if (isTracking) trackPlayer();
    }
    
    // methods
    public void Init(ItemType itemType, int value)
    {
        this.itemType = itemType;
        this.value = value;
        isTracking = false;
    }

    public ItemType GetItemType() { return itemType; }
    public int GetValue() { return value; }

    public void UseMagnet(Transform player)
    {
        isTracking = true;
        this.player = player;
    }

    private void trackPlayer()
    {
        this.transform.position +=
            (player.position - this.transform.position).normalized * DEFAULT_SPEED * Time.deltaTime;
    }
}
