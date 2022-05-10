using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    PlayerController player;

    // Start is called before the first frame update
    void Start()
    {
        player = PlayerController.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(player.transform.position);

        //Vector3 position = -(player.transform.position - RoomManager.Instance.GetCurrentRoom().transform.position);

        //position.y = 30;

        transform.position = player.transform.position + new Vector3(0, 45, -10);
    }
}
