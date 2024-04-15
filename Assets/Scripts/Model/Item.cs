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

    // methods
    public void Init(ItemType itemType, int value)
    {
        this.itemType = itemType;
        this.value = value;
    }

    public ItemType GetItemType() { return itemType; }
    public int GetValue() { return value; }

    public void UseMagnet(Transform player)
    {
        StartCoroutine(trackPlayer(player));
    }

    private IEnumerator trackPlayer(Transform player)
    {
        while (this.gameObject.activeSelf == true)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            this.transform.position += (player.position - this.transform.position).normalized * DEFAULT_SPEED * Time.deltaTime;
        }

    }
}
