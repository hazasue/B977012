using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupplyManager : MonoBehaviour
{
	private static float DEFAULT_SPAWN_PERIOD = 50f;
	private static float DEFAULT_SPAWN_RANGE = 15f;

	public SupplyBox supplyBox;

	private Transform supplyTransform;
	
    // Start is called before the first frame update
    void Start()
    {
	    init();
    }

    private void init()
    {
	    supplyTransform = GameObject.Find("Item Transform").transform;
	    StartCoroutine(spawnSupplyBox(DEFAULT_SPAWN_PERIOD));
    }

    private IEnumerator spawnSupplyBox(float delay)
    {
	    yield return new WaitForSeconds(delay);

	    SupplyBox tempSupplyBox = Instantiate(supplyBox, this.transform, true);
	    tempSupplyBox.SetParentTransform(supplyTransform);
	    tempSupplyBox.transform.position = GameManager.GetInstance().GetPlayer().transform.position +
	                                       new Vector3(Random.Range(-DEFAULT_SPAWN_RANGE, DEFAULT_SPAWN_RANGE), 0.5f,
		                                       Random.Range(-DEFAULT_SPAWN_RANGE, DEFAULT_SPAWN_RANGE));

	    StartCoroutine(spawnSupplyBox(delay));
    }
}
