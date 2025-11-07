using UnityEngine;
using UnityEngine.SceneManagement;
using HalloweenJam.UI;

namespace HalloweenJam.UI
{
    /// <summary>
    /// Intro sahnesi yöneticisi - Intro bittiğinde UI sahnesine geçer
    /// </summary>
    public class IntroController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float introDuration = 5f; // Intro süresi (saniye)
        [SerializeField] private bool autoTransition = true; // Otomatik geçiş (süre bitince)
        [SerializeField] private KeyCode skipKey = KeyCode.Space; // Atlama tuşu

        [Header("Scene Settings")]
        [SerializeField] private int uiSceneIndex = 1; // UI sahnesi build index

        private float timer = 0f;

        private void Update()
        {
            // Atlama tuşu kontrolü
            if (Input.GetKeyDown(skipKey))
            {
                LoadUIScene();
                return;
            }

            // Otomatik geçiş
            if (autoTransition)
            {
                timer += Time.deltaTime;
                if (timer >= introDuration)
                {
                    LoadUIScene();
                }
            }
        }

        /// <summary>
        /// UI sahnesine geç (buton ile de çağrılabilir)
        /// </summary>
        public void LoadUIScene()
        {
            // GameManager varsa onu kullan, yoksa direkt yükle
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ShowMainMenu();
                SceneManager.LoadScene(uiSceneIndex);
            }
            else
            {
                SceneManager.LoadScene(uiSceneIndex);
            }
        }
    }
}

