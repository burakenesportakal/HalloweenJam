using UnityEngine;

public class SoliderMovement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public float moveSpeed = 6f;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Get input from player (WASD or Arrow keys)
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        // Normalize movement so diagonal isnt faster
        moveInput = moveInput.normalized;
    }

    void FixedUpdate()
    {
        // Move using Rigidbody2D for smoother motion
        rb.linearVelocity = moveInput * moveSpeed;
    }
}
