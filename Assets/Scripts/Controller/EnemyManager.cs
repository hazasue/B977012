using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private static EnemyManager instance;

    private static string CSV_FILENAME_STAGE = "DataTable_Stage";
    private static string CSV_FILENAME_ENEMY = "DataTable_Enemy";
    private static string DEFAULT_STAGE_CODE = "Stage1";
    private static int MAX_ENEMY_COUNT = 1200;
    private static int DEFAULT_NORMAL_ENEMY_INDEX = 0;
    private const float DEFAULT_SPAWN_DELAY = 1f;
    private const int DEFAULT_SPAWN_COUNT = 1;
    
    // attributes
    private int killedEnemiesCount;
    private Transform player;
    
    // associations
    private Dictionary<string, EnemyInfo> enemyInfos;
    
    private Dictionary<int, Enemy> activeEnemies;
    private Queue<Enemy> unactiveEnemies;
    private int key;

    private Dictionary<string, Enemy> enemyObjects;

    // Stage enemy info
    private int normalEnemyCount;
    private int bossEnemyCount;
    
    private List<string> normalEnemyList;
    private List<string> bossEnemyList;
    
    // Start is called before the first frame update
    void Awake()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Init()
    {
        enemyInfos = new Dictionary<string, EnemyInfo>();
        enemyObjects = new Dictionary<string, Enemy>();

        List<Dictionary<string, object>> EnemyDB = CSVReader.Read(CSV_FILENAME_ENEMY);

        foreach (Dictionary<string, object> enemyInfo in EnemyDB)
        {
            enemyObjects.Add(enemyInfo["EnemyCode"].ToString(),
                Resources.Load<Enemy>("Prefabs/enemies/" + enemyInfo["EnemyCode"].ToString()));
            enemyInfos.Add(enemyInfo["EnemyCode"].ToString(),
                new EnemyInfo(enemyInfo["EnemyCode"].ToString(),
                    enemyInfo["EnemyType"].ToString(),
                    enemyInfo["EnemyGrade"].ToString(),
                    (int)enemyInfo["MaxHp"],
                    (int)enemyInfo["Damage"],
                    float.Parse(enemyInfo["Speed"].ToString()),
                    (int)enemyInfo["Armor"],
                    float.Parse(enemyInfo["TickTime"].ToString()),
                    (int)enemyInfo["Exp"]
                ));
        }

        activeEnemies = new Dictionary<int, Enemy>();
        unactiveEnemies = new Queue<Enemy>();

        key = 0;
        killedEnemiesCount = 0;

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

        player = GameManager.GetInstance().GetPlayer().transform;
        InstantiateEnemies();
        StartCoroutine(SpawnNormalEnemies());
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
            unactiveEnemies.Enqueue(tempEnemy);
            tempEnemy.gameObject.SetActive(false);
        }
    }

    private IEnumerator SpawnNormalEnemies()
    {
        yield return new WaitForSeconds(DEFAULT_SPAWN_DELAY);

        Enemy tempEnemy;
        for (int count = 0; count < DEFAULT_SPAWN_COUNT; count++)
        {
            tempEnemy = unactiveEnemies.Dequeue();
            tempEnemy.gameObject.SetActive(true);
            tempEnemy.transform.position = player.position + new Vector3(5f, 0f, 5f);
            tempEnemy.Init(enemyInfos[normalEnemyList[DEFAULT_NORMAL_ENEMY_INDEX]], player, key);
            activeEnemies.Add(key++, tempEnemy);
        }

        StartCoroutine(SpawnNormalEnemies());
    }

    public void UpdateEnemyStatus(int key)
    {
        if (activeEnemies[key].GetCharacterState() == Character.CharacterState.DEAD)
        {
            unactiveEnemies.Enqueue(activeEnemies[key]);
            activeEnemies.Remove(key);
            killedEnemiesCount++;
        }
    }

    public int GetKilledEnemiesCount()
    {
        return killedEnemiesCount;
    }
}
