using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [SerializeField] Transform minimapCam;

    List<Room> roomList = new List<Room>();
    PlayerController player;

    Room currentRoom;

    public static RoomManager Instance { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (roomList.Count < Generator.Instance.GetTotalRooms())
        {
            roomList = Generator.Instance.GetRooms();
        }

        foreach (Room room in roomList)
        {
            if (Vector3.Distance(player.transform.position, room.transform.position) <= 50)
            {
                if (currentRoom != null)
                {
                    currentRoom.SetIsCurrentRoom(false);
                }

                currentRoom = room;

                currentRoom.SetIsCurrentRoom(true);
            }
        }

        minimapCam.transform.position = new Vector3(currentRoom.transform.position.x, minimapCam.transform.position.y, currentRoom.transform.position.z);

        minimapCam.rotation = Quaternion.Euler(90, Camera.main.transform.rotation.eulerAngles.y, 0);

        DiscoverAdjacentRooms();

        if (/*Input.GetKeyDown(KeyCode.Return)*/InputManager.Instance.controls.Player.CompleteRoomDevTool.triggered)
        {
            currentRoom.SetRoomCompleted(true);
        }

    }

    void DiscoverAdjacentRooms()
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            if (!roomList[i].GetDiscovered() && (roomList[i].transform.position == currentRoom.transform.position + (Vector3.right * currentRoom.GetSideLength())
                || roomList[i].transform.position == currentRoom.transform.position + (Vector3.left * currentRoom.GetSideLength())
                || roomList[i].transform.position == currentRoom.transform.position + (Vector3.back * currentRoom.GetSideLength())
                || roomList[i].transform.position == currentRoom.transform.position + (Vector3.forward * currentRoom.GetSideLength())))
            {
                roomList[i].SetDiscovered(true);
            }
        }
    }

    public Room GetCurrentRoom()
    {
        return currentRoom;
    }
}
