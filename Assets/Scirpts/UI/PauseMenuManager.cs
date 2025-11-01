using UnityEngine;
using UnityEngine.UI;

namespace HalloweenJam.UI
{
    /// <summary>
    /// Oyun içi duraklatma menüsü yönetimi
    /// </summary>
    public class PauseMenuManager : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button quitButton;

        private void Awake()
        {
            // Referansları otomatik bul
            if (resumeButton == null)
                resumeButton = transform.Find("ResumeButton")?.GetComponent<Button>();
            
            if (settingsButton == null)
                settingsButton = transform.Find("SettingsButton")?.GetComponent<Button>();
            
            if (mainMenuButton == null)
                mainMenuButton = transform.Find("MainMenuButton")?.GetComponent<Button>();
            
            if (quitButton == null)
                quitButton = transform.Find("QuitButton")?.GetComponent<Button>();
        }

        private void Start()
        {
            SetupButtons();
        }

        private void SetupButtons()
        {
            // Resume Button
            if (resumeButton != null)
                resumeButton.onClick.AddListener(OnResumeButtonClicked);

            // Settings Button
            if (settingsButton != null)
                settingsButton.onClick.AddListener(OnSettingsButtonClicked);

            // Main Menu Button
            if (mainMenuButton != null)
                mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);

            // Quit Button
            if (quitButton != null)
                quitButton.onClick.AddListener(OnQuitButtonClicked);
        }

        private void OnResumeButtonClicked()
        {
            // Oyunu devam ettir
            if (GameManager.Instance != null)
                GameManager.Instance.ResumeGame();
        }

        private void OnSettingsButtonClicked()
        {
            // Ayarlar menüsüne geç (pause menüsünden geldiğini belirt)
            if (UIManager.Instance != null)
                UIManager.Instance.ShowSettings(fromPauseMenu: true);
        }

        private void OnMainMenuButtonClicked()
        {
            // Ana menüye dön
            if (GameManager.Instance != null)
                GameManager.Instance.ReturnToMainMenu();
        }

        private void OnQuitButtonClicked()
        {
            // Oyunu kapat
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }

        /// <summary>
        /// Panel gösterildiğinde çağrılır
        /// </summary>
        public void OnPanelShown()
        {
            // Gerekirse ek işlemler yapılabilir
        }

        private void OnDestroy()
        {
            // Event listener'ları temizle
            if (resumeButton != null)
                resumeButton.onClick.RemoveAllListeners();
            if (settingsButton != null)
                settingsButton.onClick.RemoveAllListeners();
            if (mainMenuButton != null)
                mainMenuButton.onClick.RemoveAllListeners();
            if (quitButton != null)
                quitButton.onClick.RemoveAllListeners();
        }
    }
}

