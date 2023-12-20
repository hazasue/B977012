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

    public EnhanceInfo(string stat, int enhanceCount, float value, int price)
    {
        this.stat = stat;
        this.enhanceCount = enhanceCount;
        this.value = value;
        this.price = price;
    }
}
