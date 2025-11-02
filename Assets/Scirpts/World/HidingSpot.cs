using UnityEngine;

public class HidingSpot : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float checkInterval = 0.3f; // Kaç saniyede bir kontrol edilecek
    [SerializeField] private float destroyRadius = 3f; // Gizlenme alanından bu kadar uzaklıktaki enemy'leri de yok et
    private float lastCheckTime = 0f;
    private Collider2D triggerCollider;

    private void Awake()
    {
        // Trigger collider'ı bul (BoxCollider2D veya CircleCollider2D olabilir)
        triggerCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        // Belirli aralıklarla gizlenme alanı içindeki ölü enemy'leri kontrol et
        if (Time.time >= lastCheckTime + checkInterval)
        {
            CheckForDeadEnemiesInArea();
            lastCheckTime = Time.time;
        }
    }

    private void CheckForDeadEnemiesInArea()
    {
        if (triggerCollider == null) return;

        // ÖNEMLİ: Ölü enemy'ler rb.simulated = false olduğu için fizik sistemi onları algılamıyor
        // Bu yüzden FindObjectsOfType kullanarak tüm enemy'leri bulup pozisyon kontrolü yapıyoruz
        Enemy[] allEnemies = FindObjectsOfType<Enemy>();
        Vector3 hidingSpotCenter = triggerCollider.bounds.center;
        
        // Gizlenme alanının size'ını al
        float areaSize = 0f;
        if (triggerCollider is CircleCollider2D circleCollider)
        {
            areaSize = circleCollider.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);
        }
        else if (triggerCollider is BoxCollider2D boxCollider)
        {
            areaSize = Mathf.Max(boxCollider.size.x, boxCollider.size.y) * 0.5f * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);
        }
        else
        {
            // Fallback: bounds size kullan
            Bounds bounds = triggerCollider.bounds;
            areaSize = Mathf.Max(bounds.size.x, bounds.size.y) * 0.5f;
        }
        
        // Kontrol mesafesi: gizlenme alanı + ekstra radius
        float checkDistance = areaSize + destroyRadius;

        foreach (Enemy enemy in allEnemies)
        {
            if (enemy == null || !enemy.IsDead()) continue;
            
            // Enemy pozisyonunu gizlenme alanı merkezinden mesafesini hesapla
            float distance = Vector3.Distance(enemy.transform.position, hidingSpotCenter);
            
            // Eğer gizlenme alanına yakınsa (içinde veya önünde) yok et
            if (distance <= checkDistance)
            {
                // Ölü enemy gizlenme alanına yakın, yok et
                Destroy(enemy.gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Player geldiğinde
        if (other.CompareTag(playerTag))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.SetInHidingSpot(true, this);
            }
        }

        // Ölü enemy geldiğinde (taşınan enemy veya yerdeki ölü enemy)
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null && enemy.IsDead())
        {
            // Player taşıyor mu kontrol et
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null && player.IsCarrying() && player.GetCarriedEnemy() == enemy)
            {
                // Enemy'yi yok et
                Destroy(enemy.gameObject);
            }
            else
            {
                // Ölü enemy gizlilik alanına temas etti, yok et
                Destroy(enemy.gameObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.SetInHidingSpot(false, null);
                // Gizlilik alanından çıkınca otomatik olarak gizlenmeyi kaldır
                if (player.IsHidden())
                {
                    player.ExitHiding();
                }
            }
        }
    }
}

