using UnityEngine;

public class RedGate : MonoBehaviour
{
    [Header("Teleport Settings")]
    [SerializeField] private Transform teleportTarget;   // The place where the player will appear after the gate
    
    [Header("Keycard Requirements")]
    [SerializeField] private Exit keycardCount;  // Reference to Exit script that tracks keycard count
    [SerializeField] private int requiredKeycards = 1;  // Bu gate için gerekli keycard sayısı
    
    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // Make sure your player is tagged "Player"
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            
            // ÖNEMLİ: Player enemy kontrol ediyorsa teleport etme
            if (player != null && player.IsControllingEnemy())
            {
                if (showDebugLogs)
                    Debug.Log("Player enemy kontrol ediyor, teleport yapılamaz!");
                return; // Player enemy kontrol ediyor, teleport işlemi yapılmasın
            }
            
            // Keycard kontrolü
            if (keycardCount != null && keycardCount.keycardCount >= requiredKeycards)
            {
                // Yeterli keycard var, teleport et
                if (teleportTarget != null)
                {
                    collision.transform.position = teleportTarget.position; // move player
                    if (showDebugLogs)
                        Debug.Log($"Gate opened! Player teleported. Keycards: {keycardCount.keycardCount}/{requiredKeycards}");
                }
                else
                {
                    Debug.LogWarning("RedGate: Teleport target atanmamış!");
                }
            }
            else
            {
                // Yeterli keycard yok
                int currentCount = keycardCount != null ? keycardCount.keycardCount : 0;
                if (showDebugLogs)
                    Debug.Log($"Gate locked! Need {requiredKeycards} keycard(s). Current: {currentCount}");
            }
        }
    }
    
    // Inspector'da test için
    private void OnValidate()
    {
        // requiredKeycards negatif olamaz
        if (requiredKeycards < 0)
        {
            requiredKeycards = 0;
        }
    }
}
