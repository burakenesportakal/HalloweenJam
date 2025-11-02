using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace HalloweenJam.UI
{
    /// <summary>
    /// Health UI - Sol üstte her zaman görünür can ikonları (Hollow Knight maskeleri gibi)
    /// </summary>
    public class HealthUI : MonoBehaviour
    {
        [Header("Health Icons")]
        [SerializeField] private Image[] healthIcons; // HP ikonları (kalpler/maskeler) - Max 3 tane

        [Header("Health Icon Settings")]
        [SerializeField] private Sprite fullHeartSprite; // Dolu kalp sprite'ı
        [SerializeField] private Sprite emptyHeartSprite; // Boş kalp sprite'ı (opsiyonel)
        [SerializeField] private Color fullHeartColor = Color.white;
        [SerializeField] private Color emptyHeartColor = new Color(0.5f, 0.5f, 0.5f, 0.5f); // Gri, yarı saydam

        [Header("Initial Health Settings")]
        [SerializeField] private int initialHealth = 3; // Başlangıç can değeri

        private int currentHealth = 3;
        private int maxHealth = 3;

        private void Start()
        {
            // Health icons array kontrolü
            if (healthIcons == null || healthIcons.Length == 0)
            {
                Debug.LogWarning("HealthUI: Health icons array atanmamış! Inspector'da healthIcons array'ini doldurun.");
                return;
            }

            // Max health'i array uzunluğuna göre ayarla (max 3)
            maxHealth = Mathf.Clamp(healthIcons.Length, 1, 3);
            
            // Initial health'i max health'e göre ayarla
            initialHealth = Mathf.Clamp(initialHealth, 0, maxHealth);
            currentHealth = initialHealth;

            // Başlangıç sağlık değerlerini ayarla
            UpdateHealth(currentHealth, maxHealth);
        }

        /// <summary>
        /// Can ikonlarını günceller (Hollow Knight maskeleri gibi)
        /// </summary>
        public void UpdateHealth(int current, int max)
        {
            currentHealth = Mathf.Clamp(current, 0, max);
            maxHealth = Mathf.Clamp(max, 1, 3); // Max 3 kalp (Hollow Knight gibi)

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

