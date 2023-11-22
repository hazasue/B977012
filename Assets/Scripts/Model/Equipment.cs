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
    private string purpose;
    private string buyStatus;
    private int price;

    // Buy
    public void Init(string code, string purpose, string buyStatus, int price, EquipmentType type = EquipmentType.WEAPON)
    {
        this.code = code;
        this.purpose = purpose;
        this.buyStatus = buyStatus;
        this.price = price;
        this.type = type;
    }
    
    // Equip
    public void Init(string code, string purpose, EquipmentType type = EquipmentType.WEAPON)
    {
        this.code = code;
        this.purpose = purpose;
        this.type = type;
    }

    public string GetCode() { return code; }

    public string GetPurpose() { return purpose; }

    public string GetBuyStatus() { return buyStatus; }

    public int GetPrice() { return price; }

    public EquipmentType GetType() { return type; }
}
