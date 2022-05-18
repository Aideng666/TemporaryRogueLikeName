using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{

    protected float timeToNextSkillUse = 0;

    protected PlayerController player;

    protected bool cooldownComplete = true;

    protected GameObject projectile;
    protected SkillInfo skillInfo;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        player = PlayerController.Instance;

        timeToNextSkillUse = 0;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (Time.time > timeToNextSkillUse)
        {
            cooldownComplete = true;
        }
        else
        {
            cooldownComplete = false;
        }
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

    public SkillInfo GetSkillInfo()
    {
        return skillInfo;
    }

    public bool GetCooldownComplete()
    {
        return cooldownComplete;
    }

    public void SetSkillInfo(SkillInfo info)
    {
        skillInfo = info;
    }
}
