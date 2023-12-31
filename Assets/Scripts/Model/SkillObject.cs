using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillObject : MonoBehaviour
{
    private int damage;
    private float speed;
    private Vector3 attackDirection;

    // Update is called once per frame
    void Update()
    {
        move();
    }
    
    public void Init(int damage, float speed, Vector3 attackDirection)
    {
        this.damage = damage;
        this.speed = speed;
        this.attackDirection = attackDirection;
    }

    private void move()
    {
        this.transform.position += attackDirection * (Time.deltaTime * speed);
    }
    
    public void OnTriggerEnter(Collider obj)
    {
        if (!obj.CompareTag("enemy")) return;

        Enemy enemy = obj.gameObject.GetComponent<Enemy>();
        enemy.TakeDamage(damage);

    }
}
