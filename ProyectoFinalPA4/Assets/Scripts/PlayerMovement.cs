using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float speed = 5.0f;
    public float jumpHeight = 2.0f;
    public float gravity = -19.62f;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private Transform mainCamera;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        // Automatically find main camera if not assigned (kept for compatibility)
        if (Camera.main != null)
        {
            mainCamera = Camera.main.transform;
        }
    }

    void Update()
    {
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Simplified movement as requested
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 dir = new Vector3(h, 0, v).normalized;
        if (dir.magnitude >= 0.1f)
        {
            controller.Move(dir * speed * Time.deltaTime);
            transform.forward = Vector3.Slerp(transform.forward, dir, 12f * Time.deltaTime);
        }

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}