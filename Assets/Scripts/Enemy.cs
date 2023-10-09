using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;

public enum EnemyType
{
    easy,
    medium,
    hard
}

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(WeaponController))]
public class Enemy : DamageableObject
{
    public Transform playerTransform;
    private float distanceToPlayer;
    public float distanceToPlayerThreshold = 5.0f;
    public float speed = 2;
    public WeaponController weaponController;
    public CharacterController characterController;
    private Animator animator;
    public int weaponIndex = 0;
    public EnemyType type;
    private bool movementEnabled;
    private int direction = 1;
    private bool disableScheduled = false;
    private bool directionSwitchScheduled = false;
    private float gravity = -9.81f;
    public float followDelay = 0.5f;
    private Vector3 velocity;
    public NavMeshAgent pathfinder;
    public SkinnedMeshRenderer surfaceRenderer;
    public SkinnedMeshRenderer jointsRenderer;
    public Dropable dropablePrefab;

    public enum State
    {
        Standing,
        Chasing,
        Searching,
        Idle
    };

    public LayerMask playerLayerMask;
    public State currentState;
    private bool hasTarget;
    private DamageableObject targetDamageable;

    // Start is called before the first frame update
    public override void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("enemyAim");
        if (player != null)
        {
            hasTarget = true;
            playerTransform = player.transform;
            currentState = State.Searching;
        }
        weaponController = GetComponent<WeaponController>();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        weaponController.EquipWeapon(weaponIndex);
        pathfinder = GetComponent<NavMeshAgent>();
        pathfinder.speed = speed;

        base.Start();

        StartCoroutine(FindPath());
    }

    void OnTargetDeath()
    {
        hasTarget = false;
        currentState = State.Standing;
    }

    public void SetType(EnemyType enemyType)
    {
        type = enemyType;
        switch (type)
        {
            case EnemyType.easy:
                surfaceRenderer.material.SetColor("_Color", Color.green);
                jointsRenderer.material.SetColor("_Color", Color.green * 0.5f);
                break;
            case EnemyType.medium:
                surfaceRenderer.material.SetColor("_Color", Color.yellow);
                jointsRenderer.material.SetColor("_Color", Color.yellow * 0.5f);
                break;
            case EnemyType.hard:
                surfaceRenderer.material.SetColor("_Color", Color.red);
                jointsRenderer.material.SetColor("_Color", Color.red * 0.5f);
                break;
        }
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        if (!IsAlive())
        {
            Drop();
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (playerTransform != null)
        {
            distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            SetState();
            Reload();
            switch (currentState)
            {
                case State.Standing:
                    transform.LookAt(playerTransform);
                    Aim();
                    Shoot();
                    animator.SetFloat("forwardSpeed", 0);
                    break;
                case State.Chasing:
                    transform.LookAt(playerTransform);
                    Aim();
                    Shoot();
                    animator.SetFloat("forwardSpeed", 1.5f);
                    break;
                case State.Searching:
                    break;
                case State.Idle:
                    break;
            }
        }
        if (!characterController.isGrounded)
        {
            Fall();
        }
    }

    void SetState()
    {

        if (!IsAlive()) {
            currentState = State.Idle;
            return;
        }

        bool targetInSight = false;
        if (hasTarget)
        {
            if (Physics.Raycast(
                transform.position,
                playerTransform.position - transform.position,
                out RaycastHit hit,
                distanceToPlayer * 2
            ))
            {
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    targetInSight = true;
                }
            }

            Debug.DrawLine(transform.position, playerTransform.position, Color.red);
        }

        if (!targetInSight)
        {
            currentState = State.Searching;
            return;
        } else if (distanceToPlayer > distanceToPlayerThreshold + 0.01f)
        {
            currentState = State.Chasing;
            return;
        } else
        {
            currentState = State.Standing;
        }
    }

    void Aim()
    {
        weaponController.weaponHoldTransform.LookAt(playerTransform);
    }

    void Fall()
    {
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    private void DisableMovement()
    {
        movementEnabled = false;
    }

    private void EnableMovement()
    {
        movementEnabled = true;
    }

    IEnumerator DelayDisableMovementAndReenable(float delay)
    {
        yield return new WaitForSeconds(delay);
        DisableMovement();
        yield return new WaitForSeconds(delay);
        EnableMovement();
        disableScheduled = false;
    }

    IEnumerator DelayedDirectionSwitch()
    {
        float delay = Random.Range(0.5f, 2.0f);
        yield return new WaitForSeconds(delay);
        direction = -direction;
        directionSwitchScheduled = false;
    }

    private void Move()
    {
        if (type == EnemyType.easy)
        {
            transform.position += transform.forward * speed * Time.deltaTime;
            if (!disableScheduled)
            {
                disableScheduled = true;
                StartCoroutine(DelayDisableMovementAndReenable(2));
            }
        }
        else if (type == EnemyType.medium)
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }
        else if (type == EnemyType.hard)
        {
            transform.position += transform.forward * speed * Time.deltaTime;
            transform.position += transform.right * direction * speed * Time.deltaTime;
            if (!directionSwitchScheduled)
            {
                directionSwitchScheduled = true;
                StartCoroutine(DelayedDirectionSwitch());
            }
        }
    }

    IEnumerator FindPath()
    {
        while (hasTarget)
        {
            if (IsAlive())
            {
                if (currentState == State.Searching || currentState == State.Chasing)
                {
                    Vector3 directionToTarget = (
                        playerTransform.position - transform.position
                    ).normalized;
                    Vector3 targetPosition =
                        playerTransform.position - directionToTarget * distanceToPlayerThreshold;

                    Debug.DrawLine(transform.position, targetPosition, Color.red);
                    pathfinder.SetDestination(targetPosition);
                }
            }
            yield return new WaitForSeconds(followDelay);
        }
    }

    public void Shoot()
    {
        weaponController.weapon.Shoot();
    }

    void Reload ()
    { 
        if (weaponController.weapon.ammo <= 0)
        {
            weaponController.weapon.Reload(weaponController.weapon.magazineSize);
        }

    }

    void Drop ()
    {
        Dropable dropable = Instantiate(dropablePrefab, transform.position + Vector3.up, Quaternion.identity);
        dropable.SetRandomType();
    }

}
