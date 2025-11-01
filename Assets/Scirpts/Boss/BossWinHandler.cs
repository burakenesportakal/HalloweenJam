using UnityEngine;
using UnityEngine.SceneManagement;

namespace HalloweenJam.Boss
{
    /// <summary>
    /// Boss savaşı kazanma/kaybetme durumu yönetimi
    /// </summary>
    public class BossWinHandler : MonoBehaviour
    {
        public static BossWinHandler Instance { get; private set; }

        [Header("Win Conditions")]
        [SerializeField] private bool bossTakenOver = false;
        [SerializeField] private bool doorDestroyed = false;

        [Header("Post-Win Settings")]
        [SerializeField] private float winDelay = 2f; // Win sonrası bekleme
        [SerializeField] private string nextSceneName = "MainMenu"; // Oyun sonu sahnesi

        private bool gameWon = false;

        private void Awake()
        {
            // Singleton
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        public void OnBossTakenOver()
        {
            bossTakenOver = true;
            CheckWinCondition();
        }

        public void OnDoorDestroyed()
        {
            doorDestroyed = true;
            CheckWinCondition();
        }

        private void CheckWinCondition()
        {
            if (gameWon)
                return;

            // Her iki durumdan biri gerçekleşirse kazan
            if (bossTakenOver || doorDestroyed)
            {
                WinGame();
            }
        }

        private void WinGame()
        {
            gameWon = true;

            Debug.Log("Boss Savaşı Kazanıldı!");

            // Win sonrası işlemler
            // - Win ekranı göster
            // - Ses çal
            // - Sonraki sahneye geç

            Invoke(nameof(LoadNextScene), winDelay);
        }

        private void LoadNextScene()
        {
            if (!string.IsNullOrEmpty(nextSceneName))
            {
                SceneManager.LoadScene(nextSceneName);
            }
        }

        public bool IsGameWon()
        {
            return gameWon;
        }
    }
}

