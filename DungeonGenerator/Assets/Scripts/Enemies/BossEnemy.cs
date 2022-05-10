using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemy : Enemy
{
    [SerializeField] GameObject minionPrefab;
    [SerializeField] float groundSlamRange = 15;

    bool startDelay = true;
    int attackChoice;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        if (room.GetIsCurrentRoom())
        {
            if (startDelay)
            {
                startDelay = false;

                StartCoroutine(DelayBeforeAttacking());
            }
        }
    }

    protected override void Attack()
    {
        switch (attackChoice)
        {
            case 0: // Spawning Minion

                var minion = Instantiate(minionPrefab, transform.position + (Vector3.back * 10), Quaternion.identity);

                minion.GetComponent<Enemy>().SetRoom(RoomManager.Instance.GetCurrentRoom());

                startDelay = true;

                break;

            case 1: // Moving Ground Slam

                body.isKinematic = true;

                StartCoroutine(MovingSlam());

                break;

            case 2: // Ground Slam

                body.isKinematic = true;

                StartCoroutine(GroundSlam());

                break;
        }
    }

    IEnumerator MovingSlam()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = player.transform.position + (Vector3.up * 20);

        float timeElasped = 0;
        float totalTime = 1;

        while (timeElasped < totalTime)
        {
            transform.position = Vector3.Lerp(startPos, endPos, timeElasped / totalTime);

            timeElasped += Time.deltaTime;

            yield return null;
        }

        startPos = transform.position;
        endPos = transform.position + (Vector3.down * 20);

        timeElasped = 0;
        totalTime = 0.5f;

        while (timeElasped < totalTime)
        {
            transform.position = Vector3.Lerp(startPos, endPos, timeElasped / totalTime);

            timeElasped += Time.deltaTime;

            yield return null;
        }

        DealGroundSlamDamage();

        yield return null;
    }

    IEnumerator GroundSlam()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = transform.position + (Vector3.up * 10);

        float timeElasped = 0;
        float totalTime = 0.5f;

        while (timeElasped < totalTime)
        {
            transform.position = Vector3.Lerp(startPos, endPos, timeElasped / totalTime);

            timeElasped += Time.deltaTime;

            yield return null;
        }

        startPos = transform.position;
        endPos = transform.position + (Vector3.down * 10);

        timeElasped = 0;
        totalTime = 0.25f;

        while (timeElasped < totalTime)
        {
            transform.position = Vector3.Lerp(startPos, endPos, timeElasped / totalTime);

            timeElasped += Time.deltaTime;

            yield return null;
        }

        DealGroundSlamDamage();

        yield return null;
    }

    IEnumerator DelayBeforeAttacking()
    {
        yield return new WaitForSeconds(2);

        ChooseNewAttack();
    }

    protected override void Die()
    {
        base.Die();
    }

    void ChooseNewAttack()
    {
        int maxRandom = 2;
        int minRandom = 1;

        if (Vector3.Distance(player.transform.position, transform.position) <= groundSlamRange)
        {
            maxRandom = 3;
            minRandom = 1;
        }
        else 
        {
            maxRandom = 2;
            minRandom = 0;
        }

        attackChoice = Random.Range(minRandom, maxRandom); // The number of different attacks the boss has

        Attack();
    }

    void DealGroundSlamDamage()
    {
        if (Vector3.Distance(player.transform.position, transform.position) < groundSlamRange)
        {
            player.TakeDamage();

            player.ApplyKnockback(50, transform.position);
        }

        startDelay = true;
        body.isKinematic = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(new Vector3(transform.position.x, 5, transform.position.z), new Vector3(15, 10, 15f));
    }
}
