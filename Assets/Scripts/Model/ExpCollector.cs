using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpCollector : MonoBehaviour
{
    private static float DEFAULT_SCALE_Y = 1f;
    private const float DEFAULT_RANGE = 2f;
    private const float DEFAULT_MOVESPEED = 7f;

    private List<Transform> expList;
    private float range;
    
    // Start is called before the first frame update
    void Start()
    {
        Init(DEFAULT_RANGE);
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = expList.Count - 1; i >= 0; i--)
        {
            if (!expList[i]) expList.Remove(expList[i]);
            else
            {
                expList[i].position += (this.transform.position - expList[i].position).normalized * DEFAULT_MOVESPEED *
                                      Time.deltaTime;
            }
        }
    }
    
    public void Init(float range)
    {
        expList = new List<Transform>();
        this.range = range;
        this.transform.localScale = new Vector3(range, DEFAULT_SCALE_Y, range);
    }
    
    public void OnTriggerEnter(Collider exp)
    {
        if (!exp.CompareTag("exp")) return;

        expList.Add(exp.GetComponent<Transform>());
    }
}
