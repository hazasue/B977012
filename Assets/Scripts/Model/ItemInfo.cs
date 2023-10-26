using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInfo
{
    private Item.ItemType itemType;
    private int value;

    public ItemInfo(string itemType, int value)
    {
        this.value = value;

        switch (itemType)
        {
            case "EXP":
                this.itemType = Item.ItemType.EXP;
                break;
            
            case "COIN":
                this.itemType = Item.ItemType.COIN;
                break;
            
            case "ITEMBOX":
                this.itemType = Item.ItemType.ITEMBOX;
                break;
            
            default:
                Debug.Log("Invalid item type: " + itemType);
                break;
        }
    }

    public Item.ItemType GetItemType() { return itemType; }

    public int GetValue() { return value; }
}
