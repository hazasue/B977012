using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StageManager : MonoBehaviour
{
    private const float DEFAULT_ONE_BLOCK_WIDTH = 700f;
    
    public Transform stageContent;

    private Dictionary<string, CharacterData> characterDatas;
    private Dictionary<string, StageInfo> stageInfos;
    private string characterIndex;

    void Start()
    {
        init();
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            adjustStageContentPosition();
        }
    }

    private void init()
    {
        characterDatas = JsonManager.LoadJsonFile<Dictionary<string, CharacterData>>(JsonManager.DEFAULT_CHARACTER_DATA_NAME);
        stageInfos = JsonManager.LoadJsonFile<Dictionary<string, StageInfo>>(JsonManager.DEFAULT_STAGE_DATA_NAME);
        characterIndex =
            JsonManager.LoadJsonFile<CurrentCharacterInfo>(JsonManager.DEFAULT_CURRENT_CHARACTER_DATA_NAME)
                .currentSelectedCode;

        stageContent.localPosition =
            new Vector3(-DEFAULT_ONE_BLOCK_WIDTH * characterDatas[characterIndex].currentStage, 0f, 0f);
    }

    private void adjustStageContentPosition()
    {
        StartCoroutine(moveProperPosition(findProperPosition()));
    }

    private Vector3 findProperPosition()
    {
        for (int i = 0; i < stageInfos.Count; i++)
        {
            if (stageContent.localPosition.x >= -DEFAULT_ONE_BLOCK_WIDTH * i - DEFAULT_ONE_BLOCK_WIDTH / 2
                && stageContent.localPosition.x <= -DEFAULT_ONE_BLOCK_WIDTH * i + DEFAULT_ONE_BLOCK_WIDTH / 2)
            {
                return new Vector3(-DEFAULT_ONE_BLOCK_WIDTH * i, 0f, 0f);
            }
        }

        return Vector3.zero;
    }

    private IEnumerator moveProperPosition(Vector3 properPosition)
    {
        yield return new WaitForSeconds(Time.deltaTime);

        stageContent.localPosition = Vector3.Lerp(properPosition, stageContent.localPosition, 0.5f);
        
        if (stageContent.localPosition.x <= properPosition.x + 5f
            && stageContent.localPosition.x >= properPosition.x - 5f)
        {
            stageContent.localPosition = properPosition;
            characterDatas[characterIndex].currentStage = (int)-properPosition.x / 500;
            JsonManager.CreateJsonFile(JsonManager.DEFAULT_CHARACTER_DATA_NAME, characterDatas);
            yield break;
        }
        
        StartCoroutine(moveProperPosition(properPosition));
    }
}
