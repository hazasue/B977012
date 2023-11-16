using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    public enum EquipmentType
    {
        ARMOR,
        WEAPON,
    }

    private EquipmentType type;
    private string code;

    public void Init(string code, EquipmentType type = EquipmentType.WEAPON)
    {
        this.code = code;
        this.type = type;
    }

    public string GetCode() { return code; }

    public EquipmentType GetType() { return type; }
}
