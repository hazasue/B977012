using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    public RawImage backGround;
    public GameObject settings;
    
    // Start is called before the first frame update
    void Start()
    {
        if (!File.Exists(Application.dataPath + "/Data/" + JsonManager.DEFAULT_CURRENT_CHARACTER_DATA_NAME + ".json")
            || JsonManager.LoadJsonFile<CurrentCharacterInfo>(JsonManager.DEFAULT_CURRENT_CHARACTER_DATA_NAME)
                .currentSelectedCode == "NULL_CHARACTER"
            || JsonManager.LoadJsonFile<Dictionary<string, CharacterData>>(JsonManager.DEFAULT_CHARACTER_DATA_NAME)[
                JsonManager.LoadJsonFile<CurrentCharacterInfo>(JsonManager.DEFAULT_CURRENT_CHARACTER_DATA_NAME)
                    .currentSelectedCode].playerType == "WARRIOR")
        {
            backGround.texture = Resources.Load<Texture>($"sprites/title/warrior");
        } 
        else
        {
            backGround.texture = Resources.Load<Texture>($"sprites/title/wizard");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) ChangeSettingScreenActivation();
    }

    public void ChangeSettingScreenActivation()
    {
        if (settings.activeSelf == true) settings.SetActive(false);
        else
        {
            settings.SetActive(true);
        }
    }
}
