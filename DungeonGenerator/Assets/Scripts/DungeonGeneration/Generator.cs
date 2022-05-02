using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    [SerializeField] int totalRooms = 15;
    [SerializeField] GameObject roomPrefab;
    [SerializeField] GameObject largeRoomPrefab;

    List<Room> rooms = new List<Room>();

    public static Generator Instance { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        InitDungeon();
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    Regenerate();
        //}
    }

    void InitDungeon()
    {
        SpawnRoom(Vector3.zero);

        for (int i = 0; i < totalRooms - 1; i++)
        {
            ChooseNewRoomLocation();
        }

        Room currentEndRoom = rooms[0];
        for (int i = 0; i < rooms.Count; i++)
        {
            if (rooms[i].GetDistance() > currentEndRoom.GetDistance())
            {
                currentEndRoom = rooms[i];
            }
        }

        //currentEndRoom.SetRoomType(RoomTypes.Boss);
        currentEndRoom.SetEndRoom();

        CreateShopRoom();

        CreateItemRoom();

        CreateFightRooms();
    }

    void Regenerate()
    {
        rooms = new List<Room>();

        foreach (Room room in FindObjectsOfType<Room>())
        {
            Destroy(room.gameObject);
        }

        InitDungeon();
    }

    void SpawnRoom(Vector3 position, Room origin = null, DirectionsEnum directionOfOrigin = DirectionsEnum.North, DirectionsEnum directionFromOrigin = DirectionsEnum.South)
    {
        var room = Instantiate(roomPrefab, position, Quaternion.identity);

        room.GetComponent<Room>().SetOrigin(origin, directionOfOrigin);
        room.GetComponent<Room>().SetRoomNum(rooms.Count + 1);

        rooms.Add(room.GetComponent<Room>());

        if (origin != null)
        {
            GameObject newWall = origin.ChangeWall(directionFromOrigin, WallTypes.Door);

            room.GetComponent<Room>().RemoveWall(directionOfOrigin);
            room.GetComponent<Room>().SetWall(directionOfOrigin, newWall);
        }
    }

    public void SpawnBossRoom(Vector3 position, Room origin, Room roomToReplace, DirectionsEnum directionOfOrigin, DirectionsEnum directionFromOrigin)
    {
        print("Spawning Boss Room");

        for (int i = 0; i < rooms.Count; i++)
        {
            if (rooms[i].transform.position == roomToReplace.transform.position)
            {
                rooms.RemoveAt(i);
            }
        }

        Destroy(roomToReplace.gameObject);

        var room = Instantiate(largeRoomPrefab, position, Quaternion.identity);

        room.GetComponent<LargeRoom>().SetOrigin(origin, directionOfOrigin);

        rooms.Add(room.GetComponent<LargeRoom>());

        GameObject newWall = room.GetComponent<LargeRoom>().ChangeWall(directionOfOrigin, WallTypes.Door);

        origin.RemoveWall(directionFromOrigin);
        origin.SetWall(directionFromOrigin, newWall);
    }

    void CreateShopRoom()
    {
        List<Room> changeableRooms = new List<Room>();

        for (int i = 0; i < rooms.Count; i++)
        {
            if (rooms[i].GetComponentsInChildren<Animator>().Length < 2 && rooms[i].GetRoomType() == RoomTypes.Default)
            {
                changeableRooms.Add(rooms[i]);
            }
        }

        int randomIndex = Random.Range(0, changeableRooms.Count);

        changeableRooms[randomIndex].SetRoomType(RoomTypes.Shop);
    }

    void CreateItemRoom()
    {
        List<Room> changeableRooms = new List<Room>();

        for (int i = 0; i < rooms.Count; i++)
        {
            if (rooms[i].GetComponentsInChildren<Animator>().Length < 2 && rooms[i].GetRoomType() == RoomTypes.Default)
            {
                changeableRooms.Add(rooms[i]);
            }
        }

        int randomIndex = Random.Range(0, changeableRooms.Count);

        changeableRooms[randomIndex].SetRoomType(RoomTypes.Item);
    }

    void CreateFightRooms()
    {
        foreach(Room room in rooms)
        {
            if (room.GetRoomType() == RoomTypes.Default)
            {
                room.SetRoomType(RoomTypes.Fight);
            }
        }
    }

    void ChooseNewRoomLocation()
    {
        int randomRoomChoice = Random.Range(0, rooms.Count - 1);

        List<DirectionsEnum> possibleDirections = new List<DirectionsEnum>();

        if (rooms[randomRoomChoice].GetOrigin() != null)
        {
            for (int j = 0; j < 4; j++)
            {
                bool addDirection = true;

                if ((int)rooms[randomRoomChoice].GetDirectionOfOrigin() != j)
                {
                    if (j == (int)DirectionsEnum.North)
                    {
                        for (int i = 0; i < rooms.Count; i++)
                        {
                            if (rooms[i].transform.position == rooms[randomRoomChoice].transform.position + new Vector3(0, 0, rooms[i].GetSideLength()) + new Vector3(rooms[i].GetSideLength(), 0, 0)
                                || rooms[i].transform.position == rooms[randomRoomChoice].transform.position + new Vector3(0, 0, rooms[i].GetSideLength()) + new Vector3(-rooms[i].GetSideLength(), 0, 0)
                                || rooms[i].transform.position == rooms[randomRoomChoice].transform.position + new Vector3(0, 0, rooms[i].GetSideLength()) + new Vector3(0, 0, rooms[i].GetSideLength()))
                            {
                                addDirection = false;
                            }
                        }
                    }
                    if (j == (int)DirectionsEnum.East)
                    {
                        for (int i = 0; i < rooms.Count; i++)
                        {
                            if (rooms[i].transform.position == rooms[randomRoomChoice].transform.position + new Vector3(rooms[i].GetSideLength(), 0, 0) + new Vector3(rooms[i].GetSideLength(), 0, 0)
                                || rooms[i].transform.position == rooms[randomRoomChoice].transform.position + new Vector3(rooms[i].GetSideLength(), 0, 0) + new Vector3(0, 0, -rooms[i].GetSideLength())
                                || rooms[i].transform.position == rooms[randomRoomChoice].transform.position + new Vector3(rooms[i].GetSideLength(), 0, 0) + new Vector3(0, 0, rooms[i].GetSideLength()))
                            {
                                addDirection = false;
                            }
                        }
                    }
                    if (j == (int)DirectionsEnum.South)
                    {
                        for (int i = 0; i < rooms.Count; i++)
                        {
                            if (rooms[i].transform.position == rooms[randomRoomChoice].transform.position + new Vector3(0, 0, -rooms[i].GetSideLength()) + new Vector3(rooms[i].GetSideLength(), 0, 0)
                                || rooms[i].transform.position == rooms[randomRoomChoice].transform.position + new Vector3(0, 0, -rooms[i].GetSideLength()) + new Vector3(-rooms[i].GetSideLength(), 0, 0)
                                || rooms[i].transform.position == rooms[randomRoomChoice].transform.position + new Vector3(0, 0, -rooms[i].GetSideLength()) + new Vector3(0, 0, -rooms[i].GetSideLength()))
                            {
                                addDirection = false;
                            }
                        }
                    }
                    if (j == (int)DirectionsEnum.West)
                    {
                        for (int i = 0; i < rooms.Count; i++)
                        {
                            if (rooms[i].transform.position == rooms[randomRoomChoice].transform.position + new Vector3(-rooms[i].GetSideLength(), 0, 0) + new Vector3(0, 0, -rooms[i].GetSideLength())
                                || rooms[i].transform.position == rooms[randomRoomChoice].transform.position + new Vector3(-rooms[i].GetSideLength(), 0, 0) + new Vector3(-rooms[i].GetSideLength(), 0, 0)
                                || rooms[i].transform.position == rooms[randomRoomChoice].transform.position + new Vector3(-rooms[i].GetSideLength(), 0, 0) + new Vector3(0, 0, rooms[i].GetSideLength()))
                            {
                                addDirection = false;
                            }
                        }
                    }
                }

                if (addDirection)
                {
                    possibleDirections.Add((DirectionsEnum)j);
                }
            }
        }
        else
        {
            for (int j = 0; j < 4; j++)
            {
                possibleDirections.Add((DirectionsEnum)j);
            }
        }

        if (possibleDirections.Count == 0)
        {
            ChooseNewRoomLocation();

            return;
        }

        int directionChoice = Random.Range(0, possibleDirections.Count);

        Vector3 roomPosition = Vector3.zero;

        switch (possibleDirections[directionChoice])
        {
            case DirectionsEnum.North:

                roomPosition = rooms[randomRoomChoice].transform.position + new Vector3(0, 0, rooms[randomRoomChoice].GetSideLength());

                break;

            case DirectionsEnum.East:

                roomPosition = rooms[randomRoomChoice].transform.position + new Vector3(rooms[randomRoomChoice].GetSideLength(), 0, 0);

                break;

            case DirectionsEnum.South:

                roomPosition = rooms[randomRoomChoice].transform.position + new Vector3(0, 0, -rooms[randomRoomChoice].GetSideLength());

                break;

            case DirectionsEnum.West:

                roomPosition = rooms[randomRoomChoice].transform.position + new Vector3(-rooms[randomRoomChoice].GetSideLength(), 0, 0);

                break;
        }

        int totalConnectedRooms = 0;

        for (int i = 0; i < rooms.Count; i++)
        {
            if (roomPosition == rooms[i].transform.position)
            {
                ChooseNewRoomLocation();

                return;
            }
        }
        for (int i = 0; i < rooms.Count; i++)
        {
            if (roomPosition + new Vector3(rooms[i].GetSideLength(), 0, 0) == rooms[i].transform.position)
            {
                totalConnectedRooms++;
            }
            if (roomPosition + new Vector3(-rooms[i].GetSideLength(), 0, 0) == rooms[i].transform.position)
            {
                totalConnectedRooms++;
            }
            if (roomPosition + new Vector3(0, 0, rooms[i].GetSideLength()) == rooms[i].transform.position)
            {
                totalConnectedRooms++;
            }
            if (roomPosition + new Vector3(0, 0, -rooms[i].GetSideLength()) == rooms[i].transform.position)
            {
                totalConnectedRooms++;
            }

            if (totalConnectedRooms > 1)
            {
                ChooseNewRoomLocation();

                return;
            }
        }

        DirectionsEnum directionOfOrigin = possibleDirections[directionChoice];

        switch (possibleDirections[directionChoice])
        {
            case DirectionsEnum.North:

                directionOfOrigin = DirectionsEnum.South;

                break;

            case DirectionsEnum.East:

                directionOfOrigin = DirectionsEnum.West;

                break;

            case DirectionsEnum.South:

                directionOfOrigin = DirectionsEnum.North;

                break;

            case DirectionsEnum.West:

                directionOfOrigin = DirectionsEnum.East;

                break;
        }

        SpawnRoom(roomPosition, rooms[randomRoomChoice], directionOfOrigin, possibleDirections[directionChoice]);
    }

    public List<Room> GetRooms()
    {
        return rooms;
    }

    public int GetTotalRooms()
    {
        return totalRooms;
    }
}
