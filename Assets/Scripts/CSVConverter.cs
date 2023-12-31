using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CSVConverter : MonoBehaviour
{
    private static string CSV_FILENAME_WEAPON = "DataTable_Weapon";
    private static string CSV_FILENAME_WEAPON_UPGRADE = "DataTable_WeaponUpgrade";
    private static string CSV_FILENAME_ENEMY = "DataTable_Enemy";
    private static string CSV_FILENAME_STAGE = "DataTable_Stage";
    private static string CSV_FILENAME_CHARACTER = "DataTable_Character";
    private static string CSV_FILENAME_SKILL = "DataTable_Skill";
    private static string CSV_FILENAME_ENHANCEMENT = "DataTable_Enhancement";

    private static int DEFAULT_ORDER_NUMBER = 1;
    
    public void ConvertCsvToJson()
    {
        // weapon
        List<Dictionary<string, object>> WeaponDB = CSVReader.Read(CSV_FILENAME_WEAPON);
        Dictionary<string, WeaponInfo> weaponInfos = new Dictionary<string, WeaponInfo>();
        foreach (Dictionary<string, object> weaponInfo in WeaponDB)
        {
            weaponInfos.Add(weaponInfo["WeaponCode"].ToString(),
                new WeaponInfo(weaponInfo["WeaponCode"].ToString(),
                    weaponInfo["WeaponName"].ToString(),
                    weaponInfo["WeaponType"].ToString(),
                    (int)weaponInfo["Damage"],
                    float.Parse(weaponInfo["Duration"].ToString()),
                    float.Parse(weaponInfo["Delay"].ToString()),
                    (int)weaponInfo["Projectile"],
                    float.Parse(weaponInfo["Range"].ToString()),
                    float.Parse(weaponInfo["Speed"].ToString()),
                    weaponInfo["Occupation"].ToString(),
                    (int)weaponInfo["Order"],
                    (int)weaponInfo["Price"],
                    weaponInfo["Description"].ToString()
                ));
        }

        JsonManager.CreateJsonFile(JsonManager.DEFAULT_WEAPON_DATA_NAME, weaponInfos);

        // upgrade
        List<Dictionary<string, object>> UpgradeDB = CSVReader.Read(CSV_FILENAME_WEAPON_UPGRADE);
        Dictionary<string, WeaponUpgradeInfo> upgradeInfos = new Dictionary<string, WeaponUpgradeInfo>();
        foreach (Dictionary<string, object> upgradeInfo in UpgradeDB)
        {
            upgradeInfos.Add(upgradeInfo["WeaponCode"].ToString(),
                new WeaponUpgradeInfo(upgradeInfo["WeaponCode"].ToString(),
                    upgradeInfo["Option1"].ToString(),
                    float.Parse(upgradeInfo["Value1"].ToString()),
                    upgradeInfo["Option2"].ToString(),
                    float.Parse(upgradeInfo["Value2"].ToString())
                ));
        }
        JsonManager.CreateJsonFile(JsonManager.DEFAULT_WEAPON_UPGRADE_DATA_NAME, upgradeInfos);
        
        // enemy
        List<Dictionary<string, object>> EnemyDB = CSVReader.Read(CSV_FILENAME_ENEMY);
        Dictionary<string, EnemyInfo> enemyInfos = new Dictionary<string, EnemyInfo>();
        foreach (Dictionary<string, object> enemyInfo in EnemyDB)
        {
            enemyInfos.Add(enemyInfo["EnemyCode"].ToString(),
                new EnemyInfo(enemyInfo["EnemyCode"].ToString(),
                    enemyInfo["EnemyType"].ToString(),
                    enemyInfo["EnemyGrade"].ToString(),
                    (int)enemyInfo["MaxHp"],
                    (int)enemyInfo["Damage"],
                    float.Parse(enemyInfo["Speed"].ToString()),
                    (int)enemyInfo["Armor"],
                    float.Parse(enemyInfo["TickTime"].ToString()),
                    (int)enemyInfo["Exp"]
                ));
        }
        JsonManager.CreateJsonFile(JsonManager.DEFAULT_ENEMY_DATA_NAME, enemyInfos);
        
        // stage
        List<Dictionary<string, object>> StageDB = CSVReader.Read(CSV_FILENAME_STAGE);
        Dictionary<string, StageInfo> stageInfos = new Dictionary<string, StageInfo>();
        List<string> normalEnemies = new List<string>();
        List<string> bossEnemies = new List<string>();
        foreach (Dictionary<string, object> stageInfo in StageDB)
        {
            normalEnemies.Clear();
            bossEnemies.Clear();
            normalEnemies.Add(stageInfo["NormalEnemy1"].ToString());
            normalEnemies.Add(stageInfo["NormalEnemy2"].ToString());
            normalEnemies.Add(stageInfo["NormalEnemy3"].ToString());
            bossEnemies.Add(stageInfo["BossEnemy1"].ToString());
            bossEnemies.Add(stageInfo["BossEnemy2"].ToString());
            bossEnemies.Add(stageInfo["BossEnemy3"].ToString());

            stageInfos.Add(stageInfo["StageCode"].ToString(),
                new StageInfo(stageInfo["StageCode"].ToString(),
                    (int)stageInfo["NormalEnemyCount"],
                    (int)stageInfo["BossEnemyCount"],
                    normalEnemies.ToList(),
                    bossEnemies.ToList(),
                    (int)stageInfo["BasicReward"],
                    stageInfo["Texture"].ToString()
                ));
        }
        JsonManager.CreateJsonFile(JsonManager.DEFAULT_STAGE_DATA_NAME, stageInfos);
        
        // character info(basic)
        List<Dictionary<string, object>> CharacterDB = CSVReader.Read(CSV_FILENAME_CHARACTER);
        Dictionary<string, CharacterData> characterInfos = new Dictionary<string, CharacterData>();
        foreach (Dictionary<string, object> characterInfo in CharacterDB)
        {
            List<string> equipmentCodes = new List<string>();
            List<bool> clearStages = new List<bool>();
            for (int i = 0; i < 3; i++) { clearStages.Add(false); }
            equipmentCodes.Add(characterInfo["BasicWeapon"].ToString());
            characterInfos.Add(characterInfo["CharacterCode"].ToString(),
                new CharacterData(
                    characterInfo["BasicWeapon"].ToString(),
                    characterInfo["BasicSkill"].ToString(),
                    characterInfo["CharacterType"].ToString(),
                    (int)characterInfo["MaxHp"],
                    (int)characterInfo["Damage"],
                    float.Parse(characterInfo["Speed"].ToString()),
                    (int)characterInfo["Armor"],
                    DEFAULT_ORDER_NUMBER,
                    equipmentCodes,
                    clearStages)
            );
        }
        
        JsonManager.CreateJsonFile(JsonManager.DEFAULT_BASIC_CHARACTER_DATA_NAME, characterInfos);
        
        // skill
        List<Dictionary<string, object>> SkillDB = CSVReader.Read(CSV_FILENAME_SKILL);
        Dictionary<string, SkillInfo> skillInfos = new Dictionary<string, SkillInfo>();
        foreach (Dictionary<string, object> skillInfo in SkillDB)
        {
            skillInfos.Add(skillInfo["SkillCode"].ToString(),
                new SkillInfo(skillInfo["SkillCode"].ToString(),
                    skillInfo["SkillType"].ToString(),
                    float.Parse(skillInfo["Delay"].ToString()),
                    float.Parse(skillInfo["Duration"].ToString()),
                    skillInfo["Stat"].ToString(),
                    float.Parse(skillInfo["Value"].ToString()),
                    (int)skillInfo["Damage"],
                    (int)skillInfo["Projectile"]
                    ));
        }
        
        JsonManager.CreateJsonFile(JsonManager.DEFAULT_SKILL_DATA_NAME, skillInfos);
        /*
        // Enhancement
        
        List<Dictionary<string, object>> EnhanceDB = CSVReader.Read(CSV_FILENAME_ENHANCEMENT);
        Dictionary<string, EnhanceInfo> enhanceInfos = new Dictionary<string, EnhanceInfo>();
        foreach (Dictionary<string, object> enhanceInfo in EnhanceDB)
        {
            enhanceInfos.Add(enhanceInfo["EnhanceStat"].ToString(),
                new EnhanceInfo(enhanceInfo["EnhanceStat"].ToString(),
                    float.Parse(enhanceInfo["EnhanceValue"].ToString())
                ));
        }
        
        JsonManager.CreateJsonFile(JsonManager.DEFAULT_ENHANCEMENT_DATA_NAME, enhanceInfos);
        */
    }
}
