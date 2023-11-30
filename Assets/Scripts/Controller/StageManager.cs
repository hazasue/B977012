using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StageManager : MonoBehaviour
{
    private const float DEFAULT_ONE_BLOCK_WIDTH = 700f;
    private static Color32 TRANSLUCENT_COLOR = new Color32(255, 255, 255, 104);
    private static Color32 OPACITY_COLOR = new Color32(255, 255, 255, 255);

    public GameObject stagePanel;
    public Transform stageContent;
    public Stage stageObject;

    private Dictionary<string, CharacterData> characterDatas;
    private Dictionary<string, StageInfo> stageInfos;
    private string characterIndex;

    private int currentStage;

    void Start()
    {
        init();
    }

    void Update()
    {
        if (!stagePanel.activeSelf) return;
        
        if (Input.GetMouseButtonUp(0))
        {
            adjustStageContentPosition();
        }
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
        ChangeStageAlphaValue();
    }

    public void LoadCharacterDatas()
    {
        characterDatas = JsonManager.LoadJsonFile<Dictionary<string, CharacterData>>(JsonManager.DEFAULT_CHARACTER_DATA_NAME);
        characterIndex =
            JsonManager.LoadJsonFile<CurrentCharacterInfo>(JsonManager.DEFAULT_CURRENT_CHARACTER_DATA_NAME)
                .currentSelectedCode;
    }

    private void adjustStageContentPosition()
    {
        StartCoroutine(moveProperPosition(convertIndexToVector3(findProperStageIndex())));
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
        yield return new WaitForSeconds(Time.deltaTime);

        stageContent.localPosition = Vector3.Lerp(properPosition, stageContent.localPosition, 0.5f);
        
        if (stageContent.localPosition.x <= properPosition.x + 5f
            && stageContent.localPosition.x >= properPosition.x - 5f)
        {
            int index = (int)-properPosition.x / (int)DEFAULT_ONE_BLOCK_WIDTH;
            stageContent.localPosition = properPosition;
            characterDatas[characterIndex].currentStage = index;
            JsonManager.CreateJsonFile(JsonManager.DEFAULT_CHARACTER_DATA_NAME, characterDatas);
            yield break;
        }
        
        StartCoroutine(moveProperPosition(properPosition));
    }

    public void ChangeStageAlphaValue()
    {
        if (currentStage == findProperStageIndex()) return;
        
        currentStage = findProperStageIndex();
        
        for (int i = 0; i < stageContent.childCount; i++)
        {
            stageContent.GetChild(i).GetComponent<Image>().color = TRANSLUCENT_COLOR;
        }

        stageContent.GetChild(currentStage).GetComponent<Image>().color = OPACITY_COLOR;
    }
}
