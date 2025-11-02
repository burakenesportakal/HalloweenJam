using UnityEngine;

public class RedGate : MonoBehaviour
{
   
        public Transform teleportTarget;   // The place where the player will appear after the gate
        public Exit keycardCount;  // Reference to your player�s KeycardCollector script

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player")) // Make sure your player is tagged "Player"
            {
                PlayerController player = collision.GetComponent<PlayerController>();
                
                // ÖNEMLİ: Player enemy kontrol ediyorsa teleport etme
                if (player != null && player.IsControllingEnemy())
                {
                    Debug.Log("Player enemy kontrol ediyor, teleport yapılamaz!");
                    return; // Player enemy kontrol ediyor, teleport işlemi yapılmasın
                }
                
                if (keycardCount != null && keycardCount.keycardCount >= 1)
                {
                    collision.transform.position = teleportTarget.position; // move player
                    Debug.Log("Gate opened! Player teleported.");
                }
                else
                {
                    Debug.Log("Gate locked. Need a keycard!");
                }
            }
        }
    }


