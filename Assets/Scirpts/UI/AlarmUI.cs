using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace HalloweenJam.UI
{
    /// <summary>
    /// Alarm UI - Alarm çaldığında görünür (Alarm sayacı + Can ikonları - Hollow Knight maskeleri gibi)
    /// Normalde gizlidir, sadece alarm durumunda aktif olur
    /// </summary>
    public class AlarmUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject alarmContainer; // Ana container (göster/gizle için)
        [SerializeField] private TextMeshProUGUI alarmTimerText;
        [SerializeField] private Image[] healthIcons; // HP ikonları (kalpler/maskeler) - Max 3 tane

        [Header("Health Icon Settings")]
        [SerializeField] private Sprite fullHeartSprite; // Dolu kalp sprite'ı
        [SerializeField] private Sprite emptyHeartSprite; // Boş kalp sprite'ı (opsiyonel)
        [SerializeField] private Color fullHeartColor = Color.white;
        [SerializeField] private Color emptyHeartColor = new Color(0.5f, 0.5f, 0.5f, 0.5f); // Gri, yarı saydam

        private int currentHealth = 2;
        private int maxHealth = 2;
        private bool isVisible = false;

        private void Start()
        {
            // Başlangıçta gizli
            Hide();

            // Başlangıç sağlık değerlerini ayarla
            UpdateHealth(maxHealth, maxHealth);
            
            // Health icons array kontrolü
            if (healthIcons == null || healthIcons.Length == 0)
            {
                Debug.LogWarning("AlarmUI: Health icons array atanmamış! Inspector'da healthIcons array'ini doldurun.");
            }
        }

        /// <summary>
        /// Alarm UI'ı gösterir
        /// </summary>
        public void Show()
        {
            isVisible = true;
            if (alarmContainer != null)
                alarmContainer.SetActive(true);
        }

        /// <summary>
        /// Alarm UI'ı gizler
        /// </summary>
        public void Hide()
        {
            isVisible = false;
            if (alarmContainer != null)
                alarmContainer.SetActive(false);
        }

        /// <summary>
        /// Alarm sayacını günceller
        /// </summary>
        public void UpdateTimer(float remainingTime)
        {
            if (!isVisible) return;

            if (alarmTimerText != null)
            {
                // Geri sayım formatı: "ALARM: 5"
                int seconds = Mathf.CeilToInt(remainingTime);
                alarmTimerText.text = $"ALARM: {seconds}";
            }
        }

        /// <summary>
        /// Can ikonlarını günceller (Hollow Knight maskeleri gibi)
        /// </summary>
        public void UpdateHealth(int current, int max)
        {
            currentHealth = Mathf.Clamp(current, 0, max);
            maxHealth = Mathf.Clamp(max, 1, 3); // Max 3 kalp (Hollow Knight gibi)

            if (!isVisible) return;

            // Health icons güncelle
            if (healthIcons != null && healthIcons.Length > 0)
            {
                for (int i = 0; i < healthIcons.Length && i < maxHealth; i++)
                {
                    if (healthIcons[i] != null)
                    {
                        // İkonu göster
                        healthIcons[i].gameObject.SetActive(true);
                        
                        // Dolu mu boş mu?
                        if (i < currentHealth)
                        {
                            // Dolu kalp
                            if (fullHeartSprite != null)
                                healthIcons[i].sprite = fullHeartSprite;
                            healthIcons[i].color = fullHeartColor;
                        }
                        else
                        {
                            // Boş kalp
                            if (emptyHeartSprite != null)
                                healthIcons[i].sprite = emptyHeartSprite;
                            healthIcons[i].color = emptyHeartColor;
                        }
                    }
                }
                
                // Fazla ikonları gizle (eğer max health 3'ten azsa)
                for (int i = maxHealth; i < healthIcons.Length; i++)
                {
                    if (healthIcons[i] != null)
                        healthIcons[i].gameObject.SetActive(false);
                }
            }

            // Ölüm kontrolü
            if (currentHealth <= 0)
            {
                OnHealthDepleted();
            }
        }

        private void OnHealthDepleted()
        {
            Debug.Log("Health depleted! Game Over.");
            // GameManager'a bildir (oyunu baştan başlatmak için)
            if (GameManager.Instance != null)
                GameManager.Instance.OnPlayerDeath();
        }
    }
}

