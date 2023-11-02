using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    private static ItemManager instance;
    private static int DEFAULT_ITEM_COUNT = 800;

    private Queue<Item> exps;
    private Queue<Item> coins;
    private Queue<Item> itemBoxs;
    
    public Transform itemTransform;

    private void Start()
    {
        init();
    }

    private void init()
    {
        exps = new Queue<Item>();
        coins = new Queue<Item>();
        itemBoxs = new Queue<Item>();

        Item tempItem;
        
        for (int i = 0; i < DEFAULT_ITEM_COUNT; i++)
        {
            tempItem = Instantiate(Resources.Load<Item>("Prefabs/items/exp"), itemTransform, true);
            tempItem.gameObject.SetActive(false);
            exps.Enqueue(tempItem);
            tempItem = Instantiate(Resources.Load<Item>("Prefabs/items/coin"), itemTransform, true);
            tempItem.gameObject.SetActive(false);
            coins.Enqueue(tempItem);
            //itemBoxs.Enqueue(Instantiate(Resources.Load<Item>("Prefabs/items/itemBox"), itemTransform, true));
        }
    }

    public static ItemManager GetInstance()
    {
        if (instance != null) return instance;
        instance = FindObjectOfType<ItemManager>();
        if (instance == null) Debug.Log("There's no active ItemManager object");
        return instance;
    }

    public Item GetNewItem(Item.ItemType itemType)
    {
        Item tempItem;
        switch (itemType)
        {
            case Item.ItemType.EXP:
                tempItem = exps.Dequeue();
                tempItem.gameObject.SetActive(true);
                exps.Enqueue(tempItem);
                return tempItem;

            case Item.ItemType.COIN:
                tempItem = coins.Dequeue();
                tempItem.gameObject.SetActive(true);
                coins.Enqueue(tempItem);
                return tempItem;
                
            case Item.ItemType.ITEMBOX:
                tempItem = itemBoxs.Dequeue();
                tempItem.gameObject.SetActive(true);
                itemBoxs.Enqueue(tempItem);
                return tempItem;
                
            default:
                Debug.Log("Invalid item type: " + itemType);
                return new Item();
        }
    }
}
