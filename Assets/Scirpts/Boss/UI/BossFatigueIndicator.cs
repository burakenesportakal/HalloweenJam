using UnityEngine;
using TMPro;

namespace HalloweenJam.Boss
{
    /// <summary>
    /// Boss yorulma göstergesi UI
    /// </summary>
    public class BossFatigueIndicator : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject fatiguePanel;
        [SerializeField] private TextMeshProUGUI fatigueTimerText;

        private void Start()
        {
            if (fatiguePanel != null)
                fatiguePanel.SetActive(false);
        }

        public void ShowFatigue()
        {
            if (fatiguePanel != null)
                fatiguePanel.SetActive(true);
        }

        public void HideFatigue()
        {
            if (fatiguePanel != null)
                fatiguePanel.SetActive(false);
        }

        public void UpdateFatigueTimer(float remainingTime)
        {
            if (fatigueTimerText != null)
            {
                int seconds = Mathf.CeilToInt(remainingTime);
                fatigueTimerText.text = $"Boss Yorgun! {seconds}s";
            }
        }
    }
}

