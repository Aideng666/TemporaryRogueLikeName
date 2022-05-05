using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncerEnemy : Enemy
{
    [SerializeField] float bounceForce = 50;

    float timeToNextBounce = 0;
    float bounceCooldown = 3;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        if (!knockbackApplied && CanBounce() && room.GetIsCurrentRoom())
        {
            BounceTowardPlayer();
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
        }
    }

    protected void BounceTowardPlayer()
    {
        Vector3 direction = (player.transform.position - transform.position).normalized;

        body.AddForce((direction + (Vector3.up * 2)) * bounceForce, ForceMode.Impulse);
    }

    protected bool CanBounce()
    {
        if (Time.time > timeToNextBounce)
        {
            timeToNextBounce = Time.time + bounceCooldown;

            return true;
        }

        return false;
    }
}
