using Unity.VisualScripting;
using UnityEngine;

public class BrainMovement : MonoBehaviour
{
public float moveSpeed = 5f;
    public Rigidbody2D rb;
    private Vector2 moveInput;

    private GameObject targetEnemy; // Enemy you're colliding with

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // --- Movement ---
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput = moveInput.normalized;

        // --- Possession / Become enemy ---
        if (targetEnemy != null && Input.GetKeyDown(KeyCode.X))
        {
            BecomeEnemy(targetEnemy);
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("GreenSolider"))
        {
            targetEnemy = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("GreenSolider"))
        {
            if (targetEnemy == other.gameObject)
                targetEnemy = null;
        }
    }

    

    private void BecomeEnemy(GameObject enemy)
    {
        // Move player
        transform.position = enemy.transform.position;

        // Disable enemy collider immediately
        Collider2D enemyCol = enemy.GetComponent<Collider2D>();
        if (enemyCol != null)
            enemyCol.enabled = false;

        // Copy sprite etc.
        SpriteRenderer enemyRenderer = enemy.GetComponent<SpriteRenderer>();
        SpriteRenderer playerRenderer = GetComponent<SpriteRenderer>();
        if (enemyRenderer != null && playerRenderer != null)
        {
            playerRenderer.sprite = enemyRenderer.sprite;
            playerRenderer.color = enemyRenderer.color;
        }

        GetComponent<CapsuleCollider2D>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = true;

        // Destroy enemy after short delay to let physics settle
        Destroy(enemy, 0.05f);

        Debug.Log("Possessed enemy without getting stuck!");
    }
}

