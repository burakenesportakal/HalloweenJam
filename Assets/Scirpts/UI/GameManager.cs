using UnityEngine;
using UnityEngine.SceneManagement;

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
        [SerializeField] private int uiSceneIndex = 1; // UI sahnesi build index
        [SerializeField] private int gameSceneIndex = 2; // Oyun sahnesi build index
        [SerializeField] private int outroSceneIndex = 3; // Outro sahnesi build index

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

        private void OnEnable()
        {
            // Sahne yüklendiğinde kontrol et
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // UI sahnesinde ana menüyü göster
            if (scene.buildIndex == uiSceneIndex)
            {
                ShowMainMenu();
            }
            // Oyun sahnesinde oyunu başlat
            else if (scene.buildIndex == gameSceneIndex)
            {
                CurrentState = GameState.Playing;
                Time.timeScale = 1f;
                
                // Tüm menü panellerini gizle (oyun sahnesinde yok zaten ama güvenlik için)
                if (UIManager.Instance != null)
                    UIManager.Instance.HideAllPanels();
            }
        }

        private void Start()
        {
            // Eğer oyun sahnesine direkt girildiyse ve state Playing değilse, Playing yap
            if (SceneManager.GetActiveScene().buildIndex == gameSceneIndex)
            {
                if (CurrentState != GameState.Playing && CurrentState != GameState.Paused)
                {
                    CurrentState = GameState.Playing;
                    Time.timeScale = 1f;
                    Debug.Log($"GameManager: Oyun sahnesine direkt girildi. State = {CurrentState}");
                }
            }
        }

        private void Update()
        {
            // ESC tuşu ile pause/unpause - Sadece oyun sahnesindeyken
            if (SceneManager.GetActiveScene().buildIndex == gameSceneIndex)
            {
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
        /// Oyunu başlatır (storyboard'dan sonra) - Oyun sahnesine geçer
        /// </summary>
        public void StartGame()
        {
            CurrentState = GameState.Playing;
            Time.timeScale = 1f;

            // Tüm menü panellerini gizle
            if (UIManager.Instance != null)
                UIManager.Instance.HideAllPanels();

            // Oyun sahnesine geç
            SceneManager.LoadScene(gameSceneIndex);

            Debug.Log("Game Started! Loading game scene...");
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
        /// Ana menüye döner (pause menüsünden) - UI sahnesine geçer
        /// </summary>
        public void ReturnToMainMenu()
        {
            CurrentState = GameState.MainMenu;
            Time.timeScale = 1f;

            if (UIManager.Instance != null)
            {
                UIManager.Instance.HidePauseMenu();
            }

            // UI sahnesine dön (ShowMainMenu otomatik çağrılacak - OnSceneLoaded'da)
            SceneManager.LoadScene(uiSceneIndex);

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
        /// Oyunu kazandığında çağrılır (boss yenildi veya kapı kırıldı) - Outro sahnesine geçer
        /// </summary>
        public void WinGame()
        {
            CurrentState = GameState.GameOver;
            Time.timeScale = 1f;

            Debug.Log("Game Won! Loading outro scene...");

            // Outro sahnesine geç
            SceneManager.LoadScene(outroSceneIndex);
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

