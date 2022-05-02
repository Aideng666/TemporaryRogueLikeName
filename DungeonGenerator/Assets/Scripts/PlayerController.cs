using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Inspector Variables
    [SerializeField] Transform cam;
    [SerializeField] Transform camLookAt;
    [SerializeField] Transform bodyToRotate;

    //Character Controller
    CharacterController controller;

    //For Knockback
    Rigidbody body;

    //Movement Variables
    [SerializeField] float speed;
    [SerializeField] float dashTimespan;
    [SerializeField] float dashCooldown = 2;
    TrailRenderer dashTrail;
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
    bool canAttack = true;

    //health
    [SerializeField] int maxhealth = 6;
    int currentHealth;
    bool iFramesActive;

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

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentHealth = maxhealth;
    }

    // Update is called once per frame
    void Update()
    {
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
    }

    void Move()
    {
        if (!isAttacking)
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

        controller.detectCollisions = false;

        GetComponentInChildren<BoxCollider>().enabled = false;

        ParticleManager.Instance.SpawnParticle(ParticleTypes.DashStart, transform.position);

        dashTrail.emitting = true;

        float timeElasped = 0;

        while (timeElasped < dashTimespan)
        {
            controller.Move(dashDirection * speed * 5 * Time.deltaTime);

            timeElasped += Time.deltaTime;

            yield return null;
        }

        dashTrail.emitting = false;

        foreach (MeshRenderer mesh in GetComponentsInChildren<MeshRenderer>())
        {
            mesh.enabled = true;
        }

        controller.detectCollisions = true;

        GetComponentInChildren<BoxCollider>().enabled = true;

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
        if (Time.time > lastClickTime + nextClickTimeLimit)
        {
            numberOfClicks = 0;

            isAttacking = false;

            AnimationManager.Instance.PlayerAttackCombo(GetComponent<Animator>(), 0);
        }

        if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime > 0.4f 
            && GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime < 0.9f
            && !GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            isAttacking = true;
        }

        if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.4f 
            || GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f
            && !GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            isAttacking = false;
        }

        if (!isDashing)
        {
            if (InputManager.Instance.LightAttack() && canAttack)
            {
                numberOfClicks++;

                if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("1st Attack") && numberOfClicks > 2)
                {
                    numberOfClicks = 2;
                }

                if (numberOfClicks > 3)
                {
                    numberOfClicks = 3;
                }

                AnimationManager.Instance.PlayerAttackCombo(GetComponent<Animator>(), numberOfClicks);

                if (numberOfClicks == 3 && GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("3rd Attack") && GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f)
                {
                    StartCoroutine(LightAttackCooldown());

                    numberOfClicks = 0;
                }

                lastClickTime = Time.time;
            }
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
}
