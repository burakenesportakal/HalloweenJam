using UnityEngine;

public class Exit : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    
    public int keycardCount = 0; // number of collected keycards

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ÖNEMLİ: Player enemy kontrol ediyorsa hiçbir collision işlemi yapma
        if (collision.CompareTag(playerTag))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null && player.IsControllingEnemy())
            {
                // Player enemy kontrol ediyor, tüm collision işlemlerini atla
                return;
            }
        }
        
        // Keycard toplama
        if (collision.CompareTag("Keycard"))
        {
            keycardCount++; // increase by 1
            Destroy(collision.gameObject); // remove the keycard
            Debug.Log("Keycard collected! Total: " + keycardCount);
        }
        
        // Player kontrolü - Normal çıkış işlemleri
        if (collision.CompareTag(playerTag))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                // Normal çıkış işlemleri buraya eklenebilir
                // Örnek: Scene geçişi, level tamamlama vb.
            }
        }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        // ÖNEMLİ: Player enemy kontrol ediyorsa hiçbir collision işlemi yapma
        if (collision.CompareTag(playerTag))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null && player.IsControllingEnemy())
            {
                // Player enemy kontrol ediyor, tüm collision işlemlerini atla
                return;
            }
        }
        
        // Player çıkış alanından ayrıldığında (opsiyonel)
        if (collision.CompareTag(playerTag))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                // Player çıkış alanından ayrıldı
                // Buraya gerekli işlemler eklenebilir
            }
        }
    }
}
