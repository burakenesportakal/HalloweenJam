using UnityEngine;

namespace HalloweenJam.UI
{
    /// <summary>
    /// Merkezi UI yönetim sistemi. Tüm menü panellerini ve oyun içi UI'ı yönetir.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("Menu Panels")]
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private GameObject storyboardPanel;
        [SerializeField] private GameObject pausePanel;

        [Header("Game UI")]
        [SerializeField] private HealthUI healthUI;

        [Header("Canvas")]
        [SerializeField] private Canvas mainCanvas;

        // Manager referansları - panel GameObject'lerinden otomatik bulunur
        private MainMenuManager mainMenuManager;
        private SettingsManager settingsManager;
        private StoryboardManager storyboardManager;
        private PauseMenuManager pauseMenuManager;

        private void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
                // UIManager DontDestroyOnLoad kullanmıyor - her sahne kendi UI'sını yönetir
                // UI Sahnesi: MenuCanvas (MainMenu, Settings, Storyboard)
                // Oyun Sahnesi: GameCanvas (PauseMenu, HealthUI)
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            InitializeUI();
        }

        private void InitializeUI()
        {
            // Canvas kontrolü
            if (mainCanvas == null)
                mainCanvas = GetComponentInParent<Canvas>();

            // Manager referanslarını panel'lerden otomatik bul
            if (mainMenuPanel != null)
                mainMenuManager = mainMenuPanel.GetComponent<MainMenuManager>();
            if (settingsPanel != null)
                settingsManager = settingsPanel.GetComponent<SettingsManager>();
            if (storyboardPanel != null)
                storyboardManager = storyboardPanel.GetComponent<StoryboardManager>();
            if (pausePanel != null)
                pauseMenuManager = pausePanel.GetComponent<PauseMenuManager>();

            // Panel referanslarını kontrol et
            ValidateReferences();
        }

        private void ValidateReferences()
        {
            // UI Sahnesi panelleri (null olabilir - oyun sahnesinde yok)
            if (mainMenuPanel == null)
                Debug.LogWarning("UIManager: MainMenuPanel referansı atanmamış! (UI sahnesinde olmalı)");
            if (settingsPanel == null)
                Debug.LogWarning("UIManager: SettingsPanel referansı atanmamış! (UI sahnesinde olmalı)");
            if (storyboardPanel == null)
                Debug.LogWarning("UIManager: StoryboardPanel referansı atanmamış! (UI sahnesinde olmalı)");
            
            // Oyun sahnesi UI elementleri (null olabilir - UI sahnesinde yok)
            if (pausePanel == null)
                Debug.LogWarning("UIManager: PausePanel referansı atanmamış! (Oyun sahnesinde olmalı)");
            if (healthUI == null)
                Debug.LogWarning("UIManager: HealthUI referansı atanmamış! (Oyun sahnesinde olmalı)");
        }

        #region Panel Management

        /// <summary>
        /// Ana menüyü gösterir
        /// </summary>
        public void ShowMainMenu()
        {
            HideAllPanels();
            if (mainMenuPanel != null)
                mainMenuPanel.SetActive(true);
            
            if (mainMenuManager != null)
                mainMenuManager.OnPanelShown();
        }

        /// <summary>
        /// Ayarlar menüsünü gösterir
        /// </summary>
        /// <param name="fromPauseMenu">Pause menüsünden mi geldi (true) yoksa ana menüden mi (false)</param>
        public void ShowSettings(bool fromPauseMenu = false)
        {
            HideAllPanels();
            if (settingsPanel != null)
                settingsPanel.SetActive(true);
            
            if (settingsManager != null)
                settingsManager.OnPanelShown(fromPauseMenu);
        }

        /// <summary>
        /// Storyboard ekranını gösterir
        /// </summary>
        public void ShowStoryboard()
        {
            HideAllPanels();
            if (storyboardPanel != null)
                storyboardPanel.SetActive(true);
            
            if (storyboardManager != null)
                storyboardManager.StartStoryboard();
        }

        /// <summary>
        /// Duraklatma menüsünü gösterir
        /// </summary>
        public void ShowPauseMenu()
        {
            // Settings panel'ini gizle (eğer açıksa)
            if (settingsPanel != null)
                settingsPanel.SetActive(false);
            
            // Pause panel'ini göster
            if (pausePanel != null)
                pausePanel.SetActive(true);
            
            if (pauseMenuManager != null)
                pauseMenuManager.OnPanelShown();
        }

        /// <summary>
        /// Duraklatma menüsünü gizler
        /// </summary>
        public void HidePauseMenu()
        {
            if (pausePanel != null)
                pausePanel.SetActive(false);
        }

        /// <summary>
        /// Tüm panelleri gizler (oyun modu için)
        /// </summary>
        public void HideAllPanels()
        {
            if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
            if (settingsPanel != null) settingsPanel.SetActive(false);
            if (storyboardPanel != null) storyboardPanel.SetActive(false);
            if (pausePanel != null) pausePanel.SetActive(false);
        }

        #endregion

        #region Health UI

        /// <summary>
        /// Can barını günceller (sol üstte her zaman görünür)
        /// </summary>
        public void UpdateHealth(int currentHealth, int maxHealth)
        {
            if (healthUI != null)
                healthUI.UpdateHealth(currentHealth, maxHealth);
        }

        #endregion
    }
}

