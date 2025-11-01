using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace HalloweenJam.UI
{
    /// <summary>
    /// Ayarlar menüsü yönetimi (Ses ve Dil ayarları)
    /// </summary>
    public class SettingsManager : MonoBehaviour
    {
        [Header("Audio Settings")]
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private TextMeshProUGUI masterVolumeText;
        [SerializeField] private TextMeshProUGUI musicVolumeText;
        [SerializeField] private TextMeshProUGUI sfxVolumeText;

        [Header("Language Settings")]
        [SerializeField] private TMPro.TMP_Dropdown languageDropdown;
        [SerializeField] private TextMeshProUGUI languageLabel;

        [Header("Buttons")]
        [SerializeField] private Button backButton;

        [Header("Default Values")]
        [SerializeField] private float defaultMasterVolume = 1f;
        [SerializeField] private float defaultMusicVolume = 0.8f;
        [SerializeField] private float defaultSFXVolume = 1f;

        private bool cameFromPauseMenu = false; // Pause menüsünden mi geldi?

        private void Awake()
        {
            // Referansları otomatik bul
            if (backButton == null)
                backButton = transform.Find("BackButton")?.GetComponent<Button>();
        }

        private void Start()
        {
            SetupSliders();
            SetupLanguageDropdown();
            SetupButtons();
            LoadSettings();
        }

        private void SetupSliders()
        {
            // Master Volume Slider
            if (masterVolumeSlider != null)
            {
                masterVolumeSlider.minValue = 0f;
                masterVolumeSlider.maxValue = 1f;
                masterVolumeSlider.value = defaultMasterVolume;
                masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
            }

            // Music Volume Slider
            if (musicVolumeSlider != null)
            {
                musicVolumeSlider.minValue = 0f;
                musicVolumeSlider.maxValue = 1f;
                musicVolumeSlider.value = defaultMusicVolume;
                musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            }

            // SFX Volume Slider
            if (sfxVolumeSlider != null)
            {
                sfxVolumeSlider.minValue = 0f;
                sfxVolumeSlider.maxValue = 1f;
                sfxVolumeSlider.value = defaultSFXVolume;
                sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
            }

            UpdateVolumeTexts();
        }

        private void SetupLanguageDropdown()
        {
            if (languageDropdown != null)
            {
                // Şimdilik sadece Türkçe var, ileride genişletilebilir
                languageDropdown.options.Clear();
                languageDropdown.options.Add(new TMPro.TMP_Dropdown.OptionData("Türkçe"));
                languageDropdown.value = 0;
                languageDropdown.RefreshShownValue();
            }
        }

        private void SetupButtons()
        {
            if (backButton != null)
                backButton.onClick.AddListener(OnBackButtonClicked);
        }

        private void OnMasterVolumeChanged(float value)
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.SetMasterVolume(value);
            
            UpdateVolumeTexts();
        }

        private void OnMusicVolumeChanged(float value)
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.SetMusicVolume(value);
            
            UpdateVolumeTexts();
        }

        private void OnSFXVolumeChanged(float value)
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.SetSFXVolume(value);
            
            UpdateVolumeTexts();
        }

        private void UpdateVolumeTexts()
        {
            if (masterVolumeText != null && masterVolumeSlider != null)
                masterVolumeText.text = $"Master: {(int)(masterVolumeSlider.value * 100)}%";
            
            if (musicVolumeText != null && musicVolumeSlider != null)
                musicVolumeText.text = $"Music: {(int)(musicVolumeSlider.value * 100)}%";
            
            if (sfxVolumeText != null && sfxVolumeSlider != null)
                sfxVolumeText.text = $"SFX: {(int)(sfxVolumeSlider.value * 100)}%";
        }

        private void OnBackButtonClicked()
        {
            // Nereden geldiysek oraya dön
            if (UIManager.Instance != null)
            {
                if (cameFromPauseMenu)
                {
                    // Pause menüsünden geldiyse pause menüsüne dön
                    UIManager.Instance.ShowPauseMenu();
                }
                else
                {
                    // Ana menüden geldiyse ana menüye dön
                    UIManager.Instance.ShowMainMenu();
                }
            }
        }

        /// <summary>
        /// Panel gösterildiğinde çağrılır
        /// </summary>
        /// <param name="fromPauseMenu">Pause menüsünden mi geldi</param>
        public void OnPanelShown(bool fromPauseMenu = false)
        {
            cameFromPauseMenu = fromPauseMenu;
            LoadSettings();
        }

        private void LoadSettings()
        {
            // PlayerPrefs'ten ayarları yükle
            float masterVol = PlayerPrefs.GetFloat("MasterVolume", defaultMasterVolume);
            float musicVol = PlayerPrefs.GetFloat("MusicVolume", defaultMusicVolume);
            float sfxVol = PlayerPrefs.GetFloat("SFXVolume", defaultSFXVolume);

            if (masterVolumeSlider != null)
                masterVolumeSlider.value = masterVol;
            if (musicVolumeSlider != null)
                musicVolumeSlider.value = musicVol;
            if (sfxVolumeSlider != null)
                sfxVolumeSlider.value = sfxVol;

            UpdateVolumeTexts();
        }

        private void SaveSettings()
        {
            // Ayarları PlayerPrefs'e kaydet
            if (masterVolumeSlider != null)
                PlayerPrefs.SetFloat("MasterVolume", masterVolumeSlider.value);
            if (musicVolumeSlider != null)
                PlayerPrefs.SetFloat("MusicVolume", musicVolumeSlider.value);
            if (sfxVolumeSlider != null)
                PlayerPrefs.SetFloat("SFXVolume", sfxVolumeSlider.value);
            
            PlayerPrefs.Save();
        }

        private void OnDestroy()
        {
            SaveSettings();
            
            // Event listener'ları temizle
            if (masterVolumeSlider != null)
                masterVolumeSlider.onValueChanged.RemoveAllListeners();
            if (musicVolumeSlider != null)
                musicVolumeSlider.onValueChanged.RemoveAllListeners();
            if (sfxVolumeSlider != null)
                sfxVolumeSlider.onValueChanged.RemoveAllListeners();
            if (backButton != null)
                backButton.onClick.RemoveAllListeners();
        }
    }
}

