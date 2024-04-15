using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupplyBox : MonoBehaviour
{
    private static int MAX_SUPPLY_TYPE_COUNT = 3;
	
    public Supply supplyObject;

    private Transform parentTransform;

    public void SetParentTransform(Transform parentTransform)
    {
        this.parentTransform = parentTransform;
    }

    private void DropItem()
    {
        Supply tempSupply = Instantiate(supplyObject, parentTransform, true);
        tempSupply.Init((Supply.SupplyType)Random.Range(0, MAX_SUPPLY_TYPE_COUNT));
        tempSupply.transform.position = this.transform.position + new Vector3(0f, -0.4f, 0f);
    }

    public void DestroySupplyBox()
    {
        DropItem();
        Destroy(this.gameObject);
    }
}
