using UnityEngine;

public class Player : DamageableObject
{
    public float speed = 12.0f;
    public float sensitivity = 5.0f;
    public Transform playerBody;
    public CharacterController controller;
    private Animator animator;
    private float rotationY = 0.0f;
    private float rotationX = 0.0f;
    private Vector3 velocity;
    private Vector3 moveCurrent;
    public WeaponController weaponController;
    public ThrowableController throwableController;
    public PlayerInventory inventory;

    private float gravity = -9.81f;
    private float jumpForce = 3.0f;
    private float shootDistance = 100.0f;
    private bool won;
    public bool isAlive = true;
    public int weaponIndex;
    public Crosshairs crosshairsPrefab;
    Crosshairs crosshairs;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        weaponController = GetComponent<WeaponController>();
        animator = GetComponent<Animator>();
    }

    public override void Start()
    {
        base.Start();
        won = false;
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the game window
        weaponController.EquipWeapon(0);
        crosshairs = Instantiate(
            crosshairsPrefab,
            transform.position,
            crosshairsPrefab.transform.rotation,
            transform
        );

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnWin += OnWin;
        }
        else
        {
            Debug.Log("LevelManager is null");
        }
        OnObjectDied += OnPlayerDeath;

    }

    void OnPlayerDeath()
    {
        isAlive = false;
        Debug.Log("You died!");
    }

    void Update()
    {
        if (isAlive && !won)
        {
            Look();
            Move();
            ChangeWeapon();
            Reload();
            Fall();
            Jump();
            ThrowGranade();
            Aim();
        }
    }

    public void OnWin()
    {
        won = true;
        Debug.Log("You won!");
    }

    public void resetPlayer()
    {
        isAlive = true;
        won = false;
        health = totalHealth;
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        weaponController.EquipWeapon(0);
    }

    void Look()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        // Rotate the player's body (left and right)
        rotationX += mouseX;
        playerBody.Rotate(Vector3.up * mouseX);

        // Calculate vertical rotation
        rotationY -= mouseY;
        rotationY = Mathf.Clamp(rotationY, -60.0f, 60.0f); // Clamp vertical rotation to avoid flipping

        // Rotate the camera and weapon vertically
        Camera.main.transform.localRotation = Quaternion.Euler(rotationY, 0.0f, 0.0f);
    }

    void Move()
    {
        Vector3 moveTarget = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        if (Input.GetKey(KeyCode.LeftShift))
        {
            moveTarget *= 2.0f;
        }

        animator.SetFloat("forwardSpeed", moveTarget.z);
        animator.SetFloat("strafeSpeed", moveTarget.x);

        moveTarget = transform.TransformDirection(moveTarget);
        controller.Move(moveTarget * Time.deltaTime * speed);
    }

    void Jump()
    {
        if (controller.isGrounded)
        {
            animator.SetBool("jump", false);
            if (Input.GetButtonDown("Jump"))
            {
                velocity.y = Mathf.Sqrt(jumpForce * -2.0f * gravity);
                animator.SetBool("jump", true);
            }
        }
    }

    void Fall()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void Aim()
    {
        // Cast Ray from gunMuzzzle to mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Ray ray2 = new Ray(
            weaponController.weapon.gunMuzzleTransform.position,
            weaponController.weapon.gunMuzzleTransform.forward
        );
        Debug.DrawRay(ray.origin, ray.direction * shootDistance, Color.red);
        weaponController.weaponHoldTransform.LookAt(ray.GetPoint(shootDistance));

        crosshairs.transform.position = ray.GetPoint(10);
        crosshairs.transform.LookAt(ray.GetPoint(20));
        Shoot(ray);
    }

    void Shoot(Ray ray)
    {
        if (Input.GetMouseButton(0))
        {
            weaponController.weapon.Shoot();
        }
    }

    void ChangeWeapon()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && weaponIndex != 0)
        {
            weaponIndex = 0;
            weaponController.EquipWeapon(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && weaponIndex != 1)
        {
            weaponIndex = 1;
            weaponController.EquipWeapon(1);
        }
    }
    void ThrowGranade()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            int availableGranades = inventory.granadeCount;
            if (availableGranades > 0)
            {
                inventory.granadeCount--;
                throwableController.Throw();
            }
        }
    }

    void Reload()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            int inventoryAmmo = inventory.GetAvailableAmmo(weaponIndex);
            int remainigAmmo = weaponController.weapon.Reload(inventoryAmmo);
            inventory.SetAvailableAmmo(weaponIndex, remainigAmmo);
        }
    }
}
