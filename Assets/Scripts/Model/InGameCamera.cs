using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameCamera : MonoBehaviour
{
    private Transform player;

    void Start()
    {
        player = GameManager.GetInstance().GetPlayer().transform;
    }

    // Update is called once per frame
    void Update()
    {
        MoveCamera();
    }

    private void MoveCamera()
    {
        this.transform.position = player.position + new Vector3(0f, 20f, 0f);
    }
}
