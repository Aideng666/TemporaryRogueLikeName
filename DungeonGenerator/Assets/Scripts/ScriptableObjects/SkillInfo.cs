using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Skill", menuName = "Skill Information")]
public class SkillInfo : ScriptableObject
{
    public float damage;
    public float cooldown;

    public GameObject pedestalModel;
    public GameObject skillPrefab;

    //Add stuff for UI icons as well
}
