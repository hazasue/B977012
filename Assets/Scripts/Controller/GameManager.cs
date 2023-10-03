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
    
    // attributes
    private GameStatus gameStatus;
    private float time;
    
    // associations
    private Player player;
    
    private void Awake()
    {
        
    }
    
    private void Update()
    {
        
    }
    
    private void Init() {}
    
    private void SetDirections() {}
    
    private void UpdateGameStatus() {}
}
