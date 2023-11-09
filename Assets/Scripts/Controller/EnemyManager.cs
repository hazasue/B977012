using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private static EnemyManager instance;

    private static string CSV_FILENAME_STAGE = "DataTable_Stage";
    private static string DEFAULT_STAGE_CODE = "Stage1";
    private static int MAX_ENEMY_COUNT = 1200;
    private static int DEFAULT_NORMAL_ENEMY_INDEX = 0;
    private static int DEFAULT_ENEMY_SPAWN_POS_COUNT = 4;
    private static Vector3 SPAWN_ENEMY_POSITION = new Vector3(30.0f, 0.0f, 0.0f);
    private static Vector3 SPAWN_BOSS_POSITION = new Vector3(0.0f, 0.0f, 10.0f);
    private const float DEFAULT_SPAWN_DELAY = 1f;
    private const int DEFAULT_SPAWN_COUNT = 2;
    private static float RIGHT_ANGLE = 90f;
    private static float DEFAULT_BASIC_ENEMY_SPAWN_ANGLE = 30f;
    private static float DEFAULT_ENEMY_SPAWN_RANGE = 30f;
    
    // attributes
    private int killedEnemiesCount;
    private Transform player;
    
    // associations
    private Dictionary<string, EnemyInfo> enemyInfos;
    
    private Dictionary<int, Enemy> activeEnemies;
    private Dictionary<int, Enemy> bossEnemy;
    private Queue<Enemy> inactiveEnemies;
    private int key;

    private int bossPhase;
    private bool isBossActive;

    private Dictionary<string, Enemy> enemyObjects;

    // Stage enemy info
    private int normalEnemyCount;
    private int bossEnemyCount;
    
    private List<string> normalEnemyList;
    private List<string> bossEnemyList;
    
    private Vector3[] basicEnemySpawnPos;
    
    // Start is called before the first frame update
    void Awake()
    {
        init();
    }

    private void init()
    {
        enemyInfos = new Dictionary<string, EnemyInfo>();
        enemyObjects = new Dictionary<string, Enemy>();
        
        Dictionary<string, EnemyInfo> tempEnemyInfos =
            JsonManager.LoadJsonFile<Dictionary<string, EnemyInfo>>(JsonManager.DEFAULT_ENEMY_DATA_NAME);

        foreach (KeyValuePair<string, EnemyInfo> data in tempEnemyInfos)
        {
            enemyObjects.Add(data.Key, Resources.Load<Enemy>("Prefabs/enemies/" + data.Key));
            enemyInfos.Add(data.Key, data.Value);
        }

        activeEnemies = new Dictionary<int, Enemy>();
        inactiveEnemies = new Queue<Enemy>();
        bossEnemy = new Dictionary<int, Enemy>();

        key = 0;
        killedEnemiesCount = 0;

        bossPhase = 0;
        isBossActive = false;

        normalEnemyList = new List<string>();
        bossEnemyList = new List<string>();

        List<Dictionary<string, object>> StageDB = CSVReader.Read(CSV_FILENAME_STAGE);

        foreach (Dictionary<string, object> stageInfo in StageDB)
        {
            if (stageInfo["StageCode"].ToString() != DEFAULT_STAGE_CODE) continue;

            normalEnemyCount = (int)stageInfo["NormalEnemyCount"];
            bossEnemyCount = (int)stageInfo["BossEnemyCount"];

            for (int idx = 1; idx <= normalEnemyCount; idx++)
            {
                normalEnemyList.Add(stageInfo["NormalEnemy" + idx].ToString());
            }

            for (int idx = 1; idx <= bossEnemyCount; idx++)
            {
                bossEnemyList.Add(stageInfo["BossEnemy" + idx].ToString());
            }
        }
        
        // enemy spawn position
        basicEnemySpawnPos = new Vector3[DEFAULT_ENEMY_SPAWN_POS_COUNT];

        for (int i = 0; i < DEFAULT_ENEMY_SPAWN_POS_COUNT; i++)
        {
            basicEnemySpawnPos[i] = Quaternion.Euler(0f, (RIGHT_ANGLE * i) + DEFAULT_BASIC_ENEMY_SPAWN_ANGLE, 0f) *
                                    SPAWN_ENEMY_POSITION;
        }

        player = GameManager.GetInstance().GetPlayer().transform;
        InstantiateEnemies();
        StartCoroutine(SpawnNormalEnemies());
        StartCoroutine(SpawnBossEnemy(20f));
    }

    public static EnemyManager GetInstance()
    {
        if (instance != null) return instance;
        instance = FindObjectOfType<EnemyManager>();
        if (instance == null) Debug.Log("There's no active EnemyManager object");
        return instance;
    }

    private void InstantiateEnemies()
    {
        Enemy tempEnemy;

        for (int count = 0; count < MAX_ENEMY_COUNT; count++)
        {
            tempEnemy = Instantiate(enemyObjects[normalEnemyList[DEFAULT_NORMAL_ENEMY_INDEX]], this.transform, true);
            inactiveEnemies.Enqueue(tempEnemy);
            tempEnemy.gameObject.SetActive(false);
        }
    }

    private IEnumerator SpawnNormalEnemies()
    {
        yield return new WaitForSeconds(DEFAULT_SPAWN_DELAY);
        if (isBossActive) yield break;

        Enemy tempEnemy;
        for (int count = 0; count < DEFAULT_SPAWN_COUNT; count++)
        {
            tempEnemy = inactiveEnemies.Dequeue();
            tempEnemy.gameObject.SetActive(true);
            tempEnemy.transform.position = player.position +
                                           (Quaternion.Euler(0f,
                                               Random.Range(-DEFAULT_ENEMY_SPAWN_RANGE, DEFAULT_ENEMY_SPAWN_RANGE),
                                               0f) * basicEnemySpawnPos[Random.Range(0, DEFAULT_ENEMY_SPAWN_POS_COUNT)]);
            tempEnemy.Init(enemyInfos[normalEnemyList[DEFAULT_NORMAL_ENEMY_INDEX]], player, key);
            activeEnemies.Add(key++, tempEnemy);
        }

        StartCoroutine(SpawnNormalEnemies());
    }

    private IEnumerator SpawnBossEnemy(float waitingTime)
    {
        if (bossPhase >= bossEnemyCount) yield break;
        yield return new WaitForSeconds(waitingTime);
        
        Enemy tempEnemy = Instantiate(enemyObjects[bossEnemyList[bossPhase]], this.transform, true);
        tempEnemy.Init(enemyInfos[bossEnemyList[bossPhase]], player, key);
        tempEnemy.transform.localPosition = player.gameObject.transform.localPosition + SPAWN_BOSS_POSITION;
        tempEnemy.GetComponent<BossEnemy>().InitBoss();
        bossEnemy.Add(key, tempEnemy);
        key++;

        foreach (Enemy enemy in activeEnemies.Values)
        {
            enemy.gameObject.SetActive(false);
            inactiveEnemies.Enqueue(enemy);
        }

        activeEnemies.Clear();

        isBossActive = true;
        bossPhase++;
    }

    public void UpdateEnemyStatus(Enemy.EnemyGrade enemyGrade, int key)
    {
        switch (enemyGrade)
        {
            case Enemy.EnemyGrade.NORMAL:
                if (activeEnemies[key].GetCharacterState() == Character.CharacterState.DEAD)
                {
                    inactiveEnemies.Enqueue(activeEnemies[key]);
                    activeEnemies.Remove(key);
                    killedEnemiesCount++;
                }
                break;
            
            case Enemy.EnemyGrade.ELITE:
            case Enemy.EnemyGrade.BOSS:
                if (bossEnemy[key].GetCharacterState() == Character.CharacterState.DEAD)
                {
                    Enemy tempEnemy = bossEnemy[key];
                    bossEnemy.Remove(key);
                    Destroy(tempEnemy.gameObject);
                    killedEnemiesCount++;

                    if (bossPhase >= bossEnemyCount)
                    {
                        GameManager.GetInstance().ClearGame();
                        return;
                    }

                    isBossActive = false;
                    StartCoroutine(SpawnNormalEnemies());
                    StartCoroutine(SpawnBossEnemy(20f));
                }

                break;
            
            
        }
    }

    public int GetKilledEnemiesCount()
    {
        return killedEnemiesCount;
    }
}
