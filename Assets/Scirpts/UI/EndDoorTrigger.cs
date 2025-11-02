using UnityEngine;
using HalloweenJam.UI;

namespace HalloweenJam.UI
{
    /// <summary>
    /// Oyun sonu kapısı - Oyuncu kapıya değdiğinde oyunu kapatır veya outro sahnesine geçer
    /// </summary>
    public class EndDoorTrigger : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private string playerTag = "Player";
        [SerializeField] private bool closeGame = false; // True = Oyunu kapat, False = Outro sahnesine geç

        private bool hasTriggered = false;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (hasTriggered) return;

            if (other.CompareTag(playerTag))
            {
                hasTriggered = true;
                OnPlayerReachedDoor();
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (hasTriggered) return;

            if (collision.gameObject.CompareTag(playerTag))
            {
                hasTriggered = true;
                OnPlayerReachedDoor();
            }
        }

        private void OnPlayerReachedDoor()
        {
            Debug.Log("End door reached!");

            if (closeGame)
            {
                // Oyunu kapat
                #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
                #else
                    Application.Quit();
                #endif
            }
            else
            {
                // Outro sahnesine geç
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.WinGame();
                }
                else
                {
                    Debug.LogError("EndDoorTrigger: GameManager bulunamadı!");
                }
            }
        }
    }
}
