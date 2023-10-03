using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    // attributes
    private int killedEnemiesCount;
    
    // associations
    private List<Enemy> enemies;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void Init() {}
    
    private void SpawnEnemy() {}
    
    private void CheckEnemyStatus() {}

    public int GetKilledEnemiesCount()
    {
        return killedEnemiesCount;
    }
}
