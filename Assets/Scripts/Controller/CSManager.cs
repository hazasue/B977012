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
    private static string DEFAULT_NULL_CHARACTER = "NULL_CHARACTER";
    private static Color DEFAULT_SLOT_COLOR_UNSELECTED = new Color32(128, 128, 128, 255);
    private static Color DEFAULT_SLOT_COLOR_SELECTED = new Color32(255, 255, 255, 255);
    private static int MAX_CHARACTER_COUNT = 3;
    private static int DEFAULT_SELECTED_INDEX = 0;

    public string[] codes = new string[MAX_CHARACTER_COUNT];
    public GameObject createScreen;
    public Image[] characterImage = new Image[MAX_CHARACTER_COUNT];
    public TMP_Text[] characterSlots = new TMP_Text[MAX_CHARACTER_COUNT];

    private Dictionary<string, CharacterData> characterDatas;
    private CurrentCharacterInfo currentCharacterInfo;

    private int selectedSlot;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    private void Init()
    {
        selectedSlot = DEFAULT_SELECTED_INDEX;
        UpdateCharacterInfo();
    }

    public void CreateCharacter(string playerType)
    {
        if (characterDatas.Count >= MAX_CHARACTER_COUNT) return;
        
        List<Dictionary<string, object>> CharacterDB = CSVReader.Read(CSV_FILENAME_CHARACTER);
        
        foreach (Dictionary<string, object> characterInfo in CharacterDB)
        {
            if (characterInfo["CharacterCode"].ToString() != playerType) continue;

            characterDatas.Add((currentCharacterInfo.currentCreatedCode.ToString()),
                new CharacterData(
                    characterInfo["BasicWeapon"].ToString(),
                    characterInfo["CharacterType"].ToString(),
                    (int)characterInfo["MaxHp"],
                    (int)characterInfo["Damage"],
                    float.Parse(characterInfo["Speed"].ToString()),
                    (int)characterInfo["Armor"])
            );
            currentCharacterInfo.currentCreatedCode++;
            break;
        }
        
        JsonManager.CreateJsonFile(JsonManager.DEFAULT_CHARACTER_DATA_NAME, characterDatas);
        JsonManager.CreateJsonFile(JsonManager.DEFAULT_CURRENT_CHARACTER_DATA_NAME, currentCharacterInfo);

        UpdateCharacterInfo();
    }

    public void DeleteCharacter()
    {
        if (selectedSlot >= MAX_CHARACTER_COUNT)
        {
            selectedSlot = DEFAULT_SELECTED_INDEX;
            return;
        }
        if (codes[selectedSlot] == DEFAULT_NULL_CHARACTER) return;

        characterDatas.Remove(codes[selectedSlot]);
        JsonManager.CreateJsonFile(JsonManager.DEFAULT_CHARACTER_DATA_NAME, characterDatas);
        UpdateCharacterInfo();

        if (selectedSlot >= characterDatas.Count)
        {
            selectedSlot = DEFAULT_SELECTED_INDEX;
            SelectSlot(selectedSlot);
        }
    }

    private void UpdateCharacterInfo()
    {
        if (!File.Exists(Application.dataPath + "/Data/" + JsonManager.DEFAULT_CHARACTER_DATA_NAME + ".json")
            || JsonManager.LoadJsonFile<Dictionary<string, CharacterData>>(JsonManager.DEFAULT_CHARACTER_DATA_NAME).Count <= 0)
        {
            for (int slot = 0; slot < MAX_CHARACTER_COUNT; slot++)
            {
                codes[slot] = DEFAULT_NULL_CHARACTER;
                characterSlots[slot].text = "null";
                characterImage[slot].color = DEFAULT_SLOT_COLOR_UNSELECTED;
            }

            characterDatas = new Dictionary<string, CharacterData>();
            JsonManager.CreateJsonFile(JsonManager.DEFAULT_CHARACTER_DATA_NAME, characterDatas);
        }
        else
        {
            characterDatas =
                JsonManager.LoadJsonFile<Dictionary<string, CharacterData>>(JsonManager.DEFAULT_CHARACTER_DATA_NAME);
            int slot = 0;
            foreach (KeyValuePair<string, CharacterData> data in characterDatas)
            {
                codes[slot] = data.Key;
                characterSlots[slot].text = codes[slot] + "\n" +data.Value.playerType;
                characterImage[slot].color = DEFAULT_SLOT_COLOR_SELECTED;
                slot++;
            }

            for (; slot < MAX_CHARACTER_COUNT; slot++)
            {
                codes[slot] = DEFAULT_NULL_CHARACTER;
                characterSlots[slot].text = "null";
                characterImage[slot].color = DEFAULT_SLOT_COLOR_UNSELECTED;
            }
        }

        if (!File.Exists(Application.dataPath + "/Data/" + JsonManager.DEFAULT_CURRENT_CHARACTER_DATA_NAME + ".json"))
        {
            currentCharacterInfo = new CurrentCharacterInfo();
            currentCharacterInfo.currentCreatedCode = 0;
            currentCharacterInfo.currentSelectedCode = DEFAULT_NULL_CHARACTER;
            JsonManager.CreateJsonFile(JsonManager.DEFAULT_CURRENT_CHARACTER_DATA_NAME, currentCharacterInfo);
        }
        else
        {
            currentCharacterInfo = JsonManager.LoadJsonFile<CurrentCharacterInfo>(JsonManager.DEFAULT_CURRENT_CHARACTER_DATA_NAME);
        }
    }

    public void PlayGame()
    {
        if (selectedSlot >= MAX_CHARACTER_COUNT)
        {
            selectedSlot = DEFAULT_SELECTED_INDEX;
            return;
        }
        if (codes[selectedSlot] == DEFAULT_NULL_CHARACTER) return;
        
        JsonManager.CreateJsonFile(JsonManager.DEFAULT_CURRENT_CHARACTER_DATA_NAME, currentCharacterInfo);
        CommonUIManager.GetInstance().MoveScene("InGame");
    }

    public void SelectSlot(int slot)
    {
        if (slot >= MAX_CHARACTER_COUNT) return;
        if (codes[slot] == DEFAULT_NULL_CHARACTER) return;
        
        selectedSlot = slot;
        currentCharacterInfo.currentSelectedCode = codes[selectedSlot];

        characterSlots[slot].text += "\nSelected"; // Change info effect, not text
    }

    public void ActiveCreateScreen(bool _bool)
    {
        createScreen.SetActive(_bool);
    }
}
