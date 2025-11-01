using UnityEngine;

namespace HalloweenJam.Boss
{
    /// <summary>
    /// Boss mermisi - Kapıya ve player'a hasar verir
    /// </summary>
    public class BossBullet : MonoBehaviour
    {
        [Header("Bullet Settings")]
        [SerializeField] private int damage = 10;
        [SerializeField] private float lifetime = 3f;
        [SerializeField] private LayerMask damageLayers;

        [Header("Visual Effects")]
        [SerializeField] private GameObject hitEffect;

        private void Start()
        {
            // Lifetime sonrası yok ol
            Destroy(gameObject, lifetime);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Player'a hasar
            if (collision.CompareTag("Player") || collision.CompareTag("Symbiote"))
            {
                // Player damage sistemi buraya entegre edilecek
                // PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
                // if (playerHealth != null) playerHealth.TakeDamage(damage);
                
                DestroyBullet();
                return;
            }

            // Kapıya hasar
            EndGameDoor door = collision.GetComponent<EndGameDoor>();
            if (door != null)
            {
                door.TakeDamage(damage);
                DestroyBullet();
                return;
            }

            // Duvar/zemin (sadece yok ol)
            if (collision.CompareTag("Wall") || collision.CompareTag("Ground"))
            {
                DestroyBullet();
            }
        }

        private void DestroyBullet()
        {
            // Hit effect
            if (hitEffect != null)
                Instantiate(hitEffect, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }
}

