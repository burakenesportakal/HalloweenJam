using UnityEngine;

namespace HalloweenJam.UI
{
    /// <summary>
    /// Ses yönetim sistemi - Ayarlardan kontrol edilir
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;

        [Header("Audio Clips")]
        [SerializeField] private AudioClip backgroundMusic;
        [SerializeField] private AudioClip alarmSound;

        private float masterVolume = 1f;
        private float musicVolume = 0.8f;
        private float sfxVolume = 1f;

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
            // PlayerPrefs'ten ayarları yükle
            LoadAudioSettings();

            // Arka plan müziğini başlat
            if (backgroundMusic != null && musicSource != null)
            {
                musicSource.clip = backgroundMusic;
                musicSource.loop = true;
                musicSource.Play();
            }
        }

        /// <summary>
        /// Master volume ayarlar
        /// </summary>
        public void SetMasterVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);
            PlayerPrefs.SetFloat("MasterVolume", masterVolume);
            PlayerPrefs.Save();
            UpdateAudioVolumes();
        }

        /// <summary>
        /// Müzik volume ayarlar
        /// </summary>
        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            PlayerPrefs.SetFloat("MusicVolume", musicVolume);
            PlayerPrefs.Save();
            UpdateAudioVolumes();
        }

        /// <summary>
        /// SFX volume ayarlar
        /// </summary>
        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
            PlayerPrefs.Save();
            UpdateAudioVolumes();
        }

        private void UpdateAudioVolumes()
        {
            // Müzik volume = master * music
            if (musicSource != null)
                musicSource.volume = masterVolume * musicVolume;

            // SFX volume = master * sfx
            if (sfxSource != null)
                sfxSource.volume = masterVolume * sfxVolume;
        }

        private void LoadAudioSettings()
        {
            masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
            musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.8f);
            sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
            UpdateAudioVolumes();
        }

        /// <summary>
        /// Alarm sesini çalar
        /// </summary>
        public void PlayAlarmSound()
        {
            if (alarmSound != null && sfxSource != null)
            {
                sfxSource.PlayOneShot(alarmSound);
            }
        }

        /// <summary>
        /// SFX çalar
        /// </summary>
        public void PlaySFX(AudioClip clip)
        {
            if (clip != null && sfxSource != null)
            {
                sfxSource.PlayOneShot(clip);
            }
        }

        public float GetMasterVolume() => masterVolume;
        public float GetMusicVolume() => musicVolume;
        public float GetSFXVolume() => sfxVolume;
    }
}

