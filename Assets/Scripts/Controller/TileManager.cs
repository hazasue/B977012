using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    private static TileManager instance;

    private static float DEFAULT_TILE_SIZE = 100f;
    private static int DEFAULT_TILE_COUNT = 9;
    private static int DEFAULT_COLLIDER_COUNT = 4;
    private const int DEFAULT_SMALL_TILE_COUNT = 10;
    
    private Dictionary<string, CharacterData> characterDatas;
    private string characterIndex;
    private StageInfo stageInfo;

    private Material tileMaterial;

    public List<MeshRenderer> tileTextures;
    public List<Tile> tiles;

    void Awake()
    {
        init();
    }

    private void init()
    {
        characterDatas = JsonManager.LoadJsonFile<Dictionary<string, CharacterData>>(JsonManager.DEFAULT_CHARACTER_DATA_NAME);
        characterIndex = JsonManager.LoadJsonFile<CurrentCharacterInfo>(JsonManager.DEFAULT_CURRENT_CHARACTER_DATA_NAME).currentSelectedCode;
        stageInfo =
            JsonManager.LoadJsonFile<Dictionary<string, StageInfo>>(JsonManager.DEFAULT_STAGE_DATA_NAME)[
                characterDatas[characterIndex].currentStage.ToString()];

        tileMaterial = Resources.Load<Material>("Materials/" + stageInfo.texture);
        
        tileTextures = new List<MeshRenderer>();
        tiles = new List<Tile>();
        for (int i = 0; i < DEFAULT_COLLIDER_COUNT; i++)
        {
            tiles.Add(this.transform.GetChild(1 + 2 * i).GetComponent<Tile>());
        }

        for (int i = 0; i < DEFAULT_TILE_COUNT; i++)
        {
            for (int j = 0; j < DEFAULT_SMALL_TILE_COUNT; j++)
            {
                for (int k = 0; k < DEFAULT_SMALL_TILE_COUNT; k++)
                {
                    tileTextures.Add(this.transform.GetChild(i).GetChild(0).GetChild(j).GetChild(k).GetComponent<MeshRenderer>());
                }
            }
        }

        foreach (MeshRenderer texture in tileTextures)
        {
            texture.material = tileMaterial;
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
