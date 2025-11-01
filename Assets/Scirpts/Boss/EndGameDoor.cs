using UnityEngine;

namespace HalloweenJam.Boss
{
    /// <summary>
    /// Oyun sonu kapısı - Boss saldırıları kapıya hasar verir, kırılabilir
    /// </summary>
    public class EndGameDoor : MonoBehaviour
    {
        [Header("Door Settings")]
        [SerializeField] private int maxHealth = 100;
        [SerializeField] private int currentHealth;

        [Header("Visual Feedback")]
        [SerializeField] private SpriteRenderer doorSprite;
        [SerializeField] private GameObject destroyedEffect; // Kırılma efekti (kapı kırıldığında)

        private bool isDestroyed = false;

        private void Start()
        {
            currentHealth = maxHealth;
            UpdateHealthDisplay();
        }

        /// <summary>
        /// Kapıya hasar ver (boss saldırılarından)
        /// </summary>
        public void TakeDamage(int damage)
        {
            if (isDestroyed)
                return;

            currentHealth = Mathf.Max(0, currentHealth - damage);
            UpdateHealthDisplay();

            if (currentHealth <= 0)
            {
                DestroyDoor();
            }
        }

        private void DestroyDoor()
        {
            if (isDestroyed)
                return;

            isDestroyed = true;

            // Görsel efekt (kırılma efekti)
            if (destroyedEffect != null)
                Instantiate(destroyedEffect, transform.position, Quaternion.identity);

            // Kapıyı gizle/yok et (alternatif son - oyuncu keşfetmeli)
            if (doorSprite != null)
                doorSprite.enabled = false; // Veya Destroy(doorSprite.gameObject)

            // Collider'ı kaldır (artık geçilebilir)
            Collider2D col = GetComponent<Collider2D>();
            if (col != null)
                col.enabled = false;

            // Win condition'a bildir
            if (BossWinHandler.Instance != null)
                BossWinHandler.Instance.OnDoorDestroyed();
        }

        private void UpdateHealthDisplay()
        {
            // Minimal UI - Kapı hasar göstermez, sadece kırıldığında yok olur
            // Alternatif son: Oyuncu kapının kırılabileceğini keşfetmeli
            // Hasar durumunda görsel değişiklik YOK
        }

        public bool IsDestroyed()
        {
            return isDestroyed;
        }

        public int GetCurrentHealth()
        {
            return currentHealth;
        }

        public int GetMaxHealth()
        {
            return maxHealth;
        }
    }
}

