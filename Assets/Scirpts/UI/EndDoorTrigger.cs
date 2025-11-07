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
        [SerializeField] private bool closeGame = false; // True = Oyunu kapat, False = Outro sahnesine geç (varsayılan: False = Outro'ya git)

        [Header("Transition (Opsiyonel)")]
        [SerializeField] private GameToOutroTransition transition; // Transition script referansı

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
            Debug.Log("End door reached! Close Game = " + closeGame);

            if (closeGame)
            {
                // Oyunu kapat
                Debug.Log("Closing game...");
                #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
                #else
                    Application.Quit();
                #endif
            }
            else
            {
                // Transition varsa onu kullan, yoksa direkt outro'ya geç
                if (transition != null)
                {
                    Debug.Log("Starting transition to outro...");
                    transition.StartTransition();
                }
                else
                {
                    // Transition yok, direkt outro'ya geç
                    if (GameManager.Instance != null)
                    {
                        Debug.Log("Calling GameManager.WinGame()...");
                        GameManager.Instance.WinGame();
                    }
                    else
                    {
                        Debug.LogError("EndDoorTrigger: GameManager.Instance bulunamadı! Oyun sahnesinde GameManager var mı?");
                    }
                }
            }
        }
    }
}
