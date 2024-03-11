using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enhancement : MonoBehaviour
{
    private string stat;
    private int enhanceCount;
    private float value;
    private int price;
    private string description;

    public void Init(string stat, int enhanceCount, float value, int price, string description)
    {
        this.stat = stat;
        this.enhanceCount = enhanceCount;
        this.value = value;
        this.price = price;
        this.description = description;
    }

    public string GetStat() { return stat; }
    
    public int GetEnhanceCount() { return enhanceCount; }
    
    public float GetValue() { return value; }

    public int GetPrice() { return price; }

    public string GetDescription() { return description; }
}
