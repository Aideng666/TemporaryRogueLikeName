using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    //Change these to the scriptable object
    //[SerializeField] protected float damage = 5;
    //[SerializeField] protected float cooldown = 1;
    ////////////////////////////////////////

    [SerializeField] protected SkillInfo skillInfo;

    protected float timeToNextSkillUse = 0;

    protected PlayerController player;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        player = PlayerController.Instance;

        timeToNextSkillUse = 0;
    }

    // Update is called once per frame
    protected virtual void Update()
    {

    }

    public virtual void UseSkill()
    {

    }

    public bool CanUseSkill()
    {
        if (Time.time > timeToNextSkillUse)
        {
            timeToNextSkillUse = Time.time + skillInfo.cooldown;

            return true;
        }
        return false;
    }
}