using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enhancement : MonoBehaviour
{
    private string stat;
    private int enhanceCount;
    private float value;
    private int price;

    public void Init(string stat, int enhanceCount, float value, int price)
    {
        this.stat = stat;
        this.enhanceCount = enhanceCount;
        this.value = value;
        this.price = price;
    }

    public string GetStat() { return stat; }
    
    public int GetEnhanceCount() { return enhanceCount; }
    
    public float GetValue() { return value; }

    public int GetPrice() { return price; }
}
