using UnityEngine;

namespace HalloweenJam.Boss
{
    /// <summary>
    /// Boss yorulma sistemi - 4 saldırıdan sonra 5 saniye yorulma
    /// </summary>
    public class BossFatigueSystem : MonoBehaviour
    {
        [Header("Fatigue Settings")]
        [SerializeField] private int attacksUntilFatigue = 4;
        [SerializeField] private float fatigueDuration = 5f;

        [Header("UI References")]
        [SerializeField] private BossFatigueIndicator fatigueIndicator;

        private int attackCount = 0;
        private bool isFatigued = false;
        private float fatigueStartTime = 0f;

        private System.Action onBossFatigued;
        private System.Action onFatigueEnded;

        public void Initialize(System.Action onFatigued, System.Action onEnded)
        {
            onBossFatigued = onFatigued;
            onFatigueEnded = onEnded;
            attackCount = 0;
            isFatigued = false;
        }

        public void UpdateFatigue()
        {
            if (isFatigued)
            {
                // Yorulma süresi kontrolü
                float elapsedTime = Time.time - fatigueStartTime;
                
                if (elapsedTime >= fatigueDuration)
                {
                    // Yorulma bitti
                    EndFatigue();
                }
                else
                {
                    // UI güncelle
                    if (fatigueIndicator != null)
                    {
                        float remainingTime = fatigueDuration - elapsedTime;
                        fatigueIndicator.UpdateFatigueTimer(remainingTime);
                    }
                }
            }
        }

        public void OnAttackCompleted()
        {
            if (isFatigued)
                return; // Zaten yorgunsa sayma

            attackCount++;

            if (attackCount >= attacksUntilFatigue)
            {
                // Boss yoruldu!
                StartFatigue();
            }
        }

        public void OnTakeoverFailed()
        {
            // Başarısız ele geçirme - reset
            attackCount = 0;
            EndFatigue();
        }

        private void StartFatigue()
        {
            isFatigued = true;
            fatigueStartTime = Time.time;
            attackCount = 0; // Reset

            // UI göster
            if (fatigueIndicator != null)
                fatigueIndicator.ShowFatigue();

            // Callback
            onBossFatigued?.Invoke();
        }

        private void EndFatigue()
        {
            isFatigued = false;
            fatigueStartTime = 0f;

            // UI gizle
            if (fatigueIndicator != null)
                fatigueIndicator.HideFatigue();

            // Callback
            onFatigueEnded?.Invoke();
        }

        public void Stop()
        {
            isFatigued = false;
            attackCount = 0;
            if (fatigueIndicator != null)
                fatigueIndicator.HideFatigue();
        }

        public bool IsFatigued()
        {
            return isFatigued;
        }

        public float GetFatigueRemainingTime()
        {
            if (!isFatigued)
                return 0f;

            float elapsed = Time.time - fatigueStartTime;
            return Mathf.Max(0f, fatigueDuration - elapsed);
        }
    }
}

