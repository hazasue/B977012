using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum ItemType
    {
        EXP,
        COIN,
        ITEMBOX,
    }
    
    // attributes
    private ItemType itemType;
    private int value;

    // methods
    public void Init(ItemType itemType, int value)
    {
        this.itemType = itemType;
        this.value = value;
    }

    public ItemType GetItemType() { return itemType; }
    public int GetValue() { return value; }
}
