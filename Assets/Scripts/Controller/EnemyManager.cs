using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private static EnemyManager instance;

    private static string CSV_FILENAME_STAGE = "DataTable_Stage";
    private const string NULL_STRING = "";
    private static int MAX_ENEMY_COUNT = 1200;
    private static int DEFAULT_NORMAL_ENEMY_INDEX = 0;
    private static int DEFAULT_GROUP_ENEMY_INDEX = 0;
    private static int DEFAULT_GUARD_ENEMY_INDEX = 1;

    private static int DEFAULT_ENEMY_SPAWN_POS_COUNT = 4;
    private static Vector3 SPAWN_ENEMY_POSITION = new Vector3(30.0f, 0.0f, 0.0f);
    private static Vector3 SPAWN_BOSS_POSITION = new Vector3(0.0f, 0.0f, 15.0f);
    private const float DEFAULT_SPAWN_CYCLE = 1f;
    private const int DEFAULT_SPAWN_COUNT = 2;
    private const float DEFAULT_RANGED_SPAWN_CYCLE = 30f;
    private const float DEFAULT_RANGED_SPAWN_DELAY = 5f;
    private const int DEFAULT_RANGED_SPAWN_COUNT = 4;
    private static float RIGHT_ANGLE = 90f;
    private static float DEFAULT_BASIC_ENEMY_SPAWN_ANGLE = 30f;
    private static float DEFAULT_ENEMY_SPAWN_RANGE = 30f;
    private static float DEFAULT_BOSS_ENEMY_SPAWN_DELAY = 120f;
    private static float DEFAULT_BOSS_WARNING_DURATION = 5f;
    private static float DEFAULT_ELITE_ENEMY_SPAWN_DELAY = 40f;
    private const float DEFAULT_SPAWN_PHASE_CHANGE_DELAY = 60f;

    private const float DEFAULT_GROUP_ENEMY_SPAWN_DELAY = 60f;
    private const int DEFAULT_SPAWN_GROUP_COUNT = 18;
    private const float DEFAULT_GROUP_ENEMY_DURATION = 10f;
    
    private const int DEFAULT_SPAWN_GUARD_COUNT = 60;
    private const float DEFAULT_GUARD_ENEMY_SPAWN_DISTANCE = 15f;

    private const float DEFAULT_TWO_RADIANS = 360f;
    private const float DEFAULT_ONE_THIRD_RADIAN = 60f;

    private const int MAX_DAMAGE = 9999;
    private const float DEFAULT_BOMB_RANGE = 30f;
    
    // attributes
    private int killedEnemiesCount;
    private Transform player;
    
    // associations
    public Light light;
    
    private Dictionary<string, EnemyInfo> enemyInfos;
    
    private Dictionary<int, Enemy> activeEnemies;
    private Dictionary<int, Enemy> bossEnemy;
    private Queue<Enemy> inactiveEnemies;
    private int key;

    private int bossPhase;
    private int spawnPhase;
    private bool isBossActive;
    
    private Dictionary<string, Enemy> enemyObjects;

    // Stage enemy info
    private int normalEnemyCount;
    private int rangedEnemyCount;
    private int specialEnemyCount;
    private int eliteEnemyCount;
    private int bossEnemyCount;
    private int guardPatternCount;
    
    private List<string> normalEnemyList;
    private List<string> rangedEnemyList;
    private List<string> specialEnemyList;
    private List<string> eliteEnemyList;
    private List<string> bossEnemyList;
    private List<string> guardPatterns;
    
    private Dictionary<string, GuardInfo> guardInfos;
    
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
        spawnPhase = 0;
        isBossActive = false;

        normalEnemyList = new List<string>();
        rangedEnemyList = new List<string>();
        specialEnemyList = new List<string>();
        eliteEnemyList = new List<string>();
        bossEnemyList = new List<string>();
        guardPatterns = new List<string>();
        
        Dictionary<string, StageInfo> stageInfos =
            JsonManager.LoadJsonFile<Dictionary<string, StageInfo>>(JsonManager.DEFAULT_STAGE_DATA_NAME);

        StageInfo stageInfo = stageInfos[
            JsonManager.LoadJsonFile<Dictionary<string, CharacterData>>(JsonManager.DEFAULT_CHARACTER_DATA_NAME)[
                JsonManager.LoadJsonFile<CurrentCharacterInfo>(JsonManager.DEFAULT_CURRENT_CHARACTER_DATA_NAME)
                    .currentSelectedCode].currentStage.ToString()];

        normalEnemyCount = stageInfo.normalEnemyCount;
        rangedEnemyCount = stageInfo.rangedEnemyCount;
        specialEnemyCount = stageInfo.specialEnemyCount;
        eliteEnemyCount = stageInfo.eliteEnemyCount;
        bossEnemyCount = stageInfo.bossEnemyCount;
        guardPatternCount = stageInfo.guardPatternCount;

        normalEnemyList = stageInfo.normalEnemies.ToList();
        rangedEnemyList = stageInfo.rangedEnemies.ToList();
        specialEnemyList = stageInfo.specialEnemies.ToList();
        eliteEnemyList = stageInfo.eliteEnemies.ToList();
        bossEnemyList = stageInfo.bossEnemies.ToList();
        guardPatterns = stageInfo.guardPatterns.ToList();

        // enemy spawn position
        basicEnemySpawnPos = new Vector3[DEFAULT_ENEMY_SPAWN_POS_COUNT];

        for (int i = 0; i < DEFAULT_ENEMY_SPAWN_POS_COUNT; i++)
        {
            basicEnemySpawnPos[i] = Quaternion.Euler(0f, (RIGHT_ANGLE * i) + DEFAULT_BASIC_ENEMY_SPAWN_ANGLE, 0f) *
                                    SPAWN_ENEMY_POSITION;
        }

        player = GameManager.GetInstance().GetPlayer().transform;
        InstantiateEnemies();
        
        // guard enemy infos;
        guardInfos = JsonManager.LoadJsonFile<Dictionary<string, GuardInfo>>(JsonManager.DEFAULT_GUARD_DATA_NAME);

        StartCoroutine(spawnNormalEnemies());
        StartCoroutine(spawnRangedEnemies());
        startSpawnRoutine();
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

        //for (int count = 0; count < MAX_ENEMY_COUNT; count++)
        //{
        //    tempEnemy = Instantiate(enemyObjects[normalEnemyList[DEFAULT_NORMAL_ENEMY_INDEX]], this.transform, true);
        //    inactiveEnemies.Enqueue(tempEnemy);
        //    tempEnemy.gameObject.SetActive(false);
        //}
    }

    private IEnumerator spawnNormalEnemies()
    {
        yield return new WaitForSeconds(DEFAULT_SPAWN_CYCLE);

        Enemy tempEnemy;
        for (int count = 0; count < DEFAULT_SPAWN_COUNT; count++)
        {
            tempEnemy = Instantiate(enemyObjects[normalEnemyList[spawnPhase]],
                player.position +
                (Quaternion.Euler(0f,
                    Random.Range(-DEFAULT_ENEMY_SPAWN_RANGE, DEFAULT_ENEMY_SPAWN_RANGE),
                    0f) * basicEnemySpawnPos[Random.Range(0, DEFAULT_ENEMY_SPAWN_POS_COUNT)]),
                Quaternion.identity,
                this.transform); //inactiveEnemies.Dequeue();
            tempEnemy.Init(enemyInfos[normalEnemyList[spawnPhase]], player, key);
            activeEnemies.Add(key++, tempEnemy);
        }

        StartCoroutine(spawnNormalEnemies());
    }

    private IEnumerator spawnRangedEnemies() {
        yield return new WaitForSeconds(DEFAULT_RANGED_SPAWN_CYCLE);

        Enemy tempEnemy;

        for (int spawnCount = 0; spawnCount <= DEFAULT_RANGED_SPAWN_COUNT; spawnCount++ ){
            for (int count = 0; count < DEFAULT_RANGED_SPAWN_COUNT; count++) {
                tempEnemy = Instantiate(enemyObjects[rangedEnemyList[spawnPhase]],
                    player.position +
                    (Quaternion.Euler(0f,
                        Random.Range(-DEFAULT_ENEMY_SPAWN_RANGE, DEFAULT_ENEMY_SPAWN_RANGE),
                        0f) * basicEnemySpawnPos[Random.Range(0, DEFAULT_ENEMY_SPAWN_POS_COUNT)]),
                    Quaternion.identity,
                    this.transform); //inactiveEnemies.Dequeue();
                tempEnemy.Init(enemyInfos[rangedEnemyList[spawnPhase]], player, key);
                activeEnemies.Add(key++, tempEnemy);
                yield return new WaitForSeconds(DEFAULT_RANGED_SPAWN_DELAY);
            }
        }

        StartCoroutine(spawnRangedEnemies());
    }

    private IEnumerator spawnGroupEnemies()
    {
        yield return new WaitForSeconds(DEFAULT_GROUP_ENEMY_SPAWN_DELAY);
        if (isBossActive && bossPhase >= bossEnemyCount) yield break;

        StartCoroutine(spawnGroupEnemies());

        Enemy tempEnemy;
        int posIdx = Random.Range(0, DEFAULT_ENEMY_SPAWN_POS_COUNT);
        float interval = 1f;
        float angle = DEFAULT_ONE_THIRD_RADIAN;
        float current = 0f;
        float max = DEFAULT_TWO_RADIANS / angle;

        for (int count = 0; count < DEFAULT_SPAWN_GROUP_COUNT; count++)
        {
            tempEnemy = Instantiate(enemyObjects[specialEnemyList[DEFAULT_GROUP_ENEMY_INDEX]],
                player.position + basicEnemySpawnPos[posIdx] +
                (Quaternion.Euler(0f, angle * current++, 0f)
                 * new Vector3(interval, 0f, 0f)),
                Quaternion.identity,
                this.transform);
            tempEnemy.gameObject.SetActive(true);
            tempEnemy.Init(enemyInfos[specialEnemyList[DEFAULT_GROUP_ENEMY_INDEX]], player, key,
                -basicEnemySpawnPos[posIdx].normalized);
            activeEnemies.Add(key++, tempEnemy);
            StartCoroutine(removeEnemy(tempEnemy, DEFAULT_GROUP_ENEMY_DURATION));

            if (current >= max)
            {
                angle /= 2f;
                max = DEFAULT_TWO_RADIANS / angle;
                current = 0f;
                interval += 1f;
            }
        }
    }

    private IEnumerator spawnGuardEnemies(string type, float width, float height, float spawnDelay, float spawnDuration, bool loop)
    {
        yield return new WaitForSeconds(spawnDelay);
        if (isBossActive) yield break;

        Enemy tempEnemy;
        switch (type)
        {
            case "STAY":
                for (float i = -1.5f * (width - 1); i <= 1.5f * (width - 1); i++)
                {
                    tempEnemy = Instantiate(enemyObjects[specialEnemyList[DEFAULT_GUARD_ENEMY_INDEX]],
                        player.position + new Vector3(i, 0f, -0.5f * (height - 1)),
                        Quaternion.identity,
                        this.transform);
                    tempEnemy.gameObject.SetActive(true);
                    tempEnemy.Init(enemyInfos[specialEnemyList[DEFAULT_GUARD_ENEMY_INDEX]], player, key, Vector3.zero);
                    activeEnemies.Add(key++, tempEnemy);
                    StartCoroutine(removeEnemy(tempEnemy, spawnDuration));
                    
                    tempEnemy = Instantiate(enemyObjects[specialEnemyList[DEFAULT_GUARD_ENEMY_INDEX]],
                        player.position + new Vector3(i, 0f, 0.5f * (height - 1)),
                        Quaternion.identity,
                        this.transform);
                    tempEnemy.gameObject.SetActive(true);
                    tempEnemy.Init(enemyInfos[specialEnemyList[DEFAULT_GUARD_ENEMY_INDEX]], player, key, Vector3.zero);
                    activeEnemies.Add(key++, tempEnemy);
                    StartCoroutine(removeEnemy(tempEnemy, spawnDuration));
                }

                for (float i = -1.5f * (height - 1); i <= 1.5f * (height - 1); i++)
                {
                    tempEnemy = Instantiate(enemyObjects[specialEnemyList[DEFAULT_GUARD_ENEMY_INDEX]],
                        player.position + new Vector3(-0.5f * (width - 1), 0f, i),
                        Quaternion.identity,
                        this.transform);
                    tempEnemy.gameObject.SetActive(true);
                    tempEnemy.Init(enemyInfos[specialEnemyList[DEFAULT_GUARD_ENEMY_INDEX]], player, key, Vector3.zero);
                    activeEnemies.Add(key++, tempEnemy);
                    StartCoroutine(removeEnemy(tempEnemy, spawnDuration));
                    
                    tempEnemy = Instantiate(enemyObjects[specialEnemyList[DEFAULT_GUARD_ENEMY_INDEX]],
                        player.position + new Vector3(0.5f * (width - 1), 0f, i),
                        Quaternion.identity,
                        this.transform);
                    tempEnemy.gameObject.SetActive(true);
                    tempEnemy.Init(enemyInfos[specialEnemyList[DEFAULT_GUARD_ENEMY_INDEX]], player, key, Vector3.zero);
                    activeEnemies.Add(key++, tempEnemy);
                    StartCoroutine(removeEnemy(tempEnemy, spawnDuration));
                }
                break;
            
            case "CHASE_PLAYER":
                break;
            
            case "CHASE_LEFT":
                for (float i = -0.5f * (height - 1); i <= 0.5f * (height - 1); i++)
                {
                    tempEnemy = Instantiate(enemyObjects[specialEnemyList[DEFAULT_GUARD_ENEMY_INDEX]],
                        player.position + new Vector3(width, 0f, i),
                        Quaternion.identity,
                        this.transform);
                    tempEnemy.gameObject.SetActive(true);
                    tempEnemy.Init(enemyInfos[specialEnemyList[DEFAULT_GUARD_ENEMY_INDEX]], player, key, Vector3.left);
                    activeEnemies.Add(key++, tempEnemy);
                    StartCoroutine(removeEnemy(tempEnemy, spawnDuration));
                }
                break;
            
            case "CHASE_RIGHT":
                for (float i = -0.5f * (height - 1); i <= 0.5f * (height - 1); i++)
                {
                    tempEnemy = Instantiate(enemyObjects[specialEnemyList[DEFAULT_GUARD_ENEMY_INDEX]],
                        player.position + new Vector3(-width, 0f, i),
                        Quaternion.identity,
                        this.transform);
                    tempEnemy.gameObject.SetActive(true);
                    tempEnemy.Init(enemyInfos[specialEnemyList[DEFAULT_GUARD_ENEMY_INDEX]], player, key, Vector3.right);
                    activeEnemies.Add(key++, tempEnemy);
                    StartCoroutine(removeEnemy(tempEnemy, spawnDuration));
                }
                break;

            default:
                break;
        }
        
        /*
        int current = 0;

        for (int count = 0; count < DEFAULT_SPAWN_GUARD_COUNT; count++)
        {
            tempEnemy = Instantiate(enemyObjects[normalEnemyList[DEFAULT_GUARD_ENEMY_INDEX]],
                player.position + 
                (Quaternion.Euler(0f, DEFAULT_TWO_RADIANS / DEFAULT_SPAWN_GUARD_COUNT * current++, 0f)
                 * new Vector3(DEFAULT_GUARD_ENEMY_SPAWN_DISTANCE, 0f, 0f)),
                Quaternion.identity,
                this.transform);
            tempEnemy.gameObject.SetActive(true);
            tempEnemy.Init(enemyInfos[normalEnemyList[DEFAULT_GUARD_ENEMY_INDEX]], player, key,
                (player.position - tempEnemy.transform.position).normalized);
            activeEnemies.Add(key++, tempEnemy);
            StartCoroutine(removeEnemy(tempEnemy, DEFAULT_GROUP_ENEMY_DURATION));
        }
        */

        if(loop) StartCoroutine(spawnGuardEnemies(type, width, height, spawnDelay, spawnDuration, loop));
    }

    private IEnumerator spawnEliteEnemy(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (isBossActive) yield break;
        
        Enemy tempEnemy;

        tempEnemy = Instantiate(enemyObjects[eliteEnemyList[spawnPhase]],
            player.position +
            (Quaternion.Euler(0f,
                Random.Range(-DEFAULT_ENEMY_SPAWN_RANGE, DEFAULT_ENEMY_SPAWN_RANGE),
                0f) * basicEnemySpawnPos[Random.Range(0, DEFAULT_ENEMY_SPAWN_POS_COUNT)]),
            Quaternion.identity,
            this.transform); //inactiveEnemies.Dequeue();
        tempEnemy.Init(enemyInfos[eliteEnemyList[spawnPhase]], player, key);
        activeEnemies.Add(key++, tempEnemy);

        StartCoroutine(spawnEliteEnemy(delay));
    }

    private IEnumerator spawnBossEnemy(float waitingTime)
    {
        if (bossPhase >= bossEnemyCount) yield break;
        yield return new WaitForSeconds(waitingTime);

        SoundManager.GetInstance().ChangeBGM("bossStage", false);
        
        Enemy tempEnemy = Instantiate(enemyObjects[bossEnemyList[bossPhase]], this.transform, true);
        tempEnemy.Init(enemyInfos[bossEnemyList[bossPhase]], player, key);
        tempEnemy.transform.localPosition = player.gameObject.transform.localPosition + SPAWN_BOSS_POSITION;
        tempEnemy.GetComponent<BossEnemy>().InitBoss();
        bossEnemy.Add(key++, tempEnemy);

        bossPhase++;

        if (bossPhase >= bossEnemyCount)
        {
            foreach (Enemy enemy in activeEnemies.Values)
            {
                enemy.gameObject.SetActive(false);
                inactiveEnemies.Enqueue(enemy);
            }

            activeEnemies.Clear();
        }

        isBossActive = true;
    }

    public void UpdateEnemyStatus(Enemy.EnemyGrade enemyGrade, int key)
    {
        switch (enemyGrade)
        {
            case Enemy.EnemyGrade.NORMAL:
            case Enemy.EnemyGrade.GROUP:
            case Enemy.EnemyGrade.GUARD:
            case Enemy.EnemyGrade.ELITE:
                if (activeEnemies[key].GetCurrentDamage() != MAX_DAMAGE)
                    UIManager.GetInstance().ShowDamageText(activeEnemies[key].transform.position,
                        activeEnemies[key].GetCurrentDamage());

                if (activeEnemies[key].GetCharacterState() == Character.CharacterState.DEAD)
                {
                    inactiveEnemies.Enqueue(activeEnemies[key]);
                    activeEnemies.Remove(key);
                    killedEnemiesCount++;
                    UIManager.GetInstance().UpdateKilledEnemyCount(killedEnemiesCount);
                }
                break;
            
            case Enemy.EnemyGrade.BOSS:
                UIManager.GetInstance().ShowDamageText(bossEnemy[key].transform.position,
                    bossEnemy[key].GetCurrentDamage());
                
                if (bossEnemy[key].GetCharacterState() == Character.CharacterState.DEAD)
                {
                    Enemy tempEnemy = bossEnemy[key];
                    bossEnemy.Remove(key);
                    Destroy(tempEnemy.gameObject);
                    killedEnemiesCount++;
                    UIManager.GetInstance().UpdateKilledEnemyCount(killedEnemiesCount);
                    spawnPhase++;
                    StartCoroutine(makeLightDarker(0.05f, 10));

                    if (bossPhase >= bossEnemyCount)
                    {
                        GameManager.GetInstance().ClearGame();
                        GameManager.GetInstance().AddReward();
                        return;
                    }

                    SoundManager.GetInstance().ChangeBGM(GameManager.GetInstance().GetStageName(), true);

                    isBossActive = false;
                    startSpawnRoutine();
                }

                break;
        }
    }

    private IEnumerator changeSpawnPhase(float delay)
    {
        yield return new WaitForSeconds(DEFAULT_SPAWN_PHASE_CHANGE_DELAY);

        if (spawnPhase < normalEnemyCount - 1
            && !isBossActive)
        {
            spawnPhase++;
            StartCoroutine(changeSpawnPhase(delay));
        }
        
    }

    public int GetKilledEnemiesCount()
    {
        return killedEnemiesCount;
    }

    private IEnumerator removeEnemy(Enemy enemy, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (enemy.gameObject.activeSelf == false) yield break;

        enemy.gameObject.SetActive(false);
        inactiveEnemies.Enqueue(activeEnemies[enemy.GetKey()]);
        activeEnemies.Remove(enemy.GetKey());


    }

    private void startSpawnRoutine()
    {
        GuardInfo info;
        StartCoroutine(spawnGroupEnemies());
        StartCoroutine(spawnBossEnemy(DEFAULT_BOSS_ENEMY_SPAWN_DELAY));
        StartCoroutine(UIManager.GetInstance().WarningBossStage(DEFAULT_BOSS_ENEMY_SPAWN_DELAY - DEFAULT_BOSS_WARNING_DURATION, DEFAULT_BOSS_WARNING_DURATION));
        StartCoroutine(spawnEliteEnemy(DEFAULT_ELITE_ENEMY_SPAWN_DELAY));
        StartCoroutine(changeSpawnPhase(DEFAULT_SPAWN_PHASE_CHANGE_DELAY));

        foreach (string code in guardPatterns)
        {
            if (code == NULL_STRING) break;
            
            info = guardInfos[code];
            StartCoroutine(spawnGuardEnemies(info.guardType, info.width, info.height, info.spawnDelay, info.spawnDuration, info.loop));
        }
    }

    public void Bomb()
    {
        List<Enemy> tempEnemies = new List<Enemy>();
        foreach (Enemy enemy in activeEnemies.Values)
        {
            if (Vector3.Distance(player.position, enemy.transform.position) <= DEFAULT_BOMB_RANGE)
                tempEnemies.Add(enemy);
        }

        for (int i = tempEnemies.Count - 1; i >= 0; i--)
        {
            tempEnemies[i].TakeDamage(MAX_DAMAGE);
        }
    }

    private IEnumerator makeLightDarker(float value, int repeatTime)
    {
        for (int i = 0; i < repeatTime; i++)
        {
            yield return new WaitForSeconds(0.5f);
            light.intensity -= value;
        }
    }

}
