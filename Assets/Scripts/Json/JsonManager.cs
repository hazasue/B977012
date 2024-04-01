using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class JsonManager
{
    public static string DEFAULT_CHARACTER_DATA_NAME = "CharacterData";
    public static string DEFAULT_CURRENT_CHARACTER_DATA_NAME = "CurrentCharacterData";
    public static string DEFAULT_BASIC_CHARACTER_DATA_NAME = "DataTable_BasicCharacterData";
    public static string DEFAULT_STAGE_DATA_NAME = "DataTable_Stage";
    public static string DEFAULT_ENEMY_DATA_NAME = "DataTable_Enemy";
    public static string DEFAULT_WEAPON_DATA_NAME = "DataTable_Weapon";
    public static string DEFAULT_WEAPON_UPGRADE_DATA_NAME = "DataTable_WeaponUpgrade";
    public static string DEFAULT_SKILL_DATA_NAME = "DataTable_Skill";
    public static string DEFAULT_ENHANCEMENT_DATA_NAME = "DataTable_Enhancement";
    public static string DEFAULT_ENHANCEMENT_BACKUP_DATA_NAME = "DataTable_Enhancement_BackUp";
    public static string DEFAULT_SETTING_DATA_NAME = "Settings";
    public static string DEFAULT_GUARD_DATA_NAME = "DataTable_GuardInfo";

    public static void CreateJsonFile(string fileName, object obj)
    {
        // 데이터 폴더가 없다면 생성하기
        if (!File.Exists(Application.dataPath + "/Data/"))
        {
            Directory.CreateDirectory(Application.dataPath + "/Data/");
        }

        FileStream fileStream = new FileStream(Application.dataPath + "/Data/" + fileName + ".json", FileMode.OpenOrCreate);
        byte[] data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));
        fileStream.SetLength(0);
        fileStream.Write(data, 0, data.Length);
        fileStream.Close();
    }

    public static T LoadJsonFile<T>(string fileName)
    {
        if (!File.Exists(Application.dataPath + "/Data/" + fileName + ".json"))
        {
            Debug.Log(Application.dataPath + "/Data/" + fileName + ".json" + ":  Does not exist.");
            return default(T);
        }
        
        FileStream fileStream = new FileStream(Application.dataPath + "/Data/" + fileName + ".json", FileMode.Open);
        byte[] data = new byte[fileStream.Length];
        fileStream.Read(data, 0, data.Length);
        fileStream.Close();
        string jsonData = Encoding.UTF8.GetString(data);
        return JsonConvert.DeserializeObject<T>(jsonData);
    }
}
