using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace HalloweenJam.Boss
{
    /// <summary>
    /// Kapı can barı UI
    /// </summary>
    public class DoorHealthBar : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject healthBarPanel;
        [SerializeField] private Image healthFillImage;
        [SerializeField] private TextMeshProUGUI healthText;

        private void Start()
        {
            if (healthBarPanel != null)
                healthBarPanel.SetActive(true);
        }

        public void UpdateHealth(float healthPercent)
        {
            healthPercent = Mathf.Clamp01(healthPercent);

            if (healthFillImage != null)
                healthFillImage.fillAmount = healthPercent;

            if (healthText != null)
            {
                int percent = Mathf.RoundToInt(healthPercent * 100);
                healthText.text = $"Kapı: {percent}%";
            }
        }

        public void Hide()
        {
            if (healthBarPanel != null)
                healthBarPanel.SetActive(false);
        }
    }
}

