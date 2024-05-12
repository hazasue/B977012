using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StageInfo
{
    public string stageCode;
    public int normalEnemyCount;
    public int rangedEnemyCount;
    public int groupEnemyCount;
    public int guardEnemyCount;
    public int explosiveEnemyCount;
    public int eliteEnemyCount;
    public int bossEnemyCount;
    public List<string> normalEnemies;
    public List<string> rangedEnemies;
    public List<string> groupEnemies;
    public List<string> guardEnemies;
    public List<string> explosiveEnemies;
    public List<string> eliteEnemies;
    public List<string> bossEnemies;
    public int basicReward;
    public string texture;

    public StageInfo(string stageCode, int normalEnemyCount, int rangedEnemyCount, int groupEnemyCount,
        int guardEnemyCount, int explosiveEnemyCount, int eliteEnemyCount,
        int bossEnemyCount, List<string> normalEnemies, List<string> rangedEnemies, List<string> groupEnemies,
        List<string> guardEnemies, List<string> explosiveEnemies, List<string> eliteEnemies,
        List<string> bossEnemies, int basicReward, string texture)
    {
        this.stageCode = stageCode;
        this.normalEnemyCount = normalEnemyCount;
        this.rangedEnemyCount = rangedEnemyCount;
        this.groupEnemyCount = groupEnemyCount;
        this.guardEnemyCount = guardEnemyCount;
        this.explosiveEnemyCount = explosiveEnemyCount;
        this.eliteEnemyCount = eliteEnemyCount;
        this.bossEnemyCount = bossEnemyCount;
        this.normalEnemies = normalEnemies;
        this.rangedEnemies = rangedEnemies;
        this.groupEnemies = groupEnemies;
        this.guardEnemies = guardEnemies;
        this.explosiveEnemies = explosiveEnemies;
        this.eliteEnemies = eliteEnemies;
        this.bossEnemies = bossEnemies;
        this.basicReward = basicReward;
        this.texture = texture;
    }
}
