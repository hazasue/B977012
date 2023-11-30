using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    private static Vector3 DEFAULT_OBJECT_POSITION = new Vector3(1f, 0f, 0f);
    private static Vector3 DEFAULT_OBJECT_POSITION_Y = new Vector3(0f, 0.5f, 0f);
    private static float DEFAULT_360_DEGREE = 360f;
    private static float DEFAULT_MOVESPEED = 15f;
    
    public enum SkillType
    {
        BUFF,
        DEALING,
    }

    private SkillType skillType;
    private string code;
    private float delay;
    private float duration;
    private string stat;
    private float value;
    private int damage;
    private int projectile;

    private bool canUseSkill;

    private Transform instanceTransform;

    public void Init(SkillInfo skillInfo)
    {
        this.code = skillInfo.code;
        this.delay = skillInfo.delay;
        this.duration = skillInfo.duration;
        this.stat = skillInfo.stat;
        this.value = skillInfo.value;
        this.damage = skillInfo.damage;
        this.projectile = skillInfo.projectile;
        canUseSkill = false;

        switch (skillInfo.skillType)
        {
            case "BUFF":
                this.skillType = SkillType.BUFF;
                break;

            case "DEALING":
                this.skillType = SkillType.DEALING;
                break;

            default:
                Debug.Log("Invalid skill type: " + skillInfo.skillType);
                break;
        }
        
        instanceTransform = GameObject.Find("Projectile Transform").transform;

        StartCoroutine(ableToUseSkill());
    }

    public void UseSkill(Weapon basicWeapon)
    {
        if (!canUseSkill) return;

        switch (skillType)
        {
            case SkillType.BUFF:
                switch (stat)
                {
                    case "delay":
                        StartCoroutine(basicWeapon.ApplyBuffSkill(stat, value, duration));
                        break;
                    
                    case "damage":
                        break;
                    
                    default:
                        Debug.Log("Invalid stat name: " + stat);
                        break;
                }
                break;
            
            case SkillType.DEALING:
                SkillObject tempObject;
                for (int i = 0; i < projectile; i++)
                {
                    tempObject = Instantiate(Resources.Load<SkillObject>("Prefabs/skills/" + code + "_object"), instanceTransform, true);
                    tempObject.transform.position =
                        this.transform.position + DEFAULT_OBJECT_POSITION_Y + Quaternion.Euler(0f, DEFAULT_360_DEGREE / projectile * i, 0f) * DEFAULT_OBJECT_POSITION;
                    tempObject.Init(damage, DEFAULT_MOVESPEED,
                        Quaternion.Euler(0f, DEFAULT_360_DEGREE / projectile * i, 0f) * DEFAULT_OBJECT_POSITION);
                    StartCoroutine(removeSkillObject(tempObject));
                }
                break;
            
            default:
                Debug.Log("Invalid skill type: " + skillType);
                break;
        }
        
        canUseSkill = false;
        UIManager.GetInstance().ResetSkillBar();
        StartCoroutine(ableToUseSkill());
    }

    private IEnumerator ableToUseSkill()
    {
        yield return new WaitForSeconds(delay);

        canUseSkill = true;
    }

    private IEnumerator removeSkillObject(SkillObject skillObject)
    {
        yield return new WaitForSeconds(duration);

        Destroy(skillObject.gameObject);
    }

    public float GetDelay()
    {
        return delay;
    }
}
