using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Inspector Variables
    [SerializeField] Transform cam;
    [SerializeField] Transform camLookAt;
    [SerializeField] Transform bodyToRotate;
    [SerializeField] float speed;
    [SerializeField] float dashTimespan;
    [SerializeField] float dashCooldown = 2;

    //Character Controller
    CharacterController controller;

    //Movement Variables
    TrailRenderer dashTrail;
    float timeToNextDash = 0;
    bool isDashing;
    Vector3 dashDirection;
    Vector3 moveDir;

    //Attack Variables
    int numberOfClicks = 0;
    float lastClickTime = 0;
    float nextClickTimeLimit = 1;
    bool isAttacking;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        dashTrail = GetComponentInChildren<TrailRenderer>();

        dashTrail.emitting = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //InputManager.Instance.controls.Player.Dash.performed += Dash;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDashing)
        {
            if (!isAttacking)
            {
                Move();
            }

            Attack();

            if (/*(Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.Space))*/InputManager.Instance.Dash() && CanDash())
            {
                StartCoroutine(Dash());
            }
        }
    }

    void Move()
    {
        //float horizontalInput = Input.GetAxisRaw("Horizontal");
        //float verticalInput = Input.GetAxisRaw("Vertical");

        //Vector3 direction = new Vector3(horizontalInput, 0f, verticalInput).normalized;
        Vector2 direction = InputManager.Instance.Move();


        float targetAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
        moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

        if (direction.magnitude > 0)
        {
            transform.rotation = Quaternion.Euler(0, targetAngle, 0);
        }

        camLookAt.rotation = cam.rotation;

        if (moveDir != Vector3.zero)
        {
            dashDirection = moveDir;
        }

        if (direction.magnitude <= 0.1f)
        {
            moveDir = new Vector3(0, 0, 0);
        }

        controller.Move(moveDir * speed * Time.deltaTime);
    }

    IEnumerator Dash()
    {
        numberOfClicks = 0;

        isAttacking = false;

        AnimationManager.Instance.PlayerAttackCombo(GetComponent<Animator>(), 0);

        isDashing = true;

        //GetComponent<MeshRenderer>().enabled = false;
        foreach(MeshRenderer mesh in GetComponentsInChildren<MeshRenderer>())
        {
            mesh.enabled = false;
        }


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

        //GetComponent<MeshRenderer>().enabled = true;
        foreach (MeshRenderer mesh in GetComponentsInChildren<MeshRenderer>())
        {
            mesh.enabled = true;
        }

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

        if (!isDashing)
        {
            if (/*Input.GetMouseButtonDown(0)*/InputManager.Instance.LightAttack())
            {
                isAttacking = true;

                numberOfClicks++;

                AnimationManager.Instance.PlayerAttackCombo(GetComponent<Animator>(), numberOfClicks);

                if (numberOfClicks > 3)
                {
                    numberOfClicks = 0;

                    AnimationManager.Instance.PlayerAttackCombo(GetComponent<Animator>(), 0);
                }

                lastClickTime = Time.time;
            }
        }
    }
}
