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
    private bool isFromControlledEnemy = false; // Player tarafından kontrol edilen enemy'den mi geldiği

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    public void Initialize(Vector2 dir, float spd, int dmg, int type, bool fromControlledEnemy = false)
    {
        direction = dir.normalized;
        speed = spd;
        damage = dmg;
        enemyType = type;
        isFromControlledEnemy = fromControlledEnemy;

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
            // Debug: Projectile enemy'ye çarptı
            Debug.Log($"Projectile enemy'ye çarptı! Enemy Type: {enemy.GetEnemyType()}, Projectile Type: {enemyType}");

            int targetEnemyType = enemy.GetEnemyType();

            // Hasar verme kuralları:
            // 1. Eğer projectile player tarafından kontrol edilen enemy'den geliyorsa -> Her enemy'ye hasar ver
            // 2. Eğer farklı tipte enemy'lerse -> Hasar ver
            // 3. Eğer aynı tipte enemy'ye çarpmışsa -> Hasar verme

            bool canDamage = false;

            // Önce kontrol edilen enemy kontrolü
            if (enemy.IsControlled())
            {
                // Kontrol edilen enemy'ye hasar ver (kendi projectile'ı hariç)
                if (!isFromControlledEnemy || targetEnemyType != enemyType)
                {
                    canDamage = true;
                }
            }
            // Player kontrolündeki enemy'den geliyorsa
            else if (isFromControlledEnemy)
            {
                // Player kontrolündeki enemy'den geldi, her enemy'ye hasar verebilir
                canDamage = true;
            }
            // Normal enemy'ler arası çatışma - EN ÖNEMLİ KISIM
            else if (!isFromControlledEnemy && !enemy.IsControlled())
            {
                // Farklı tipte enemy'lerse hasar verebilir
                if (targetEnemyType != enemyType)
                {
                    canDamage = true;
                    // Debug için
                    Debug.Log($"Enemy {enemyType} -> Enemy {targetEnemyType} hasar veriyor! Damage: {damage}");
                }
                else
                {
                    // Aynı tipte enemy'ye çarpmış
                    Debug.Log($"Aynı tipte enemy'ye çarptı: {enemyType} == {targetEnemyType}");
                }
            }

            if (canDamage)
            {
                enemy.TakeDamage(damage);
                Destroy(gameObject);
                return;
            }
            else
            {
                // Aynı tipte enemy'ye çarpmış veya hasar verilemez, projectile'ı yok et
                Destroy(gameObject);
                return;
            }
        }
    }

    private bool IsInLayerMask(int layer, LayerMask mask)
    {
        return mask == (mask | (1 << layer));
    }
}

