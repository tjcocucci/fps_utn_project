using UnityEngine;

public class Player : DamageableObject
{
    public float speed = 12.0f;
    public float sensitivity = 5.0f;
    public Transform playerBody;
    public CharacterController controller;
    private float rotationY = 0.0f;
    private float rotationX = 0.0f;
    private Vector3 velocity;
    public WeaponController weaponController;

    private float gravity = -9.81f;
    private float jumpForce = 3.0f;
    private float shootDistance = 100.0f;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        weaponController = GetComponent<WeaponController>();
    }

    override public void Start()
    {
        base.Start();
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the game window
        weaponController.EquipWeapon(0);
    }

    void Update()
    {
        Look();
        Move();
        Fall();
        Jump();
        Aim();
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
        weaponController.weapon.transform.localRotation = Quaternion.Euler(rotationY, 0.0f, 0.0f);
    }

    void Move()
    {
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        controller.Move(move * Time.deltaTime * speed);
    }

    void Jump()
    {
        if (controller.isGrounded)
        {
            if (Input.GetButtonDown("Jump"))
            {
                velocity.y = Mathf.Sqrt(jumpForce * -2.0f * gravity);
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
        Ray ray2 = new Ray(weaponController.weapon.gunMuzzleTransform.position, weaponController.weapon.gunMuzzleTransform.forward);
        Debug.DrawRay(ray.origin, ray.direction * shootDistance, Color.red);
        Debug.DrawRay(ray2.origin, ray2.direction * shootDistance, Color.red);
        Shoot(ray);
    }

    void Shoot(Ray ray)
    {
        if (Input.GetMouseButtonDown(0))
        {
            weaponController.weapon.Shoot();
        }
    }
}
