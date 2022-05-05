using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitOnDeathEnemy : SimpleFollowMeleeEnemy
{
    [SerializeField] GameObject splitChildEnemy;
    [SerializeField] int numberOfEnemiesSpawned = 4;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void Attack()
    {
        base.Attack();
    }

    protected override void Die()
    {
        for (int i = 0; i < numberOfEnemiesSpawned; i++)
        {
            var enemy = Instantiate(splitChildEnemy, transform.position, Quaternion.identity);

            enemy.GetComponent<Enemy>().SetRoom(room);
        }

        base.Die();
    }
}

