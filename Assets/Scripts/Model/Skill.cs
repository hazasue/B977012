using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    private static Vector3 DEFAULT_OBJECT_POSITION = new Vector3(1f, 0f, 0f);
    private static Vector3 DEFAULT_OBJECT_POSITION_Y = new Vector3(0f, 0.5f, 0f);
    private static float DEFAULT_360_DEGREE = 360f;
    private static float DEFAULT_MOVESPEED = 15f;
    private static float DEFAULT_SKILL_RANGE = 8f;
    private const int DEFAULT_SKILL_COUNT = 3;
    
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
    private AudioSource audioSource;

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

        audioSource = this.GetComponent<AudioSource>();
        SoundManager.GetInstance().AddToSfxList(audioSource);
        audioSource.volume = SoundManager.GetInstance().audioSourceSfx.volume;
        audioSource.clip = Resources.Load<AudioClip>($"Sfxs/skills/{code}_sound");

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

    public IEnumerator UseSkill(Weapon basicWeapon)
    {
        if (!canUseSkill) yield break;
        
        canUseSkill = false;
        UIManager.GetInstance().ResetSkillBar();
        StartCoroutine(ableToUseSkill());

        switch (skillType)
        {
            case SkillType.BUFF:
                StartCoroutine(basicWeapon.ApplyBuffSkill(stat, value, duration));
                break;

            case SkillType.DEALING:
                SkillObject tempObject;
                Vector3 attackDirection;
                for (int i = 0; i < projectile * DEFAULT_SKILL_COUNT; i++)
                {

                    attackDirection = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f) * DEFAULT_OBJECT_POSITION;
                    tempObject = Instantiate(Resources.Load<SkillObject>("Prefabs/skills/" + code + "_object"),
                        instanceTransform, true);
                    tempObject.transform.position = this.transform.position + DEFAULT_OBJECT_POSITION_Y +
                                                    attackDirection * Random.Range(0f, DEFAULT_SKILL_RANGE);
                    tempObject.Init(damage, 0f, attackDirection);
                    audioSource.Play();
                    StartCoroutine(removeSkillObject(tempObject));
                    
                    yield return new WaitForSeconds(duration / 6);
                }

                break;
            
            default:
                Debug.Log("Invalid skill type: " + skillType);
                break;
        }
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

    public void ApplyEnhanceOption(string stat, float value)
    {
        switch (stat)
        {
            case "SkillDelay":
                this.delay -= value * this.delay;
                break;
            
            case "Damage":
                this.damage += (int)(value * this.damage);
                break;
            
            default:
                break;
        }
    }
}
