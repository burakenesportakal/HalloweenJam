using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace HalloweenJam.UI
{
    /// <summary>
    /// Ana menü paneli yönetimi
    /// </summary>
    public class MainMenuManager : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI gameTitleText;
        [SerializeField] private Button playButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;

        private void Awake()
        {
            // Referansları otomatik bul (eğer atanmamışsa)
            if (gameTitleText == null)
                gameTitleText = transform.Find("GameTitle")?.GetComponent<TextMeshProUGUI>();
            
            if (playButton == null)
                playButton = transform.Find("PlayButton")?.GetComponent<Button>();
            
            if (settingsButton == null)
                settingsButton = transform.Find("SettingsButton")?.GetComponent<Button>();
            
            if (quitButton == null)
                quitButton = transform.Find("QuitButton")?.GetComponent<Button>();
        }

        private void Start()
        {
            SetupButtons();
        }

        private void SetupButtons()
        {
            // Play Button
            if (playButton != null)
                playButton.onClick.AddListener(OnPlayButtonClicked);

            // Settings Button
            if (settingsButton != null)
                settingsButton.onClick.AddListener(OnSettingsButtonClicked);

            // Quit Button
            if (quitButton != null)
                quitButton.onClick.AddListener(OnQuitButtonClicked);
        }

        private void OnPlayButtonClicked()
        {
            // Storyboard ekranına geç
            if (UIManager.Instance != null)
                UIManager.Instance.ShowStoryboard();
        }

        private void OnSettingsButtonClicked()
        {
            // Ayarlar menüsüne geç
            if (UIManager.Instance != null)
                UIManager.Instance.ShowSettings();
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
            if (playButton != null)
                playButton.onClick.RemoveAllListeners();
            if (settingsButton != null)
                settingsButton.onClick.RemoveAllListeners();
            if (quitButton != null)
                quitButton.onClick.RemoveAllListeners();
        }
    }
}

