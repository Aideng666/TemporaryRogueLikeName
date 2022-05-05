using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    [SerializeField] protected float damage;
    [SerializeField] protected float cooldown;

    float timeToNextSkillUse = 0;

    protected PlayerController player;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        player = PlayerController.Instance;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (InputManager.Instance.Skill1() && CanUseSkill())
        {
            UseSkill();
        }
    }

    protected virtual void UseSkill()
    {

    }

    protected bool CanUseSkill()
    {
        if (Time.time > timeToNextSkillUse)
        {
            timeToNextSkillUse = Time.time + cooldown;

            return true;
        }

        return false;
    }
}
