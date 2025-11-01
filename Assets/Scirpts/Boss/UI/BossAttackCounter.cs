using UnityEngine;
using TMPro;

namespace HalloweenJam.Boss
{
    /// <summary>
    /// Boss saldırı sayacı UI (opsiyonel - 4 saldırıdan sonra yorulma gösterimi)
    /// </summary>
    public class BossAttackCounter : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI attackCountText;
        [SerializeField] private int maxAttacks = 4;

        public void UpdateAttackCount(int currentAttacks)
        {
            if (attackCountText != null)
            {
                attackCountText.text = $"Saldırı: {currentAttacks}/{maxAttacks}";
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}

