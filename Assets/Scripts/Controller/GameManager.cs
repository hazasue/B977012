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

    private void Init()
    {
        mUIManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        mEnemyManager = GameObject.Find("EnemyManager").GetComponent<EnemyManager>();

        Dictionary<string, CharacterData> characterDatas = JsonManager.GetInstance()
            .LoadJsonFile<Dictionary<string, CharacterData>>(JsonManager.DEFAULT_CHARACTER_DATA_NAME);
        
        string characterIndex = JsonManager.GetInstance()
            .LoadJsonFile<CurrentCharacterInfo>(JsonManager.DEFAULT_CURRENT_CHARACTER_DATA_NAME).currentSelectedCode;
        
        player = Instantiate(Resources.Load<Player>("prefabs/characters/" + characterDatas[characterIndex].playerType));
        player.Init(characterDatas[characterIndex].maxHp,
            characterDatas[characterIndex].damage,
            characterDatas[characterIndex].speed,
            characterDatas[characterIndex].armor);

        player.GetInventory().AddWeapon(WeaponManager.GetInstance().GetWeaponInfo(characterDatas[characterIndex].basicWeapon));
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
