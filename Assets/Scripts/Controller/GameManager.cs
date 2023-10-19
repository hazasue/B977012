using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private enum GameStatus
    {
        playing,
        paused,
        fail,
        clear,
    }

    private static GameManager instance;

    private static string DEFAULT_CHARACTER_NAME = "warrior";
    
    // attributes
    private GameStatus gameStatus;
    private float time;
    
    // associations
    private Player player;
    private UIManager mUIManager;
    private EnemyManager mEnemyManager;
    
    private void Awake()
    {
        Init();
    }
    
    private void Update()
    {
        
    }

    private void Init()
    {
        mUIManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        mEnemyManager = GameObject.Find("EnemyManager").GetComponent<EnemyManager>();

        CharacterData tempData = JsonManager.GetInstance().LoadJsonFile<CharacterData>(JsonManager.DEFAULT_CHARACTER_DATA_NAME);
        
        int characterIndex = JsonManager.GetInstance()
            .LoadJsonFile<CurrentCharacterInfo>(JsonManager.DEFAULT_CURRENT_CHARACTER_DATA_NAME).currentCharacterCode;
        
        player = Instantiate(Resources.Load<Player>("prefabs/characters/" + tempData.playerType[characterIndex]));
        player.Init(tempData.maxHp[characterIndex],
            tempData.damage[characterIndex],
            tempData.speed[characterIndex],
            tempData.armor[characterIndex]);

        player.GetInventory().AddWeapon(WeaponManager.GetInstance().GetWeaponInfo(tempData.basicWeapon[characterIndex]));
    }

    public static GameManager GetInstance()
    {
        if (instance != null) return instance;
        instance = FindObjectOfType<GameManager>();
        if (instance == null) Debug.Log("There's no active GameManager object");
        return instance;
    }

    private void UpdateGameStatus() {}

    public Player GetPlayer() { return player; }
}
