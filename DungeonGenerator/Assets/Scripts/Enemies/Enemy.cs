using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected float health;
    [SerializeField] protected float moveSpeed;

    protected PlayerController player;
    protected Rigidbody body;

    protected bool knockbackApplied;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        body = GetComponent<Rigidbody>();
        player = PlayerController.Instance;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (knockbackApplied && body.velocity.magnitude < 1f)
        {
            knockbackApplied = false;
        }
    }

    protected virtual void Attack()
    {

    }

    public void ApplyKnockback(float knockbackForce, Vector3 knockbackOrigin)
    {
        Vector3 directionOfKnockback = (transform.position - knockbackOrigin).normalized;

        directionOfKnockback.y = 0;

        body.AddForce(directionOfKnockback * knockbackForce, ForceMode.Impulse);

        knockbackApplied = true;
    }
}
