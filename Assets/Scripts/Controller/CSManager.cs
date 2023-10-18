using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CSManager : MonoBehaviour
{
    private static string CSV_FILENAME_CHARACTER = "DataTable_Character";
    
    private static string DEFAULT_WARRIOR_BASIC_WEAPON = "tempMelee";
    private static string DEFAULT_WIZARD_BASIC_WEAPON = "tempTracking";

    public TMP_Text currentCharacter;
    private CharacterData tempData;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    private void Init()
    {
        Debug.Log(Application.dataPath + "/Data/" + JsonManager.DEFAULT_CHARACTER_DATA_NAME + ".json");
        Debug.Log(File.Exists(Application.dataPath + "/Data/" + JsonManager.DEFAULT_CHARACTER_DATA_NAME + ".json"));
        UpdateCharacterScreen();
    }

    public void CreateCharacter(string playerType)
    {
        List<Dictionary<string, object>> CharacterDB = CSVReader.Read(CSV_FILENAME_CHARACTER);

        CharacterData tempData = new CharacterData();
        foreach (Dictionary<string, object> characterInfo in CharacterDB)
        {
            if (characterInfo["CharacterCode"].ToString() != playerType) continue;

            tempData = new CharacterData(characterInfo["CharacterCode"].ToString(),
                characterInfo["BasicWeapon"].ToString(),
                characterInfo["CharacterType"].ToString(),
                (int)characterInfo["MaxHp"],
                (int)characterInfo["Damage"],
                float.Parse(characterInfo["Speed"].ToString()),
                (int)characterInfo["Armor"]
            );
            break;
        }

        switch (playerType)
        {
            case "warrior":
                JsonManager.GetInstance().CreateJsonFile(JsonManager.DEFAULT_CHARACTER_DATA_NAME, tempData);
                break;
            
            case "wizard":
                JsonManager.GetInstance().CreateJsonFile(JsonManager.DEFAULT_CHARACTER_DATA_NAME, tempData);
                break;
            
            default:
                Debug.Log("Invalid player type:  " + playerType);
                return;
        }

        UpdateCharacterScreen();
    }

    private void UpdateCharacterScreen()
    {
        if (!File.Exists(Application.dataPath + "/Data/" + JsonManager.DEFAULT_CHARACTER_DATA_NAME + ".json")
            || !JsonManager.GetInstance().LoadJsonFile<CharacterData>(JsonManager.DEFAULT_CHARACTER_DATA_NAME).Exist())
        {
            currentCharacter.text = "none";
            JsonManager.GetInstance().CreateJsonFile(JsonManager.DEFAULT_CHARACTER_DATA_NAME, new CharacterData());
        }
        else
        {
            tempData = JsonManager.GetInstance().LoadJsonFile<CharacterData>(JsonManager.DEFAULT_CHARACTER_DATA_NAME);
            currentCharacter.text = "exist";
        }
    }

    public void PlayGame()
    {
        if (!JsonManager.GetInstance().LoadJsonFile<CharacterData>(JsonManager.DEFAULT_CHARACTER_DATA_NAME).Exist()) return;

        CommonUIManager.GetInstance().MoveScene("InGame");
    }
}
