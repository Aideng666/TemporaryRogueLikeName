using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainReactionSkill : Skill
{
    [SerializeField] GameObject chainShotPrefab;
    [SerializeField] int defaultTotalRicochets = 3;

    int totalRicochets;

    GameObject chainShot;
    GameObject previousEnemy;

    List<GameObject> enemiesAlreadyHit = new List<GameObject>();
    
    protected override void Start()
    {
        base.Start();

        totalRicochets = defaultTotalRicochets;
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void UseSkill()
    {
        chainShot = Instantiate(chainShotPrefab, player.transform.position, Quaternion.identity);

        if (RoomManager.Instance.GetCurrentRoom().GetEnemiesInRoom().Count == 1)
        {
            totalRicochets = RoomManager.Instance.GetCurrentRoom().GetEnemiesInRoom().Count;
        }
        else
        {
            totalRicochets = defaultTotalRicochets;
        }

        StartCoroutine(MoveToNextTarget());
    }

    IEnumerator MoveToNextTarget()
    {
        previousEnemy = null;

        for (int i = 0; i < totalRicochets; i++)
        {
            GameObject targetEnemy = FindClosestEnemy();

            float timeElasped = 0;
            float totalTime = 0.1f;

            Vector3 startPos = chainShot.transform.position;
            Vector3 endPos = targetEnemy.transform.position;

            while (timeElasped < totalTime)
            {
                float t = timeElasped / totalTime;

                chainShot.transform.position = Vector3.Lerp(startPos, endPos, t);

                timeElasped += Time.deltaTime;

                yield return null;
            }

            targetEnemy.GetComponent<Enemy>().TakeDamage(damage);

            previousEnemy = targetEnemy;

            enemiesAlreadyHit.Add(targetEnemy);

            yield return null;
        }

        enemiesAlreadyHit = new List<GameObject>();
        Destroy(chainShot);

        yield return null;
    }

    GameObject FindClosestEnemy()
    {
        List<GameObject> possibleEnemies = new List<GameObject>();

        foreach (GameObject enemy in RoomManager.Instance.GetCurrentRoom().GetEnemiesInRoom())
        {
            if (previousEnemy != enemy)
            {
                possibleEnemies.Add(enemy);
            }
        }

        GameObject currentClosestEnemy = possibleEnemies[0];
        Vector3 startPos = chainShot.transform.position;

        foreach (GameObject enemy in possibleEnemies)
        {
            if (Vector3.Distance(startPos, enemy.transform.position) < Vector3.Distance(startPos, currentClosestEnemy.transform.position) && previousEnemy != enemy)
            {
                currentClosestEnemy = enemy;
            }
        }

        return currentClosestEnemy;
    }
}
