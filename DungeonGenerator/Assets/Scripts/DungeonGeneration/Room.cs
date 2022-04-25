using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] GameObject northSouthSolidWallPrefab;
    [SerializeField] GameObject eastWestSolidWallPrefab;
    [SerializeField] GameObject northSouthDoorWallPrefab;
    [SerializeField] GameObject eastWestDoorWallPrefab;

    [Header("Materials")]
    [SerializeField] Material endRoomMaterial;
    [SerializeField] Material startRoomMaterial;

    [Header("Room Environment")]
    [SerializeField] GameObject northWall;
    [SerializeField] GameObject eastWall;
    [SerializeField] GameObject southWall;
    [SerializeField] GameObject westWall;
    [SerializeField] GameObject ground;
    [SerializeField] int sideLength = 100;

    [Header("Minimap Objects")]
    [SerializeField] GameObject minimapGround;

    PlayerController player;

    Room originRoom;

    DirectionsEnum directionOfOrigin;
    RoomTypes roomType = RoomTypes.Fight;

    int distanceFromStart;
    int roomNum;
    bool discovered;
    bool roomCompleted;
    bool doorsOpen;
    bool isCurrentRoom;

    private void Start()
    {
        player = FindObjectOfType<PlayerController>();

        minimapGround.SetActive(false);
    }

    private void Update()
    {
        if (discovered)
        {
            minimapGround.SetActive(true);
        }

        if (isCurrentRoom)
        {
            if (!roomCompleted)
            {
                CloseDoors();


                if (Vector3.Distance(player.transform.position, transform.position) <= 50)
                {
                    discovered = true;
                }
            }
            else
            {
                OpenDoors();
            }
        }
    }

    public DirectionsEnum GetDirectionOfOrigin()
    {
        return directionOfOrigin;
    }

    public Room GetOrigin()
    {
        return originRoom;
    }

    public int GetDistance()
    {
        return distanceFromStart;
    }

    public int GetRoomNum()
    {
        return roomNum;
    }

    public bool GetDiscovered()
    {
        return discovered;
    }

    public bool GetRoomCompleted()
    {
        return roomCompleted;
    }

    public bool GetIsCurrentRoom()
    {
        return isCurrentRoom;
    }

    public void SetIsCurrentRoom(bool isCurrent)
    {
        isCurrentRoom = isCurrent;
    }

    public void SetRoomCompleted(bool complete)
    {
        roomCompleted = complete;
    }

    public int GetSideLength()
    {
        return sideLength;
    }

    public void SetDiscovered(bool disc)
    {
        discovered = disc;
    }

    public void SetRoomType(RoomTypes type)
    {
        roomType = type;
    }

    public void SetOrigin(Room origin, DirectionsEnum direction)
    {
        originRoom = origin;

        if (origin != null)
        {
            directionOfOrigin = direction;

            distanceFromStart = originRoom.GetDistance() + 1;

            ChangeWall(direction, WallTypes.Door);
        }
        else
        {
            distanceFromStart = 0;

            SetRoomType(RoomTypes.Start);

            roomCompleted = true;

            ground.GetComponent<MeshRenderer>().material = startRoomMaterial;

            minimapGround.GetComponent<MeshRenderer>().material = startRoomMaterial;
        }
    }

    public void SetRoomNum(int num)
    {
        roomNum = num;
    }

    public void SetEndRoom()
    {
        SetRoomType(RoomTypes.Boss);

        ground.GetComponent<MeshRenderer>().material = endRoomMaterial;

        minimapGround.GetComponent<MeshRenderer>().material = endRoomMaterial;
    }

    public GameObject ChangeWall(DirectionsEnum direction, WallTypes type)
    {
        switch (direction)
        {
            case DirectionsEnum.North:

                Destroy(northWall.gameObject);

                if (type == WallTypes.Solid)
                {
                    northWall = Instantiate(northSouthSolidWallPrefab, transform.position + new Vector3(0, northSouthSolidWallPrefab.transform.position.y, 50), Quaternion.identity, transform);
                }
                else
                {
                    northWall = Instantiate(northSouthDoorWallPrefab, transform.position + new Vector3(0, northSouthSolidWallPrefab.transform.position.y, 50), Quaternion.identity, transform);
                }

                return northWall;

            case DirectionsEnum.East:

                Destroy(eastWall.gameObject);

                if (type == WallTypes.Solid)
                {
                    eastWall = Instantiate(eastWestSolidWallPrefab, transform.position + new Vector3(50, northSouthSolidWallPrefab.transform.position.y, 0), Quaternion.Euler(0, 90, 0), transform);
                }
                else
                {
                    eastWall = Instantiate(eastWestDoorWallPrefab, transform.position + new Vector3(50, northSouthSolidWallPrefab.transform.position.y, 0), Quaternion.Euler(0, 90, 0), transform);
                }

                return eastWall;

            case DirectionsEnum.South:

                Destroy(southWall.gameObject);

                if (type == WallTypes.Solid)
                {
                    southWall = Instantiate(northSouthSolidWallPrefab, transform.position + new Vector3(0, northSouthSolidWallPrefab.transform.position.y, -50), Quaternion.identity, transform);
                }
                else
                {
                    southWall = Instantiate(northSouthDoorWallPrefab, transform.position + new Vector3(0, northSouthSolidWallPrefab.transform.position.y, -50), Quaternion.identity, transform);
                }

                return southWall;

            case DirectionsEnum.West:

                Destroy(westWall.gameObject);

                if (type == WallTypes.Solid)
                {
                    westWall = Instantiate(eastWestSolidWallPrefab, transform.position + new Vector3(-50, northSouthSolidWallPrefab.transform.position.y, 0), Quaternion.Euler(0, 90, 0), transform);
                }
                else
                {
                    westWall = Instantiate(eastWestDoorWallPrefab, transform.position + new Vector3(-50, northSouthSolidWallPrefab.transform.position.y, 0), Quaternion.Euler(0, 90, 0), transform);
                }

                return westWall;
        }

        return null;
    }

    public void SetWall(DirectionsEnum wallDirection, GameObject wall)
    {
        switch (wallDirection)
        {
            case DirectionsEnum.North:

                northWall = wall;

                break;

            case DirectionsEnum.East:

                eastWall = wall;

                break;

            case DirectionsEnum.South:

                southWall = wall;

                break;

            case DirectionsEnum.West:

                westWall = wall;

                break;
        }
    }

    public void RemoveWall(DirectionsEnum wallDirection)
    {
        switch (wallDirection)
        {
            case DirectionsEnum.North:

                Destroy(northWall.gameObject);

                break;

            case DirectionsEnum.East:

                Destroy(eastWall.gameObject);

                break;

            case DirectionsEnum.South:

                Destroy(southWall.gameObject);

                break;

            case DirectionsEnum.West:

                Destroy(westWall.gameObject);

                break;
        }
    }

    public void OpenDoors()
    {
        if (northWall.GetComponentInChildren<Animator>() != null)
        {
            AnimationManager.Instance.OpenDoor(northWall.GetComponentInChildren<Animator>());
        }

        if (eastWall.GetComponentInChildren<Animator>() != null)
        {
            AnimationManager.Instance.OpenDoor(eastWall.GetComponentInChildren<Animator>());
        }

        if (southWall.GetComponentInChildren<Animator>() != null)
        {
            AnimationManager.Instance.OpenDoor(southWall.GetComponentInChildren<Animator>());
        }

        if (westWall.GetComponentInChildren<Animator>() != null)
        {
            AnimationManager.Instance.OpenDoor(westWall.GetComponentInChildren<Animator>());
        }

        doorsOpen = true;
    }

    public void CloseDoors()
    {
        if (northWall.GetComponentInChildren<Animator>() != null)
        {
            AnimationManager.Instance.CloseDoor(northWall.GetComponentInChildren<Animator>());
        }

        if (eastWall.GetComponentInChildren<Animator>() != null)
        {
            AnimationManager.Instance.CloseDoor(eastWall.GetComponentInChildren<Animator>());
        }

        if (southWall.GetComponentInChildren<Animator>() != null)
        {
            AnimationManager.Instance.CloseDoor(southWall.GetComponentInChildren<Animator>());
        }

        if (westWall.GetComponentInChildren<Animator>() != null)
        {
            AnimationManager.Instance.CloseDoor(westWall.GetComponentInChildren<Animator>());
        }

        doorsOpen = false;
    }
}
