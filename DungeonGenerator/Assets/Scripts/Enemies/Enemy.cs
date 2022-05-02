using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected float health;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected Material tookDamageMaterial;

    protected PlayerController player;
    protected Rigidbody body;
    protected Room room;
    protected Material startingMaterial;

    protected EnemyTypes type;

    protected bool knockbackApplied;
    protected bool checkRemoveKnockback;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        body = GetComponent<Rigidbody>();
        player = PlayerController.Instance;

        type = EnemyTypes.Enemy;

        startingMaterial = GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (knockbackApplied && body.velocity.magnitude < 1f && checkRemoveKnockback)
        {
            knockbackApplied = false;

            checkRemoveKnockback = false;
        }

        if (health <= 0)
        {
            Destroy(gameObject);

            room.SetRoomCompleted(true);
        }
    }

    protected virtual void Attack()
    {

    }

    public void ApplyKnockbackFromOriginPoint(float knockbackForce, Vector3 knockbackOrigin)
    {
        Vector3 directionOfKnockback = (transform.position - knockbackOrigin).normalized;

        directionOfKnockback.y = 0;

        body.AddForce(directionOfKnockback * knockbackForce, ForceMode.Impulse);

        knockbackApplied = true;

        StartCoroutine(RemoveKnockbackDelay());
    }

    public void ApplyKnockbackInDirection(float knockbackForce, Vector3 direction)
    {
        direction.y = 0;

        body.AddForce(direction * knockbackForce, ForceMode.Impulse);

        knockbackApplied = true;

        StartCoroutine(RemoveKnockbackDelay());
    }

    IEnumerator RemoveKnockbackDelay()
    {
        checkRemoveKnockback = false;

        yield return new WaitForSeconds(0.2f);

        checkRemoveKnockback = true;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        StartCoroutine(FlashRed());
    }

    IEnumerator FlashRed()
    {
        GetComponent<MeshRenderer>().material = tookDamageMaterial;

        yield return new WaitForSeconds(0.15f);

        GetComponent<MeshRenderer>().material = startingMaterial;
    }

    public void SetRoom(Room r)
    {
        room = r;
    }
}
