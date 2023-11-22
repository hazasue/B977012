using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    private static SkillManager instance;
    
    private Dictionary<string, SkillInfo> skillInfos;
    
    // Start is called before the first frame update
    void Awake()
    {
        init();
    }

    private void init()
    {
        skillInfos = JsonManager.LoadJsonFile<Dictionary<string, SkillInfo>>(JsonManager.DEFAULT_SKILL_DATA_NAME);
        instance = this;
    }

    public static SkillManager GetInstance()
    {
        if (instance != null) return instance;
        instance = FindObjectOfType<SkillManager>();
        if (instance == null) Debug.Log("There's no active SkillManager object");
        return instance;
    }

    public SkillInfo GetSkillInfo(string code)
    {
        if (skillInfos.ContainsKey(code)) return skillInfos[code];
        else
        {
            Debug.Log("Invalid skill code: " + code);
            return null;
        }
    }
}
