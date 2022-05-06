using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Inspector Variables
    [SerializeField] Transform cam;
    [SerializeField] Transform camLookAt;
    [SerializeField] Transform bodyToRotate;
    [SerializeField] Transform attackPoint;
    [SerializeField] LayerMask enemyLayer;

    //Character Controller
    CharacterController controller;

    //For Knockback
    Rigidbody body;

    //Movement Variables
    [SerializeField] float defaultSpeed;
    [SerializeField] float speedWhileAttacking;
    [SerializeField] float dashTimespan;
    [SerializeField] float dashCooldown = 2;
    TrailRenderer dashTrail;
    float speed;
    float timeToNextDash = 0;
    bool isDashing;
    bool knockbackApplied;
    Vector3 dashDirection;
    Vector3 moveDir;

    //Attack Variables
    int numberOfClicks = 0;
    float lastClickTime = 0;
    float nextClickTimeLimit = 1f;
    [SerializeField] float[] lightAttackComboDamage = new float[3];
    [SerializeField] float[] heavyAttackComboDamage = new float[2];
    [SerializeField] float lightAttackCooldown;
    [SerializeField] float heavyAttackCooldown;
    bool isAttacking;
    bool activateCollider;
    bool canAttack = true;
    PlayerAttackStates attackState = PlayerAttackStates.Idle;
    PlayerAttackStates previousAttackState = PlayerAttackStates.Idle;


    //health
    [SerializeField] int maxhealth = 6;
    int currentHealth;
    bool iFramesActive;

    int dashLayer;
    int playerLayer;

    public static PlayerController Instance { get; set; }

    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        dashTrail = GetComponentInChildren<TrailRenderer>();

        dashTrail.emitting = false;

        speed = defaultSpeed;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentHealth = maxhealth;

        dashLayer = LayerMask.NameToLayer("Dash");
        playerLayer = LayerMask.NameToLayer("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            attackState = PlayerAttackStates.Idle;
            speed = defaultSpeed;
        }
        else if(GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("1st Attack"))
        {
            attackState = PlayerAttackStates.Light1;
            speed = speedWhileAttacking;
        }
        else if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("2nd Attack"))
        {
            attackState = PlayerAttackStates.Light2;
            speed = speedWhileAttacking;
        }
        else if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("3rd Attack"))
        {
            attackState = PlayerAttackStates.Light3;
            speed = speedWhileAttacking;
        }

        if (attackState != previousAttackState)
        {
            activateCollider = true;
            isAttacking = false;
        }

        if (!isDashing)
        {
            if (!knockbackApplied)
            {
                Move();

                Attack();

                if (InputManager.Instance.Dash() && CanDash())
                {
                    StartCoroutine(Dash());
                }
            }
        }

        previousAttackState = attackState;
    }

    void Move()
    {
        if (!isAttacking || GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("3rd Attack"))
        {
            Vector2 direction = InputManager.Instance.Move();

            float targetAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
            moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            if (direction.magnitude > 0)
            {
                transform.rotation = Quaternion.Euler(0, targetAngle, 0);
            }

            if (moveDir != Vector3.zero)
            {
                dashDirection = moveDir;
            }

            if (direction.magnitude <= 0.1f)
            {
                moveDir = new Vector3(0, 0, 0);
            }

            controller.Move(moveDir * speed * Time.deltaTime);

            transform.position = new Vector3(transform.position.x, 1, transform.position.z);
        }

        camLookAt.rotation = cam.rotation;
    }

    IEnumerator Dash()
    {
        numberOfClicks = 0;

        isAttacking = false;

        AnimationManager.Instance.PlayerAttackCombo(GetComponent<Animator>(), 0);

        isDashing = true;

        foreach(MeshRenderer mesh in GetComponentsInChildren<MeshRenderer>())
        {
            mesh.enabled = false;
        }

        //controller.detectCollisions = false;
        gameObject.layer = dashLayer;

        ParticleManager.Instance.SpawnParticle(ParticleTypes.DashStart, transform.position);

        dashTrail.emitting = true;

        float timeElasped = 0;

        while (timeElasped < dashTimespan)
        {
            controller.Move(dashDirection * defaultSpeed * 5 * Time.deltaTime);

            timeElasped += Time.deltaTime;

            yield return null;
        }

        dashTrail.emitting = false;

        foreach (MeshRenderer mesh in GetComponentsInChildren<MeshRenderer>())
        {
            mesh.enabled = true;
        }

        //controller.detectCollisions = true;
        gameObject.layer = playerLayer;

        isDashing = false;

        ParticleManager.Instance.SpawnParticle(ParticleTypes.DashEnd, transform.position);

        yield return null;
    }

    bool CanDash()
    {
        if (timeToNextDash <= Time.time)
        {
            timeToNextDash = Time.time + dashCooldown;

            return true;
        }

        return false;
    }

    void Attack()
    {
        if (Time.time <= lastClickTime + 0.3f)
        {
            return;
        }

        if (Time.time > lastClickTime + nextClickTimeLimit)
        {
            numberOfClicks = 0;

            isAttacking = false;

            AnimationManager.Instance.PlayerAttackCombo(GetComponent<Animator>(), 0);
        }

        if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime > 0.4f 
            && GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime < 0.9f
            && attackState != PlayerAttackStates.Idle)
        {
            isAttacking = true;
        }

        if ((GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.4f 
            || GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)
            && attackState != PlayerAttackStates.Idle)
        {
            isAttacking = false;
        }

        if (!isDashing)
        {
            if (InputManager.Instance.LightAttack() && canAttack)
            {
                numberOfClicks++;

                if(attackState == PlayerAttackStates.Idle && numberOfClicks > 1)
                {
                    numberOfClicks = 1;
                }

                if (attackState == PlayerAttackStates.Light1 && numberOfClicks > 2)
                {
                    numberOfClicks = 2;
                }

                if (numberOfClicks > 3)
                {
                    numberOfClicks = 3;
                }

                AnimationManager.Instance.PlayerAttackCombo(GetComponent<Animator>(), numberOfClicks);

                if (numberOfClicks == 3 && attackState == PlayerAttackStates.Light3 && GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f)
                {
                    numberOfClicks = 0;

                    AnimationManager.Instance.PlayerAttackCombo(GetComponent<Animator>(), 0);

                    StartCoroutine(LightAttackCooldown());
                }

                lastClickTime = Time.time;
            }
        }

        if (/*isAttacking && */activateCollider && GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime > 0.2f)
        {
            Collider[] enemiesHit;

            switch (attackState)
            {
                case PlayerAttackStates.Light1:

                    enemiesHit = Physics.OverlapBox(attackPoint.position, new Vector3(5, 0.5f, 5), Quaternion.identity, enemyLayer);

                    foreach (Collider enemy in enemiesHit)
                    {
                        enemy.GetComponent<Enemy>().ApplyKnockbackInDirection(35, (attackPoint.position - transform.position).normalized);

                        enemy.GetComponent<Enemy>().TakeDamage(lightAttackComboDamage[0]);
                    }

                    break;

                case PlayerAttackStates.Light2:

                    enemiesHit = Physics.OverlapBox(attackPoint.position, new Vector3(5, 0.5f, 5), Quaternion.identity, enemyLayer);

                    foreach (Collider enemy in enemiesHit)
                    {
                        enemy.GetComponent<Enemy>().ApplyKnockbackInDirection(35, (attackPoint.position - transform.position).normalized);

                        enemy.GetComponent<Enemy>().TakeDamage(lightAttackComboDamage[1]);
                    }

                    break;

                case PlayerAttackStates.Light3:

                    enemiesHit = Physics.OverlapBox(transform.position, new Vector3(5, 0.5f, 5), Quaternion.identity, enemyLayer);

                    foreach (Collider enemy in enemiesHit)
                    {
                        enemy.GetComponent<Enemy>().ApplyKnockbackInDirection(20, (attackPoint.position - transform.position).normalized);

                        enemy.GetComponent<Enemy>().TakeDamage(lightAttackComboDamage[2]);
                    }

                    StartCoroutine(ActivateSecondSpinAttack());

                    break;
            }

            activateCollider = false;
        }
    }

    IEnumerator ActivateSecondSpinAttack()
    {
        yield return new WaitForSeconds(0.2f);

        Collider[] enemiesHit = Physics.OverlapBox(transform.position, new Vector3(4, 0.5f, 4), Quaternion.identity, enemyLayer);

        foreach (Collider enemy in enemiesHit)
        {
            enemy.GetComponent<Enemy>().ApplyKnockbackInDirection(40, (attackPoint.position - transform.position).normalized);

            enemy.GetComponent<Enemy>().TakeDamage(lightAttackComboDamage[2]);
        }
    }

    IEnumerator LightAttackCooldown()
    {
        canAttack = false;

        yield return new WaitForSeconds(lightAttackCooldown);

        canAttack = true;
    }

    IEnumerator HeavyAttackCooldown()
    {
        canAttack = false;

        yield return new WaitForSeconds(heavyAttackCooldown);

        canAttack = true;
    }

    public void ApplyKnockback(float knockbackForce, Vector3 knockbackOrigin)
    {
        controller.enabled = false;

        if (body == null)
        {
            body = gameObject.AddComponent<Rigidbody>();
        }

        body.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY;

        body.mass = 1;
        body.drag = 5;

        Vector3 directionOfKnockback = (transform.position - knockbackOrigin).normalized;

        directionOfKnockback.y = 0;

        body.AddForce(directionOfKnockback * knockbackForce, ForceMode.Impulse);

        knockbackApplied = true;

        StartCoroutine(RemoveKnockback());
    }

    IEnumerator RemoveKnockback()
    {
        yield return new WaitForSeconds(0.2f);

        while (knockbackApplied)
        {
            if (body.velocity.magnitude < 2f)
            {
                Destroy(gameObject.GetComponent<Rigidbody>());

                body = null;

                controller.enabled = true;

                knockbackApplied = false;
            }

            yield return null;
        }

        yield return null;
    }

    public void TakeDamage()
    {
        currentHealth--;

        StartCoroutine(ActivateIFrames());
    }

    IEnumerator ActivateIFrames()
    {
        iFramesActive = true;

        yield return new WaitForSeconds(1);

        iFramesActive = false;
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public int GetMaxHealth()
    {
        return maxhealth;
    }

    public bool GetIFramesActive()
    {
        return iFramesActive;
    }

    public float[] GetLightAttackDamages()
    {
        return lightAttackComboDamage;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(attackPoint.position, new Vector3(10, 1, 10));
        Gizmos.DrawWireCube(transform.position, new Vector3(10, 1, 10));

    }
}
