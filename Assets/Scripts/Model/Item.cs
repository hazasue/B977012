using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum ItemType
    {
        exp,
        coin,
        itemBox,
    }
    
    // attributes
    private ItemType itemType;
    private int value;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    // methods
    public void Init() {}

    public Dictionary<ItemType, int> GetItemInfo()
    {
        return new Dictionary<ItemType, int>();
    }
    
    public void DropItem() {}
}
