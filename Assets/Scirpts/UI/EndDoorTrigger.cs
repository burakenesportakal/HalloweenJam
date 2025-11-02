using UnityEngine;
using HalloweenJam.UI;

namespace HalloweenJam.UI
{
    /// <summary>
    /// Oyun sonu kapısı - Oyuncu kapıya değdiğinde outro sahnesine geçer
    /// </summary>
    public class EndDoorTrigger : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private string playerTag = "Player";
        [SerializeField] private bool hasEntered = false; // Tek sefer tetiklenmesi için

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (hasEntered)
                return;

            // Oyuncu kapıya değdi
            if (other.CompareTag(playerTag))
            {
                hasEntered = true;
                WinGame();
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (hasEntered)
                return;

            // Oyuncu kapıya değdi (Collider2D, trigger değilse)
            if (collision.gameObject.CompareTag(playerTag))
            {
                hasEntered = true;
                WinGame();
            }
        }

        private void WinGame()
        {
            Debug.Log("End door reached! Game won!");
            
            // GameManager'a win bildir
            if (GameManager.Instance != null)
            {
                GameManager.Instance.WinGame();
            }
            else
            {
                Debug.LogError("EndDoorTrigger: GameManager bulunamadı! Outro sahnesine geçilemiyor.");
            }
        }
    }
}

