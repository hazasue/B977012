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
        SetDirections();
    }

    private void Init()
    {
        mUIManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        mEnemyManager = GameObject.Find("EnemyManager").GetComponent<EnemyManager>();

        CharacterData tempData = JsonManager.GetInstance()
            .LoadJsonFile<CharacterData>(JsonManager.DEFAULT_CHARACTER_DATA_NAME);
        player = Instantiate(Resources.Load<Player>("prefabs/characters/" + tempData.playerType));
        player.Init(tempData.maxHp, tempData.damage, tempData.speed, tempData.armor);
    }

    public static GameManager GetInstance()
    {
        if (instance != null) return instance;
        instance = FindObjectOfType<GameManager>();
        if (instance == null) Debug.Log("There's no active GameManager object");
        return instance;
    }

    private void SetDirections()
    {
        Vector3 direction = Vector3.zero;
        if (Input.GetKey(KeyCode.UpArrow))
        {
            direction += Vector3.forward;
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            direction += Vector3.back;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            direction += Vector3.left;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            direction += Vector3.right;
        }

        player.SetDirections(direction.normalized);
    }
    
    private void UpdateGameStatus() {}

    public Player GetPlayer() { return player; }
}
