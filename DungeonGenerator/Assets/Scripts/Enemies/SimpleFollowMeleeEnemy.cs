using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleFollowMeleeEnemy : Enemy
{
    bool canMove = true;

    protected override void Start()
    {
        base.Start();

        type = EnemyTypes.SimpleFollow;
    }

    protected override void Update()
    {
        base.Update();

        transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));

        if (!knockbackApplied && canMove && room.GetIsCurrentRoom())
        {
            FollowPlayer();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() != null)
        {
            if (!player.GetIFramesActive())
            {
                collision.gameObject.GetComponent<PlayerController>().ApplyKnockback(30, transform.position);

                player.TakeDamage();
            }

            StartCoroutine(PauseAfterAttack());
        }
    }

    protected override void Attack()
    {
        base.Attack();
    }

    void FollowPlayer()
    {
        Vector3 direction = (player.transform.position - transform.position).normalized;

        body.velocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;

        body.AddForce(direction * moveSpeed, ForceMode.Force);
    }

    IEnumerator PauseAfterAttack()
    {
        canMove = false;

        yield return new WaitForSeconds(0.5f);

        canMove = true;
    }
}
