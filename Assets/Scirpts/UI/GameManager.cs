using UnityEngine;

namespace HalloweenJam.UI
{
    /// <summary>
    /// Oyun durumu yönetimi (Menü, Oyun, Pause)
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public enum GameState
        {
            MainMenu,
            Storyboard,
            Playing,
            Paused,
            GameOver
        }

        public GameState CurrentState { get; private set; } = GameState.MainMenu;

        [Header("Settings")]
        [SerializeField] private KeyCode pauseKey = KeyCode.Escape;

        private void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        private void Start()
        {
            // Oyun başladığında ana menüyü göster
            ShowMainMenu();
        }

        private void Update()
        {
            // ESC tuşu ile pause/unpause
            if (Input.GetKeyDown(pauseKey))
            {
                if (CurrentState == GameState.Playing)
                {
                    PauseGame();
                }
                else if (CurrentState == GameState.Paused)
                {
                    ResumeGame();
                }
            }
        }

        /// <summary>
        /// Ana menüyü gösterir
        /// </summary>
        public void ShowMainMenu()
        {
            CurrentState = GameState.MainMenu;
            Time.timeScale = 1f;

            if (UIManager.Instance != null)
                UIManager.Instance.ShowMainMenu();
        }

        /// <summary>
        /// Oyunu başlatır (storyboard'dan sonra)
        /// </summary>
        public void StartGame()
        {
            CurrentState = GameState.Playing;
            Time.timeScale = 1f;

            // Tüm menü panellerini gizle
            if (UIManager.Instance != null)
                UIManager.Instance.HideAllPanels();

            Debug.Log("Game Started!");
            // Burada oyun başlangıç logic'i eklenebilir
        }

        /// <summary>
        /// Oyunu duraklatır
        /// </summary>
        public void PauseGame()
        {
            if (CurrentState != GameState.Playing)
                return;

            CurrentState = GameState.Paused;
            Time.timeScale = 0f;

            if (UIManager.Instance != null)
                UIManager.Instance.ShowPauseMenu();
        }

        /// <summary>
        /// Oyunu devam ettirir
        /// </summary>
        public void ResumeGame()
        {
            if (CurrentState != GameState.Paused)
                return;

            CurrentState = GameState.Playing;
            Time.timeScale = 1f;

            if (UIManager.Instance != null)
                UIManager.Instance.HidePauseMenu();
        }

        /// <summary>
        /// Ana menüye döner (pause menüsünden)
        /// </summary>
        public void ReturnToMainMenu()
        {
            CurrentState = GameState.MainMenu;
            Time.timeScale = 1f;

            if (UIManager.Instance != null)
            {
                UIManager.Instance.HidePauseMenu();
                UIManager.Instance.ShowMainMenu();
            }

            // Oyun durumunu sıfırla (ileride restart için)
            ResetGame();
        }

        /// <summary>
        /// Oyuncu öldüğünde çağrılır
        /// </summary>
        public void OnPlayerDeath()
        {
            CurrentState = GameState.GameOver;
            Time.timeScale = 0f;

            // Oyunu baştan başlat (kısa bir delay ile)
            Invoke(nameof(RestartGame), 2f);
        }

        /// <summary>
        /// Oyunu baştan başlatır
        /// </summary>
        private void RestartGame()
        {
            Time.timeScale = 1f;
            
            // Sahneyi yeniden yükle
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
            );
        }

        /// <summary>
        /// Oyun durumunu sıfırlar
        /// </summary>
        private void ResetGame()
        {
            // Oyun durumunu sıfırla (ileride eklenebilir)
        }
    }
}

