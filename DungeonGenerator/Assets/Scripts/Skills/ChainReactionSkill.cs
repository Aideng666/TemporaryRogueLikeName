using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainReactionSkill : Skill
{
    //[SerializeField] GameObject chainShotPrefab;
    //[SerializeField] int defaultTotalRicochets = 3;

    //[SerializeField] GameObject projectile;

    int totalRicochets = 3;

    //GameObject chainShot;
    GameObject previousEnemy;

    List<GameObject> enemiesAlreadyHit = new List<GameObject>();
    
    protected override void Start()
    {
        base.Start();

        //totalRicochets = defaultTotalRicochets;
        projectile = Instantiate(skillInfo.skillPrefab, player.transform.position, Quaternion.identity);
        projectile.SetActive(false);
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void UseSkill()
    {

        if (RoomManager.Instance.GetCurrentRoom().GetEnemiesInRoom().Count < 1)
        {
            timeToNextSkillUse = Time.time + skillInfo.cooldown;

            return;
        }

        projectile.SetActive(true);
        projectile.transform.position = player.transform.position;

        if (RoomManager.Instance.GetCurrentRoom().GetEnemiesInRoom().Count == 1)
        {
            totalRicochets = RoomManager.Instance.GetCurrentRoom().GetEnemiesInRoom().Count;
        }
        else
        {
            //totalRicochets = defaultTotalRicochets;
            totalRicochets = 3;
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

            Vector3 startPos = projectile.transform.position;
            Vector3 endPos = targetEnemy.transform.position;

            while (timeElasped < totalTime)
            {
                float t = timeElasped / totalTime;

                projectile.transform.position = Vector3.Lerp(startPos, endPos, t);

                timeElasped += Time.deltaTime;

                yield return null;
            }

            targetEnemy.GetComponent<Enemy>().TakeDamage(skillInfo.damage);

            previousEnemy = targetEnemy;

            enemiesAlreadyHit.Add(targetEnemy);

            yield return null;
        }

        enemiesAlreadyHit = new List<GameObject>();
        projectile.SetActive(false);

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
        Vector3 startPos = projectile.transform.position;

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
