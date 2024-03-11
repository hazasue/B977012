using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnhanceInfo
{
    public string stat;
    public int enhanceCount;
    public float value;
    public int price;
    public string description;

    public EnhanceInfo(string stat, int enhanceCount, float value, int price, string description)
    {
        this.stat = stat;
        this.enhanceCount = enhanceCount;
        this.value = value;
        this.price = price;
        this.description = description;
    }
}
