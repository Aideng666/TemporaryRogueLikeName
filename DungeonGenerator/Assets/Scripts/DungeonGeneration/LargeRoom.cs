using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LargeRoom : Room
{
    protected override void Start()
    {
        base.Start();

        SetRoomType(RoomTypes.Boss);

        ground.GetComponent<MeshRenderer>().material = endRoomMaterial;

        minimapGround.GetComponent<MeshRenderer>().material = endRoomMaterial;
    }

    protected override void Update()
    {
        base.Update();
    }
}
