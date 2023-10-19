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
    private static Color DEFAULT_SLOT_COLOR_UNSELECTED = new Color32(128, 128, 128, 255);
    private static Color DEFAULT_SLOT_COLOR_SELECTED = new Color32(255, 255, 255, 255);
    private static int MAX_CHARACTER_COUNT = 3;
    private static int DEFAULT_SELECTED_INDEX = 0;

    public GameObject createScreen;
    public Image[] characterImage = new Image[MAX_CHARACTER_COUNT];
    public TMP_Text[] characterSlots = new TMP_Text[MAX_CHARACTER_COUNT];
    private CharacterData tempData;

    private int selectedSlot;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    private void Init()
    {
        selectedSlot = DEFAULT_SELECTED_INDEX;
        UpdateCharacterScreen();
    }

    public void CreateCharacter(string playerType)
    {
        if (tempData.playerCode.Count >= MAX_CHARACTER_COUNT) return;
        
        List<Dictionary<string, object>> CharacterDB = CSVReader.Read(CSV_FILENAME_CHARACTER);
        
        foreach (Dictionary<string, object> characterInfo in CharacterDB)
        {
            if (characterInfo["CharacterCode"].ToString() != playerType) continue;

            tempData.AddData(characterInfo["CharacterCode"].ToString(),
                characterInfo["BasicWeapon"].ToString(),
                characterInfo["CharacterType"].ToString(),
                (int)characterInfo["MaxHp"],
                (int)characterInfo["Damage"],
                float.Parse(characterInfo["Speed"].ToString()),
                (int)characterInfo["Armor"]
            );
            break;
        }
        
        JsonManager.GetInstance().CreateJsonFile(JsonManager.DEFAULT_CHARACTER_DATA_NAME, tempData);

        UpdateCharacterScreen();
    }

    public void DeleteCharacter()
    {
        if (selectedSlot >= tempData.playerCode.Count) return;
        
        tempData.RemoveData(selectedSlot);
        JsonManager.GetInstance().CreateJsonFile(JsonManager.DEFAULT_CHARACTER_DATA_NAME, tempData);
        UpdateCharacterScreen();

        if (selectedSlot >= tempData.playerCode.Count)
        {
            selectedSlot = DEFAULT_SELECTED_INDEX;
            SelectSlot(selectedSlot);
        }
    }

    private void UpdateCharacterScreen()
    {
        if (!File.Exists(Application.dataPath + "/Data/" + JsonManager.DEFAULT_CHARACTER_DATA_NAME + ".json")
            || !JsonManager.GetInstance().LoadJsonFile<CharacterData>(JsonManager.DEFAULT_CHARACTER_DATA_NAME).Exist(DEFAULT_SELECTED_INDEX))
        {
            for(int slot = 0; slot < MAX_CHARACTER_COUNT; slot++)
            {
                characterSlots[slot].text = "null";
                characterImage[slot].color = DEFAULT_SLOT_COLOR_UNSELECTED;
            }
            tempData = new CharacterData();
            JsonManager.GetInstance().CreateJsonFile(JsonManager.DEFAULT_CHARACTER_DATA_NAME, tempData);
        }
        else
        {
            tempData = JsonManager.GetInstance().LoadJsonFile<CharacterData>(JsonManager.DEFAULT_CHARACTER_DATA_NAME);
            for(int slot = 0; slot < MAX_CHARACTER_COUNT; slot++)
            {
                if (tempData.Exist(slot))
                {
                    characterSlots[slot].text = tempData.playerType[slot];
                    characterImage[slot].color = DEFAULT_SLOT_COLOR_SELECTED;
                }
                else
                {
                    characterSlots[slot].text = "null";
                    characterImage[slot].color = DEFAULT_SLOT_COLOR_UNSELECTED;
                }
            }
        }
    }

    public void PlayGame()
    {
        if (!tempData.Exist(selectedSlot)) return;
        
        JsonManager.GetInstance().CreateJsonFile(JsonManager.DEFAULT_CURRENT_CHARACTER_DATA_NAME, new CurrentCharacterInfo(selectedSlot));
        CommonUIManager.GetInstance().MoveScene("InGame");
    }

    public void SelectSlot(int slot)
    {
        if (slot >= MAX_CHARACTER_COUNT) return;
        if (!tempData.Exist(slot)) return;
        
        selectedSlot = slot;

        characterSlots[slot].text += "\nSelected"; // Change info effect, not text
    }

    public void ActiveCreateScreen(bool _bool)
    {
        createScreen.SetActive(_bool);
    }
}
