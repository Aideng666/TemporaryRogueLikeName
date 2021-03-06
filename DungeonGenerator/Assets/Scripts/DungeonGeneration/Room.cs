using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] protected GameObject northSouthSolidWallPrefab;
    [SerializeField] protected GameObject eastWestSolidWallPrefab;
    [SerializeField] protected GameObject northSouthDoorWallPrefab;
    [SerializeField] protected GameObject eastWestDoorWallPrefab;
    [SerializeField] protected GameObject enemyPrefab;

    [Header("Materials")]
    [SerializeField] protected Material endRoomMaterial;
    [SerializeField] protected Material startRoomMaterial;
    [SerializeField] protected Material shopRoomMaterial;
    [SerializeField] protected Material itemRoomMaterial;

    [Header("Room Environment")]
    [SerializeField] protected GameObject northWall;
    [SerializeField] protected GameObject eastWall;
    [SerializeField] protected GameObject southWall;
    [SerializeField] protected GameObject westWall;
    [SerializeField] protected GameObject ground;
    [SerializeField] protected int sideLength = 100;

    [Header("Minimap Objects")]
    [SerializeField] protected GameObject minimapGround;

    [Header("Fight Room Variables")]
    [SerializeField] protected List<FightRoomSpawnInformation> possibleSpawnInformations = new List<FightRoomSpawnInformation>();
    [SerializeField] protected List<GameObject> possibleBosses = new List<GameObject>();

    [Header("Item Room Variables")]
    [SerializeField] protected GameObject skillPedestalPrefab;
    [SerializeField] protected GameObject itemPickupPrefab;

    protected List<GameObject> currentEnemiesInRoom = new List<GameObject>();
    protected List<GameObject> currentSkillPedestalsInRoom = new List<GameObject>();

    protected PlayerController player;

    protected Room originRoom;

    protected DirectionsEnum directionOfOrigin;
    protected RoomTypes roomType = RoomTypes.Default;

    protected int distanceFromStart;
    protected int roomNum;
    protected bool discovered;
    protected bool roomCompleted;
    protected bool doorsOpen;
    protected bool isCurrentRoom;

    //protected FightRoomSpawnInformation spawnInfo;

    protected virtual void Start()
    {
        player = PlayerController.Instance;

        minimapGround.SetActive(false);
    }

    protected virtual void Update()
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


                if (Vector3.Distance(player.transform.position, transform.position) <= sideLength / 2)
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

    public List<GameObject> GetEnemiesInRoom()
    {
        return currentEnemiesInRoom;
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

    public RoomTypes GetRoomType()
    {
        return roomType;
    }

    public void SetDiscovered(bool disc)
    {
        discovered = disc;
    }

    public void SetRoomType(RoomTypes type)
    {
        roomType = type;

        switch (type)
        {
            case RoomTypes.Start:

                distanceFromStart = 0;

                roomCompleted = true;

                discovered = true;

                ground.GetComponent<MeshRenderer>().material = startRoomMaterial;

                minimapGround.GetComponent<MeshRenderer>().material = startRoomMaterial;

                break;

            case RoomTypes.Boss:

                ground.GetComponent<MeshRenderer>().material = endRoomMaterial;

                minimapGround.GetComponent<MeshRenderer>().material = endRoomMaterial;

                SpawnBoss();

                break;

            case RoomTypes.Shop:

                roomCompleted = true;

                ground.GetComponent<MeshRenderer>().material = shopRoomMaterial;

                minimapGround.GetComponent<MeshRenderer>().material = shopRoomMaterial;

                SpawnItem();

                break;

            case RoomTypes.Item:

                roomCompleted = true;

                ground.GetComponent<MeshRenderer>().material = itemRoomMaterial;

                minimapGround.GetComponent<MeshRenderer>().material = itemRoomMaterial;

                SpawnSkillPedestals();

                break;

            case RoomTypes.Fight:

                ChooseRandomEnemySpawnGroup();

                break;
        }
    }

    protected void ChooseRandomEnemySpawnGroup()
    {
        int randomIndex = UnityEngine.Random.Range(0, possibleSpawnInformations.Count);

        List<Vector3> enemyPositions = possibleSpawnInformations[randomIndex].enemyPositions;
        List<EnemyTypes> enemyTypes = possibleSpawnInformations[randomIndex].enemyTypes;

        List<GameObject> enemyPrefabs = possibleSpawnInformations[randomIndex].listOfUsedEnemies;

        for (int i = 0; i < enemyTypes.Count; i++)
        {
            for (int j = 0; j < enemyPrefabs.Count; j++)
            {
                if (enemyPrefabs[j].GetComponent<Enemy>().GetEnemyType() == enemyTypes[i])
                {
                    var enemy = Instantiate(enemyPrefabs[j], transform.position + new Vector3(enemyPositions[i].x, enemyPrefabs[j].transform.position.y, enemyPositions[i].z), Quaternion.identity);

                    enemy.GetComponent<Enemy>().SetRoom(this);
                }
            }
        }
    }

    protected void SpawnSkillPedestals()
    {
        var pedestal = Instantiate(skillPedestalPrefab, transform.position + new Vector3(-25, skillPedestalPrefab.transform.position.y, 0), Quaternion.identity);

        currentSkillPedestalsInRoom.Add(pedestal);

        pedestal = Instantiate(skillPedestalPrefab, transform.position + new Vector3(25, skillPedestalPrefab.transform.position.y, 0), Quaternion.identity);

        currentSkillPedestalsInRoom.Add(pedestal);

        currentSkillPedestalsInRoom[0].GetComponent<SkillPedestal>().SetPartner(currentSkillPedestalsInRoom[1].GetComponent<SkillPedestal>());
        currentSkillPedestalsInRoom[1].GetComponent<SkillPedestal>().SetPartner(currentSkillPedestalsInRoom[0].GetComponent<SkillPedestal>());
    }

    protected void SpawnItem()
    {
        var item = Instantiate(itemPickupPrefab, transform.position, Quaternion.identity);
    }

    protected void SpawnBoss()
    {
        int randomIndex = UnityEngine.Random.Range(0, possibleBosses.Count);

        var boss = Instantiate(possibleBosses[randomIndex], new Vector3(transform.position.x, possibleBosses[randomIndex].transform.position.y, transform.position.z), Quaternion.identity);

        boss.GetComponent<BossEnemy>().SetRoom(this);
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
            SetRoomType(RoomTypes.Start);
        }
    }

    public void SetRoomNum(int num)
    {
        roomNum = num;
    }

    //public void SetEndRoom()
    //{
    //    SetRoomType(RoomTypes.Boss);

        //ground.GetComponent<MeshRenderer>().material = endRoomMaterial;

        //minimapGround.GetComponent<MeshRenderer>().material = endRoomMaterial;

        //switch (directionOfOrigin)
        //{
        //    case DirectionsEnum.North:

        //        Generator.Instance.SpawnBossRoom(transform.position + (Vector3.back * (sideLength / 2)), originRoom, this, directionOfOrigin, DirectionsEnum.South);

        //        break;

        //    case DirectionsEnum.East:

        //        Generator.Instance.SpawnBossRoom(transform.position + (Vector3.left * (sideLength / 2)), originRoom, this, directionOfOrigin, DirectionsEnum.West);

        //        break;

        //    case DirectionsEnum.South:

        //        Generator.Instance.SpawnBossRoom(transform.position + (Vector3.forward * (sideLength / 2)), originRoom, this, directionOfOrigin, DirectionsEnum.North);

        //        break;

        //    case DirectionsEnum.West:

        //        Generator.Instance.SpawnBossRoom(transform.position + (Vector3.right * (sideLength / 2)), originRoom, this, directionOfOrigin, DirectionsEnum.East);

        //        break;
        //}
    //}

    public GameObject ChangeWall(DirectionsEnum direction, WallTypes type)
    {
        switch (direction)
        {
            case DirectionsEnum.North:

                Destroy(northWall.gameObject);

                if (type == WallTypes.Solid)
                {
                    northWall = Instantiate(northSouthSolidWallPrefab, transform.position + new Vector3(0, northSouthSolidWallPrefab.transform.position.y, sideLength / 2), Quaternion.identity, transform);
                }
                else
                {
                    northWall = Instantiate(northSouthDoorWallPrefab, transform.position + new Vector3(0, northSouthSolidWallPrefab.transform.position.y, sideLength / 2), Quaternion.identity, transform);
                }

                return northWall;

            case DirectionsEnum.East:

                Destroy(eastWall.gameObject);

                if (type == WallTypes.Solid)
                {
                    eastWall = Instantiate(eastWestSolidWallPrefab, transform.position + new Vector3(sideLength / 2, northSouthSolidWallPrefab.transform.position.y, 0), Quaternion.Euler(0, 90, 0), transform);
                }
                else
                {
                    eastWall = Instantiate(eastWestDoorWallPrefab, transform.position + new Vector3(sideLength / 2, northSouthSolidWallPrefab.transform.position.y, 0), Quaternion.Euler(0, 90, 0), transform);
                }

                return eastWall;

            case DirectionsEnum.South:

                Destroy(southWall.gameObject);

                if (type == WallTypes.Solid)
                {
                    southWall = Instantiate(northSouthSolidWallPrefab, transform.position + new Vector3(0, northSouthSolidWallPrefab.transform.position.y, -sideLength / 2), Quaternion.identity, transform);
                }
                else
                {
                    southWall = Instantiate(northSouthDoorWallPrefab, transform.position + new Vector3(0, northSouthSolidWallPrefab.transform.position.y, -sideLength / 2), Quaternion.identity, transform);
                }

                return southWall;

            case DirectionsEnum.West:

                Destroy(westWall.gameObject);

                if (type == WallTypes.Solid)
                {
                    westWall = Instantiate(eastWestSolidWallPrefab, transform.position + new Vector3(-sideLength / 2, northSouthSolidWallPrefab.transform.position.y, 0), Quaternion.Euler(0, 90, 0), transform);
                }
                else
                {
                    westWall = Instantiate(eastWestDoorWallPrefab, transform.position + new Vector3(-sideLength / 2, northSouthSolidWallPrefab.transform.position.y, 0), Quaternion.Euler(0, 90, 0), transform);
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
