using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StageInfo
{
    public string stageCode;
    public int normalEnemyCount;
    public int bossEnemyCount;
    public List<string> normalEnemies;
    public List<string> bossEnemies;

    public StageInfo(string stageCode, int normalEnemyCount, int bossEnemyCount, List<string> normalEnemies,
        List<string> bossEnemies)
    {
        this.stageCode = stageCode;
        this.normalEnemyCount = normalEnemyCount;
        this.bossEnemyCount = bossEnemyCount;
        this.normalEnemies = normalEnemies;
        this.bossEnemies = bossEnemies;
    }
}