using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody2D rb;
    private Collider2D col;

    [Header("Settings")]
    private Vector2 direction;
    private float speed;
    private int damage;
    private int enemyType; // Hangi enemy tipinden geldiği

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    public void Initialize(Vector2 dir, float spd, int dmg, int type)
    {
        direction = dir.normalized;
        speed = spd;
        damage = dmg;
        enemyType = type;

        // Velocity'yi ayarla
        if (rb != null)
        {
            rb.linearVelocity = direction * speed;
        }

        // Yönü ayarla
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Ground'a değerse yok ol
        if (IsInLayerMask(collision.gameObject.layer, groundLayer))
        {
            Destroy(gameObject);
            return;
        }

        // Player'a değerse hasar ver
        PlayerController player = collision.GetComponent<PlayerController>();
        if (player != null && !player.IsDead())
        {
            player.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // Enemy'ye değerse kontrol et
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null && !enemy.IsDead())
        {
            // Aynı türden enemy'lere hasar verme
            if (enemy.GetEnemyType() == enemyType)
            {
                // Aynı türden enemy, hasar verme
                return;
            }
            
            enemy.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }
    }

    private bool IsInLayerMask(int layer, LayerMask mask)
    {
        return mask == (mask | (1 << layer));
    }
}

