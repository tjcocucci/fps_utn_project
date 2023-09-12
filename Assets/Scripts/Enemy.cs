using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
    easy,
    medium,
    hard
}

[RequireComponent(typeof(WeaponController))]
public class Enemy : DamageableObject
{
    public Transform playerTransform;
    private float distanceToPlayer;
    public float distanceToPlayerThreshold = 5.0f;
    public float speed = 2;
    public float damage = 10;
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
    private Vector3 velocity;

    public SkinnedMeshRenderer surfaceRenderer;
    public SkinnedMeshRenderer jointsRenderer;

    // Start is called before the first frame update
    public override void Start()
    {
        movementEnabled = true;
        playerTransform = FindObjectOfType<Player>().transform;
        weaponController = GetComponent<WeaponController>();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        weaponController.EquipWeapon(weaponIndex);
        base.Start();
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

    void Update()
    {
        if (playerTransform != null)
        {
            distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            transform.LookAt(playerTransform);
            if (distanceToPlayer > distanceToPlayerThreshold && movementEnabled)
            {
                Move();
                animator.SetFloat("forwardSpeed", 1.5f);
            }
            else
            {
                animator.SetFloat("forwardSpeed", 0);
            }

            transform.LookAt(playerTransform);
            Shoot();
        }
        if (!characterController.isGrounded)
        {
            Fall();
        }
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

    public void Shoot()
    {
        weaponController.weapon.Shoot();
    }
}
