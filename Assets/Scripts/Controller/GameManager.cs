using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private enum GameStatus
    {
        PLAYING,
        PAUSED,
        FAIL,
        CLEAR,
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
        init();
    }

    private void init()
    {
        mUIManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        mEnemyManager = GameObject.Find("EnemyManager").GetComponent<EnemyManager>();
        gameStatus = GameStatus.PLAYING;

        Time.timeScale = 1;

        Dictionary<string, CharacterData> characterDatas = JsonManager.LoadJsonFile<Dictionary<string, CharacterData>>(JsonManager.DEFAULT_CHARACTER_DATA_NAME);
        
        string characterIndex = JsonManager.LoadJsonFile<CurrentCharacterInfo>(JsonManager.DEFAULT_CURRENT_CHARACTER_DATA_NAME).currentSelectedCode;
        
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

    public void FailGame()
    {
        gameStatus = GameStatus.FAIL;
        PauseGame();
        mUIManager.ActivateFailScreen();
    }

    public void ClearGame()
    {
        gameStatus = GameStatus.CLEAR;
        PauseGame();
        mUIManager.ActivateClearScreen();
    }

    public Player GetPlayer() { return player; }

    public void PauseGame() { Time.timeScale = 0; }

    public void ResumeGame() { Time.timeScale = 1; }
}
