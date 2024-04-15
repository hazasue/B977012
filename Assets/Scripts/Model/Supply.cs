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

    public void OnTriggerEnter(Collider player)
    {
        if (!player.CompareTag("player")) return;

        switch (SUPPLY_TYPE)
        {
            case SupplyType.healKit:
                player.GetComponent<Player>().Heal(DEFAULT_HEAL_VALUE);
                break;
            case SupplyType.magnet:
                ItemManager.GetInstance().Magnet();
                break;
            case SupplyType.bomb:
                EnemyManager.GetInstance().Bomb();
                break;
            default:
                break;
        }

        Destroy(this.gameObject);
    }
}
