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
}
