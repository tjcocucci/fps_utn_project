using UnityEngine;

public class FPSController : MonoBehaviour
{
    public float speed = 12.0f;
    public float sensitivity = 5.0f;
    public Transform playerBody;
    public CharacterController controller;
    private float rotationY = 0.0f;
    private float rotationX = 0.0f;
    private Vector3 velocity;

    private float gravity = -9.81f;
    private float jumpForce = 3.0f;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the game window
    }

    void Update()
    {
        Debug.Log(controller.isGrounded);
        Look();
        Move();
        Fall();
        Jump();
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

        // Rotate the camera vertically
        Camera.main.transform.localRotation = Quaternion.Euler(rotationY, 0.0f, 0.0f);
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
                Debug.Log("Jump");
                // Apply jump force
                velocity.y = Mathf.Sqrt(jumpForce * -2.0f * gravity);
            }
        }
    }

    void Fall()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
