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
    
    private static string DEFAULT_WEAPON_CODE_WARRIOR = "warriorMelee";
    private static string DEFAULT_WEAPON_CODE_WIZARD = "wizardTracking";
    
    // attributes
    private GameStatus gameStatus;
    private float time;

    private Dictionary<string, CharacterData> characterDatas;
    private string characterIndex;
    private StageInfo stageInfo;
    
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

        characterDatas = JsonManager.LoadJsonFile<Dictionary<string, CharacterData>>(JsonManager.DEFAULT_CHARACTER_DATA_NAME);
        characterIndex = JsonManager.LoadJsonFile<CurrentCharacterInfo>(JsonManager.DEFAULT_CURRENT_CHARACTER_DATA_NAME).currentSelectedCode;
        stageInfo =
            JsonManager.LoadJsonFile<Dictionary<string, StageInfo>>(JsonManager.DEFAULT_STAGE_DATA_NAME)[
                characterDatas[characterIndex].currentStage.ToString()];
        
        player = Instantiate(Resources.Load<Player>("prefabs/characters/" + characterDatas[characterIndex].playerType));
        player.Init(characterDatas[characterIndex].maxHp,
            characterDatas[characterIndex].damage,
            characterDatas[characterIndex].speed,
            characterDatas[characterIndex].armor);

        player.GetInventory().AddWeapon(WeaponManager.GetInstance().GetWeaponInfo(characterDatas[characterIndex].basicWeapon), true);
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

    public void AddReward()
    {
        characterDatas[characterIndex].coin += stageInfo.basicReward;
        if (!characterDatas[characterIndex].clearStages[characterDatas[characterIndex].currentStage])
        {
            characterDatas[characterIndex].clearStages[characterDatas[characterIndex].currentStage] = true;
            string weaponCode = "";
            switch (characterDatas[characterIndex].playerType)
            {
                case "WARRIOR":
                    weaponCode = DEFAULT_WEAPON_CODE_WARRIOR + (characterDatas[characterIndex].currentStage + 2) + "1";
                    break;
                
                case "WIZARD":
                    weaponCode = DEFAULT_WEAPON_CODE_WIZARD + (characterDatas[characterIndex].currentStage + 2) + "1";
                    break;
                
                default:
                    Debug.Log("Invalid player type: " + characterDatas[characterIndex].playerType);
                    break;
            }

            characterDatas[characterIndex].equipmentCodes.Add(weaponCode);
            characterDatas[characterIndex].order++;
        }
        JsonManager.CreateJsonFile(JsonManager.DEFAULT_CHARACTER_DATA_NAME, characterDatas);
    }

    public Player GetPlayer() { return player; }

    public void PauseGame() { Time.timeScale = 0; }

    public void ResumeGame() { Time.timeScale = 1; }
}
