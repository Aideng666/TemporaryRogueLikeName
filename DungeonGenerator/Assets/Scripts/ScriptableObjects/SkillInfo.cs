using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Skill", menuName = "Skill Information")]
public class SkillInfo : ScriptableObject
{
    public float damage;
    public float cooldown;

    public GameObject pedestalModel;
    public int skillNum;
    public Color orbColor;
    public GameObject skillPrefab;

    [HideInInspector]
    public GameObject skill;

    public void SetSkill()
    {
        skill = ItemPool.Instance.GetSkillFromPool(skillNum);

        skill.GetComponent<Skill>().SetSkillInfo(this);
    }
    //Add stuff for UI icons as well
}
