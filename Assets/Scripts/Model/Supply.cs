using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Supply : MonoBehaviour
{
    private static int DEFAULT_HEAL_VALUE = 50;

    public enum SupplyType
    {
        healKit, 
        magnet,
        bomb,
        NONE
    };

    private SupplyType SUPPLY_TYPE;

    public void Init(SupplyType supplyType)
    {
        SUPPLY_TYPE = supplyType;

        switch (SUPPLY_TYPE)
        {
            case SupplyType.healKit:
                GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Supplies/HealKit");
                break;
            case SupplyType.magnet:
                GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Supplies/Magnet");
                break;
            case SupplyType.bomb:
                GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Supplies/Bomb");
                break;
            default:
                return;
        }
    }

    public SupplyType GetSupplyType() { return SUPPLY_TYPE; }
}
