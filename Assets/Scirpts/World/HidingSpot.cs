using UnityEngine;

public class HidingSpot : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private LayerMask enemyLayer;

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

        // Ölü enemy geldiğinde (taşınan enemy)
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
            }
        }
    }
}

