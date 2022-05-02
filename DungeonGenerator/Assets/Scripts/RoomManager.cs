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
        player = PlayerController.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        roomList = Generator.Instance.GetRooms();

        foreach (Room room in roomList)
        {
            if (Vector3.Distance(player.transform.position, room.transform.position) <= room.GetSideLength() / 2)
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

        if (InputManager.Instance.CompleteRoom())
        {
            player.TakeDamage();

            currentRoom.SetRoomCompleted(true);
        }

    }

    void DiscoverAdjacentRooms()
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            if (!roomList[i].GetDiscovered() 
                && (roomList[i].transform.position == currentRoom.transform.position + (Vector3.right * ((roomList[i].GetSideLength() / 2) + (currentRoom.GetSideLength() / 2)))
                || roomList[i].transform.position == currentRoom.transform.position + (Vector3.left * ((roomList[i].GetSideLength() / 2) + (currentRoom.GetSideLength() / 2)))
                || roomList[i].transform.position == currentRoom.transform.position + (Vector3.back * ((roomList[i].GetSideLength() / 2) + (currentRoom.GetSideLength() / 2)))
                || roomList[i].transform.position == currentRoom.transform.position + (Vector3.forward * ((roomList[i].GetSideLength() / 2) + (currentRoom.GetSideLength() / 2)))))
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
