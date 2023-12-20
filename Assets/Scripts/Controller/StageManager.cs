using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    private const float DEFAULT_ONE_BLOCK_WIDTH = 700f;
    private static Color32 TRANSLUCENT_COLOR = new Color32(255, 255, 255, 104);
    private static Color32 OPACITY_COLOR = new Color32(255, 255, 255, 255);
    private const int DEFAULT_STAGE_ENTRANCE_STATE_UI_INDEX = 0;

    public GameObject stagePanel;
    public Transform stageContent;
    public Stage stageObject;

    public GameObject leftButton;
    public GameObject rightButton;

    private Dictionary<string, CharacterData> characterDatas;
    private Dictionary<string, StageInfo> stageInfos;
    private string characterIndex;

    private int currentStage;
    private bool adjusting;

    void Start()
    {
        init();
    }

    void Update()
    {
        if (!stagePanel.activeSelf) return;

        applyKeyInput();

    }

    private void init()
    {
        LoadCharacterDatas();
        stageInfos = JsonManager.LoadJsonFile<Dictionary<string, StageInfo>>(JsonManager.DEFAULT_STAGE_DATA_NAME);

        Stage tempStage;
        foreach (StageInfo data in stageInfos.Values)
        {
            tempStage = Instantiate(stageObject, stageContent, true);
            tempStage.Init(data.stageCode);
        }

        stageContent.localPosition =
            new Vector3(-DEFAULT_ONE_BLOCK_WIDTH * characterDatas[characterIndex].currentStage, 0f, 0f);

        currentStage = -1;
        adjusting = false;
        UpdateUIState();
    }
    
    
    private void applyKeyInput()
    {
        if (adjusting) return;

        if (Input.GetMouseButtonUp(0))
        {
            adjustStageContentPosition();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow)
                 && currentStage > 0)
        {
            adjustStageContentPosition(-1);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow)
                 && currentStage < stageInfos.Count - 1)
        {
            adjustStageContentPosition(1);
        }
    }

    public void LoadCharacterDatas()
    {
        characterDatas = JsonManager.LoadJsonFile<Dictionary<string, CharacterData>>(JsonManager.DEFAULT_CHARACTER_DATA_NAME);
        characterIndex =
            JsonManager.LoadJsonFile<CurrentCharacterInfo>(JsonManager.DEFAULT_CURRENT_CHARACTER_DATA_NAME)
                .currentSelectedCode;
    }

    private void adjustStageContentPosition(int variation = 0)
    {
        adjusting = true;
        StartCoroutine(moveProperPosition(convertIndexToVector3(findProperStageIndex() + variation)));
    }

    private Vector3 convertIndexToVector3(int index)
    {
        return new Vector3(-DEFAULT_ONE_BLOCK_WIDTH * index, 0f, 0f);
    }

    private int findProperStageIndex()
    {
        for (int i = 0; i < stageInfos.Count; i++)
        {
            if (stageContent.localPosition.x >= -DEFAULT_ONE_BLOCK_WIDTH * i - DEFAULT_ONE_BLOCK_WIDTH / 2
                && stageContent.localPosition.x <= -DEFAULT_ONE_BLOCK_WIDTH * i + DEFAULT_ONE_BLOCK_WIDTH / 2)
            {
                return i;
            }
        }

        return -1;
    }

    private IEnumerator moveProperPosition(Vector3 properPosition)
    {
        while (true)
        {
            yield return new WaitForSeconds(Time.deltaTime);

            stageContent.localPosition = Vector3.Lerp(properPosition, stageContent.localPosition, 0.5f);
            if (stageContent.localPosition.x <= properPosition.x + 5f
                && stageContent.localPosition.x >= properPosition.x - 5f)
            {
                int index = (int)-properPosition.x / (int)DEFAULT_ONE_BLOCK_WIDTH;
                stageContent.localPosition = properPosition;
                characterDatas[characterIndex].currentStage = index;
                JsonManager.CreateJsonFile(JsonManager.DEFAULT_CHARACTER_DATA_NAME, characterDatas);
                adjusting = false;
                yield break;
            }
        }
    }

    public void MoveStage(int variation)
    {
        if (adjusting) return;
        switch (variation)
        {
            case -1:
                if (currentStage <= 0) return;
                adjustStageContentPosition(variation);
                break;
            
            case 1:
                if (currentStage >= stageInfos.Count - 1) return;
                adjustStageContentPosition(variation);
                break;
            
            default:
                Debug.Log("Invalid variation: " + variation);
                break;
        }
    }

    
    public void UpdateUIState()
    {
        if (currentStage == findProperStageIndex()) return;
        
        currentStage = findProperStageIndex();
        
        if (currentStage <= 0) leftButton.SetActive(false);
        else if (currentStage >= stageInfos.Count - 1) rightButton.SetActive(false);
        else
        {
            leftButton.SetActive(true);
            rightButton.SetActive(true);
        }

        changeStageEntranceInfo();
        changeStageAlphaValue();
    }
    
    private void changeStageAlphaValue()
    {
        for (int i = 0; i < stageContent.childCount; i++)
        {
            stageContent.GetChild(i).GetComponent<Image>().color = TRANSLUCENT_COLOR;
        }

        stageContent.GetChild(currentStage).GetComponent<Image>().color = OPACITY_COLOR;
    }

    private void changeStageEntranceInfo()
    {
        for (int i = 1; i < stageContent.childCount; i++)
        {
            if (!characterDatas[characterIndex].clearStages[i - 1])
            {
                stageContent.GetChild(i).GetChild(DEFAULT_STAGE_ENTRANCE_STATE_UI_INDEX).gameObject.SetActive(true);
            }
        }
    }

    public void StartGame()
    {
        if (currentStage == 0
            || characterDatas[characterIndex].clearStages[currentStage - 1])
        {
            SceneManager.LoadScene("InGame");
        }
    }
}
