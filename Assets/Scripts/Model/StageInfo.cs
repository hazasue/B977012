using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StageInfo
{
    public string stageCode;
    public int normalEnemyCount;
    public int specialEnemyCount;
    public int eliteEnemyCount;
    public int bossEnemyCount;
    public int guardPatternCount;
    public List<string> normalEnemies;
    public List<string> specialEnemies;
    public List<string> eliteEnemies;
    public List<string> bossEnemies;
    public List<string> guardPatterns;
    public int basicReward;
    public string texture;

    public StageInfo(string stageCode, int normalEnemyCount, int specialEnemyCount, int eliteEnemyCount,
        int bossEnemyCount, int guardPatternCount, List<string> normalEnemies, List<string> specialEnemies, List<string> eliteEnemies,
        List<string> bossEnemies, List<string> guardPatterns, int basicReward, string texture)
    {
        this.stageCode = stageCode;
        this.normalEnemyCount = normalEnemyCount;
        this.specialEnemyCount = specialEnemyCount;
        this.eliteEnemyCount = eliteEnemyCount;
        this.bossEnemyCount = bossEnemyCount;
        this.guardPatternCount = guardPatternCount;
        this.normalEnemies = normalEnemies;
        this.specialEnemies = specialEnemies;
        this.eliteEnemies = eliteEnemies;
        this.bossEnemies = bossEnemies;
        this.guardPatterns = guardPatterns;
        this.basicReward = basicReward;
        this.texture = texture;
    }
}
