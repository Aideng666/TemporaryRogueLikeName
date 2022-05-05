using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "SpawnInformation", menuName = "Spawn Information")]
public class FightRoomSpawnInformation : ScriptableObject
{
    public List<Vector3> enemyPositions;
    public List<EnemyTypes> enemyTypes;

    public List<GameObject> listOfUsedEnemies = new List<GameObject>();

    //Also add Obstacle Types and positions
}
