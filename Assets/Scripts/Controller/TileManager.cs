using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    private static TileManager instance;

    private static float DEFAULT_TILE_SIZE = 100f;
    private static int DEFAULT_TILE_COUNT = 9;
    private static int DEFAULT_COLLIDER_COUNT = 4;
    
    public List<Tile> tiles;

    void Awake()
    {
        init();
    }

    private void init()
    {
        tiles = new List<Tile>();
        for (int i = 0; i < DEFAULT_COLLIDER_COUNT; i++)
        {
            tiles.Add(this.transform.GetChild(1 + 2 * i).GetComponent<Tile>());
        }

        instance = this;
    }

    public static TileManager GetInstance()
    {
        if (instance != null) return instance;
        instance = FindObjectOfType<TileManager>();
        if (instance == null) Debug.Log("There's no active TileManager object");
        return instance;
    }

    public void MoveTiles(Tile tile)
    {
        switch (tiles.FindIndex(idx => idx == tile))
        {
            case 0:
                moveTile(0f, DEFAULT_TILE_SIZE);
                break;
            
            case 1:
                moveTile(-DEFAULT_TILE_SIZE, 0f);
                break;
            
            case 2:
                moveTile(DEFAULT_TILE_SIZE, 0f);
                break;
            
            case 3:
                moveTile(0f, -DEFAULT_TILE_SIZE);
                break;
            
            default:
                Debug.Log("Invalid tile index: " + tiles.FindIndex(idx => idx == tile));
                break;
        }
    }

    private void moveTile(float x, float z)
    {
        this.transform.position += new Vector3(x, 0f, z);
    }


}
