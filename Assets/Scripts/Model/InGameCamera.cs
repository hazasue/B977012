using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameCamera : MonoBehaviour
{
    private static Vector3 DEFAULT_PLAYER_CAMERA_POSITION = new Vector3(0f, 20f, 0f);
    private static Vector3 DEFAULT_PLAYER_CAMERA_ROTATION = new Vector3(90f, 0f, 0f);
    private static Vector3 DEFAULT_BOSS_CAMERA_POSITION = new Vector3(0f, 15f, -15f);
    private static Vector3 DEFAULT_BOSS_CAMERA_ROTATION = new Vector3(45f, 0f, 0f);
    
    private Transform player;
    private Transform target;

    private Vector3 cameraPosition;

    void Start()
    {
        player = GameManager.GetInstance().GetPlayer().transform;
        target = player;
        cameraPosition = DEFAULT_PLAYER_CAMERA_POSITION;
        this.transform.rotation = Quaternion.Euler(DEFAULT_PLAYER_CAMERA_ROTATION);
    }

    // Update is called once per frame
    void Update()
    {
        MoveCamera();
    }

    private void MoveCamera()
    {
        this.transform.position = target.position + cameraPosition;
    }

    public IEnumerator ChaseBossEnemy(Transform bossEnemy, float duration)
    {
        if (duration <= 0f) yield break;
        target = bossEnemy;
        cameraPosition = DEFAULT_BOSS_CAMERA_POSITION;
        this.transform.rotation = Quaternion.Euler(DEFAULT_BOSS_CAMERA_ROTATION);

        yield return new WaitForSeconds(duration);

        target = player;
        cameraPosition = DEFAULT_PLAYER_CAMERA_POSITION;
        this.transform.rotation = Quaternion.Euler(DEFAULT_PLAYER_CAMERA_ROTATION);

    }
}
