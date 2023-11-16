using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponUpgradeInfo
{
    public string code;
    public string option1;
    public float value1;
    public string option2;
    public float value2;

    public WeaponUpgradeInfo(string code, string option1, float value1, string option2 = "", float value2 = 0f)
    {
        this.code = code;
        this.option1 = option1;
        this.value1 = value1;
        this.option2 = option2;
        this.value2 = value2;
    }
}
