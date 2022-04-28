using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public static AnimationManager Instance { get; set; }
    private void Awake()
    {
        Instance = this;
    }

    public void OpenDoor(Animator anim)
    {
        anim.SetBool("DoorOpen", true);
    }

    public void CloseDoor(Animator anim)
    {
        anim.SetBool("DoorOpen", false);
    }

    public void PlayerAttackCombo(Animator anim, int comboNum)
    {
        switch (comboNum)
        {

            case 0:

                anim.SetBool("Attack1", false);
                anim.SetBool("Attack2", false);
                anim.SetBool("Attack3", false);

                break;

            case 1:

                anim.SetBool("Attack1", true);
                anim.SetBool("Attack2", false);
                anim.SetBool("Attack3", false);

                break;

            case 2:

                anim.SetBool("Attack1", false);
                anim.SetBool("Attack2", true);
                anim.SetBool("Attack3", false);

                break;

            case 3:

                anim.SetBool("Attack1", false);
                anim.SetBool("Attack2", false);
                anim.SetBool("Attack3", true);

                break;
        }

    }
}
