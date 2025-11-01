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
        [SerializeField] private AlarmUI alarmUI;

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
            if (mainMenuPanel == null)
                Debug.LogWarning("UIManager: MainMenuPanel referansı atanmamış!");
            if (settingsPanel == null)
                Debug.LogWarning("UIManager: SettingsPanel referansı atanmamış!");
            if (storyboardPanel == null)
                Debug.LogWarning("UIManager: StoryboardPanel referansı atanmamış!");
            if (pausePanel == null)
                Debug.LogWarning("UIManager: PausePanel referansı atanmamış!");
            if (alarmUI == null)
                Debug.LogWarning("UIManager: AlarmUI referansı atanmamış!");
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
        public void ShowSettings()
        {
            HideAllPanels();
            if (settingsPanel != null)
                settingsPanel.SetActive(true);
            
            if (settingsManager != null)
                settingsManager.OnPanelShown();
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

        #region Alarm UI

        /// <summary>
        /// Alarm UI'ı gösterir (alarm çaldığında)
        /// </summary>
        public void ShowAlarmUI()
        {
            if (alarmUI != null)
                alarmUI.Show();
        }

        /// <summary>
        /// Alarm UI'ı gizler (alarm bittiğinde)
        /// </summary>
        public void HideAlarmUI()
        {
            if (alarmUI != null)
                alarmUI.Hide();
        }

        /// <summary>
        /// Alarm sayacını günceller
        /// </summary>
        public void UpdateAlarmTimer(float remainingTime)
        {
            if (alarmUI != null)
                alarmUI.UpdateTimer(remainingTime);
        }

        /// <summary>
        /// Can barını günceller
        /// </summary>
        public void UpdateHealth(int currentHealth, int maxHealth)
        {
            if (alarmUI != null)
                alarmUI.UpdateHealth(currentHealth, maxHealth);
        }

        #endregion
    }
}

