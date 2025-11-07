using UnityEngine;
using UnityEngine.SceneManagement;
using HalloweenJam.UI;

namespace HalloweenJam.UI
{
    /// <summary>
    /// Oyun sonu transition - Kapıya değince transition gösterir, sonra outro sahnesine geçer
    /// </summary>
    public class GameToOutroTransition : MonoBehaviour
    {
        [Header("Transition Settings")]
        [SerializeField] private float transitionDuration = 2f; // Transition süresi (saniye)
        [SerializeField] private bool autoTransition = true; // Otomatik geçiş
        [SerializeField] private KeyCode skipKey = KeyCode.Space; // Atlama tuşu

        [Header("UI Elements (Opsiyonel)")]
        [SerializeField] private GameObject transitionPanel; // Transition ekranı (fade, loading vs.)

        private bool transitionStarted = false;
        private float timer = 0f;

        /// <summary>
        /// Transition'ı başlat (EndDoorTrigger'dan çağrılacak)
        /// </summary>
        public void StartTransition()
        {
            if (transitionStarted) return;

            transitionStarted = true;
            timer = 0f;

            Debug.Log("Game to Outro transition started!");

            // Transition panel'ini göster (varsa)
            if (transitionPanel != null)
            {
                transitionPanel.SetActive(true);
            }

            // Zamanı durdur (opsiyonel - transition sırasında oyun durur)
            Time.timeScale = 0f;
        }

        private void Update()
        {
            if (!transitionStarted) return;

            // Atlama tuşu kontrolü
            if (Input.GetKeyDown(skipKey))
            {
                LoadOutroScene();
                return;
            }

            // Otomatik geçiş
            if (autoTransition)
            {
                // Time.timeScale = 0 olduğu için Time.unscaledDeltaTime kullan
                timer += Time.unscaledDeltaTime;
                if (timer >= transitionDuration)
                {
                    LoadOutroScene();
                }
            }
        }

        /// <summary>
        /// Outro sahnesine geç
        /// </summary>
        private void LoadOutroScene()
        {
            // Zamanı normale döndür
            Time.timeScale = 1f;

            Debug.Log("Loading outro scene...");

            // GameManager varsa onu kullan, yoksa direkt yükle
            if (GameManager.Instance != null)
            {
                GameManager.Instance.WinGame();
            }
            else
            {
                // GameManager yoksa direkt outro sahnesine geç (index 3)
                SceneManager.LoadScene(3);
            }
        }
    }
}

