using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private static string DEFAULT_WEAPON_OBJECT = "Basic Weapon";
    private static string DEFAULT_SKILL_OBJECT = "Basic Skill";
    
    // attributes
    private int coin;
    private float damageMultiple;
    private float coinMultiple;
    public RangeCollider rangeCollider;
    
    // associations
    private Dictionary<string, Weapon> weapons;
    private Dictionary<string, Accessory> accessories;
    private Skill skill;
    
    void Awake()
    {
        Init();
    }

    // methods
    private void Init()
    {
        weapons = new Dictionary<string, Weapon>();
        accessories = new Dictionary<string, Accessory>();
        skill = null;
    }

    public void ApplyEnhanceOptions(float damageMultiple, float coinMultiple)
    {
        this.damageMultiple = damageMultiple;
        this.coinMultiple = coinMultiple;
    }

    public Dictionary<string, Weapon> GetWeapons()
    {
        return weapons;
    }

    public void RemoveWeapon(string code)
    {
        if (weapons.ContainsKey(code))
        {
            Destroy(weapons[code].gameObject);
            weapons.Remove(code);
        }
    }

    public void AddWeapon(WeaponInfo weaponInfo, bool mainWeapon = false)
    {
        if (weaponInfo == null) return;

        GameObject tempWeapon = Instantiate(Resources.Load<GameObject>("Prefabs/weapons/" + DEFAULT_WEAPON_OBJECT), this.transform, true);
        tempWeapon.transform.localPosition = Vector3.zero;
        switch (weaponInfo.GetType())
        {
            case "MELEE":
                tempWeapon.AddComponent<MeleeWeapon>();
                break;
            
            case "RANGED":
                break;
            
            case "TRACKING":
                tempWeapon.AddComponent<TrackingWeapon>();
                break;
            
            case "CHAINING":
                tempWeapon.AddComponent<ChainingWeapon>();
                break;
            
            case "BEAM":
                tempWeapon.AddComponent<BeamWeapon>();
                break;
            
            case "BARRIER":
                tempWeapon.AddComponent<BarrierWeapon>();
                break;
            
            case "EXPLOSIVE":
                break;
            
            case "BOOMERANG":
                tempWeapon.AddComponent<BoomerangWeapon>();
                break;
            
            default:
                break;
        }
        
        RangeCollider tempRangeCollider = Instantiate(rangeCollider, tempWeapon.transform, true);
        tempRangeCollider.transform.localPosition = Vector3.zero;
        Weapon tempWeaponScript = tempWeapon.GetComponent<Weapon>();
        tempWeaponScript.Init(weaponInfo, tempRangeCollider, this.damageMultiple, mainWeapon);
        weapons.Add(tempWeaponScript.GetCode(), tempWeaponScript);
    }

    public Dictionary<string, Accessory> GetAccessories()
    {
        return accessories;
    }
    
    public void AddAccessory(Accessory accessory) {}

    public Skill GetSkill() { return skill; }

    public void AddSkill(SkillInfo skillInfo)
    {
        if (skillInfo == null) return;
        if (skill != null) return;
        
        skill = Instantiate(Resources.Load<Skill>("Prefabs/skills/" + DEFAULT_SKILL_OBJECT), this.transform, true);
        skill.transform.localPosition = Vector3.zero;
        skill.Init(skillInfo);
    }

    public int CheckCoins()
    {
        return coin;
    }

    public void GainCoins(int value)
    {
        this.coin += (int)(value * coinMultiple);
        UIManager.GetInstance().UpdateCoinCount(this.coin);
    }
}
