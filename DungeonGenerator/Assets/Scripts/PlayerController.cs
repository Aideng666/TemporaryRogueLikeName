using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //Inspector Variables
    [SerializeField] bool controlWithMouse;
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
    [SerializeField] GameObject[] skillOrbs = new GameObject[4];
    [SerializeField] float[] lightAttackComboDamage = new float[3];
    [SerializeField] float[] heavyAttackComboDamage = new float[2];
    [SerializeField] float lightAttackCooldown;
    [SerializeField] float heavyAttackCooldown;
    bool isAttacking;
    bool activateCollider;
    bool canAttack = true;
    PlayerAttackStates attackState = PlayerAttackStates.Idle;
    PlayerAttackStates previousAttackState = PlayerAttackStates.Idle;
    Skill[] equippedSkills = new Skill[4];
    List<Item> itemList = new List<Item>();

    //health
    [SerializeField] int maxhealth = 6;
    int currentHealth;
    bool iFramesActive;

    int invincibleLayer;
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

        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;

        currentHealth = maxhealth;

        invincibleLayer = LayerMask.NameToLayer("Invincible");
        playerLayer = LayerMask.NameToLayer("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth > maxhealth)
        {
            currentHealth = maxhealth;
        }

        for (int i = 0; i < skillOrbs.Length; i++)
        {
            if (equippedSkills[i] == null || !equippedSkills[i].GetCooldownComplete())
            {
                skillOrbs[i].GetComponent<MeshRenderer>().enabled = false;
            }
            else if (equippedSkills[i].GetCooldownComplete())
            {
                skillOrbs[i].GetComponent<MeshRenderer>().enabled = true;
            }
        }

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

                UseSkills();

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
            Vector2 aimDirection = InputManager.Instance.Aim();

            Vector3 mousePos;
            Vector3 mouseAimDirection = Vector3.zero;

            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100))
            {
                mousePos = hit.point;

                mouseAimDirection = (mousePos - transform.position).normalized;

                mouseAimDirection.y = 0;
            }

            float targetAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;/* + Camera.main.transform.eulerAngles.y;*/
            moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            float aimAngle = Mathf.Atan2(aimDirection.x, aimDirection.y) * Mathf.Rad2Deg;/* + Camera.main.transform.eulerAngles.y;*/
            float mouseAimAngle = Mathf.Atan2(mouseAimDirection.x, mouseAimDirection.z) * Mathf.Rad2Deg;


            if (controlWithMouse)
            {
                transform.rotation = Quaternion.Euler(0, mouseAimAngle, 0);
            }
            else if (aimDirection.magnitude <= 0.1f)
            {
                transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, aimAngle, 0);
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
        gameObject.layer = invincibleLayer;

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
                        enemy.GetComponent<Enemy>().ApplyKnockbackInDirection(25, (attackPoint.position - transform.position).normalized);

                        enemy.GetComponent<Enemy>().TakeDamage(lightAttackComboDamage[0]);
                    }

                    break;

                case PlayerAttackStates.Light2:

                    enemiesHit = Physics.OverlapBox(attackPoint.position, new Vector3(5, 0.5f, 5), Quaternion.identity, enemyLayer);

                    foreach (Collider enemy in enemiesHit)
                    {
                        enemy.GetComponent<Enemy>().ApplyKnockbackInDirection(25, (attackPoint.position - transform.position).normalized);

                        enemy.GetComponent<Enemy>().TakeDamage(lightAttackComboDamage[1]);
                    }

                    break;

                case PlayerAttackStates.Light3:

                    enemiesHit = Physics.OverlapBox(transform.position, new Vector3(6, 0.5f, 6), Quaternion.identity, enemyLayer);

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

    void UseSkills()
    {    
        if (equippedSkills[0] != null)
        {
            if (InputManager.Instance.Skill1() && equippedSkills[0].CanUseSkill())
            {
                equippedSkills[0].UseSkill();
            }
        }

        if (equippedSkills[1] != null)
        {
            if (InputManager.Instance.Skill2() && equippedSkills[1].CanUseSkill())
            {
                equippedSkills[1].UseSkill();
            }
        }

        if (equippedSkills[2] != null)
        {
            if (InputManager.Instance.Skill3() && equippedSkills[2].CanUseSkill())
            {
                equippedSkills[2].UseSkill();
            }
        }

        if (equippedSkills[3] != null)
        {
            if (InputManager.Instance.Skill4() && equippedSkills[3].CanUseSkill())
            {
                equippedSkills[3].UseSkill();
            }
        }
    }

    public void EquipSkill(SkillInfo newSkill)
    {
        for (int i = 0; i < equippedSkills.Length; i++)
        {
            if (equippedSkills[i] == null)
            {
                equippedSkills[i] = newSkill.skill.GetComponent<Skill>();

                skillOrbs[i].GetComponent<MeshRenderer>().enabled = true;

                skillOrbs[i].GetComponent<MeshRenderer>().material.color = newSkill.orbColor;

                return;
            }
        }

        //Write the code to swap between skills here (for when the player already has the max skills and wants a new one)
    }

    public void AddItem(Item item)
    {
        itemList.Add(item);

        if (item.GetItemType() == ItemTypes.Stat)
        {
            item.ItemEffect();
        }
    }

    IEnumerator ActivateSecondSpinAttack()
    {
        yield return new WaitForSeconds(0.2f);

        Collider[] enemiesHit = Physics.OverlapBox(transform.position, new Vector3(6, 0.5f, 6), Quaternion.identity, enemyLayer);

        foreach (Collider enemy in enemiesHit)
        {
            //enemy.GetComponent<Enemy>().ApplyKnockbackInDirection(40, (attackPoint.position - transform.position).normalized);
            enemy.GetComponent<Enemy>().ApplyKnockbackFromOriginPoint(40, transform.position);

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
        if (!iFramesActive)
        {
            currentHealth--;

            StartCoroutine(ActivateIFrames());
        }
    }

    public void Heal(int amount)
    {
        if (amount > 0)
        {
            currentHealth += amount;
        }
    }

    IEnumerator ActivateIFrames()
    {
        iFramesActive = true;

        gameObject.layer = invincibleLayer;

        yield return new WaitForSeconds(1);

        gameObject.layer = playerLayer;

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

    public void ModifyMaxHealth(int value)
    {
        maxhealth += value;
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

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.TryGetComponent<SkillPedestal>(out var pedestal))
        {
            pedestal.GiveSkill();
        }

        if (hit.gameObject.TryGetComponent<ItemPickup>(out var pickup))
        {
            pickup.GiveItem();
        }
    }
}
