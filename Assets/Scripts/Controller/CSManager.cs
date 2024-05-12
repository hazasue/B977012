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
    public GameObject[] characterSlots = new GameObject[MAX_CHARACTER_COUNT];

    public TMP_Text codeText;
    public TMP_Text playerTypeText;
    public TMP_Text basicWeaponText;
    public TMP_Text basicSkillText;
    public TMP_Text equipmentsText;

    public Sprite warriorImage;
    public Sprite wizardImage;

    private Dictionary<string, CharacterData> characterInfos;
    private Dictionary<string, CharacterData> characterDatas;
    private CurrentCharacterInfo currentCharacterInfo;

    private int selectedSlot;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    void Update()
    {
        if (createScreen.activeSelf == false) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ActiveCreateScreen(false);
        }
    }

    private void Init()
    {
        characterInfos =
            JsonManager.LoadJsonFile<Dictionary<string, CharacterData>>(JsonManager.DEFAULT_BASIC_CHARACTER_DATA_NAME);
        
        selectedSlot = DEFAULT_SELECTED_INDEX;
        UpdateCharacterInfo();
    }

    public void CreateCharacter(string playerType)
    {
        if (characterDatas.Count >= MAX_CHARACTER_COUNT) return;

        characterDatas.Add((currentCharacterInfo.currentCreatedCode.ToString()),
            characterInfos[playerType]);
        currentCharacterInfo.currentSelectedCode = currentCharacterInfo.currentCreatedCode.ToString();
        currentCharacterInfo.currentCreatedCode++;

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
        }
        SelectSlot(selectedSlot);
    }

    private void UpdateCharacterInfo()
    {
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
        
        if (!File.Exists(Application.dataPath + "/Data/" + JsonManager.DEFAULT_CHARACTER_DATA_NAME + ".json")
            || JsonManager.LoadJsonFile<Dictionary<string, CharacterData>>(JsonManager.DEFAULT_CHARACTER_DATA_NAME).Count <= 0)
        {
            for (int slot = 0; slot < MAX_CHARACTER_COUNT; slot++)
            {
                codes[slot] = DEFAULT_NULL_CHARACTER;
                characterImage[slot].gameObject.SetActive(false);
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
                characterImage[slot].gameObject.SetActive(true);
                switch (data.Value.playerType)
                {
                    case "WARRIOR":
                        characterImage[slot].sprite = warriorImage;
                        break;
                    
                    case "WIZARD":
                        characterImage[slot].sprite = wizardImage;
                        break;
                    
                    default:
                        Debug.Log("Invalid player type: " + data.Value.playerType);
                        break;
                }
                if (data.Key == currentCharacterInfo.currentSelectedCode.ToString()
                    && currentCharacterInfo.currentSelectedCode != DEFAULT_NULL_CHARACTER)
                {
                    characterImage[slot].color = DEFAULT_SLOT_COLOR_SELECTED;
                    SelectSlot(slot);
                }
                else
                {
                    characterImage[slot].color = DEFAULT_SLOT_COLOR_UNSELECTED;
                }
                slot++;
            }

            for (; slot < MAX_CHARACTER_COUNT; slot++)
            {
                codes[slot] = DEFAULT_NULL_CHARACTER;
                characterImage[slot].gameObject.SetActive(false);
            }
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
        CommonUIManager.GetInstance().MoveScene("Lobby");
    }

    public void SelectSlot(int slot)
    {
        if (slot >= MAX_CHARACTER_COUNT) return;
        if (codes[slot] == DEFAULT_NULL_CHARACTER)
        {
            codeText.text = "";
            playerTypeText.text = "";
            basicWeaponText.text = "";
            basicSkillText.text = "";
            equipmentsText.text = "";
            return;
        }
        
        selectedSlot = slot;
        currentCharacterInfo.currentSelectedCode = codes[selectedSlot];
        JsonManager.CreateJsonFile(JsonManager.DEFAULT_CURRENT_CHARACTER_DATA_NAME, currentCharacterInfo);
        
        for (int i = 0; i < MAX_CHARACTER_COUNT; i++)
        {
            if (i == slot)
            {
                characterSlots[i].SetActive(true);
                characterImage[i].color = DEFAULT_SLOT_COLOR_SELECTED;
            }
            else
            {
                characterSlots[i].SetActive(false);
                characterImage[i].color = DEFAULT_SLOT_COLOR_UNSELECTED;
            }
        }

        CharacterData tempData = characterDatas[codes[selectedSlot]];
        codeText.text = codes[selectedSlot];
        playerTypeText.text = tempData.playerType;
        basicWeaponText.text = WeaponManager.GetInstance().GetWeaponInfo(tempData.basicWeapon).GetName();
        basicSkillText.text = tempData.basicSkillName;
        equipmentsText.text = "";
        foreach (string code in tempData.equipmentCodes)
        {
            equipmentsText.text += WeaponManager.GetInstance().GetWeaponInfo(code).GetName() + "\n";
        }
    }

    public void ActiveCreateScreen(bool _bool)
    {
        createScreen.SetActive(_bool);
    }
}
