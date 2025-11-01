using UnityEngine;
using TMPro;

namespace HalloweenJam.Boss
{
    /// <summary>
    /// Boss ele geçirme prompt UI (X tuşuna bas gösterimi)
    /// </summary>
    public class BossTakeoverPrompt : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject promptPanel;
        [SerializeField] private TextMeshProUGUI promptText;

        private void Start()
        {
            if (promptPanel != null)
                promptPanel.SetActive(false);
        }

        public void Show(string text)
        {
            if (promptPanel != null)
                promptPanel.SetActive(true);

            if (promptText != null)
                promptText.text = text;
        }

        public void Hide()
        {
            if (promptPanel != null)
                promptPanel.SetActive(false);
        }
    }
}

