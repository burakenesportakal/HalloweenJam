using UnityEngine;

namespace HalloweenJam.Boss
{
    /// <summary>
    /// Boss ele geçirme sistemi - 3 başarısız denemeden sonra ele geçirilebilir
    /// </summary>
    public class BossTakeoverSystem : MonoBehaviour
    {
        [Header("Takeover Settings")]
        [SerializeField] private float takeoverRange = 1.5f; // Ele geçirme mesafesi
        [SerializeField] private KeyCode takeoverKey = KeyCode.X;
        [SerializeField] private int failedAttemptsNeeded = 3;

        [Header("UI References")]
        [SerializeField] private BossTakeoverPrompt takeoverPrompt;

        private BossController bossController;
        private Transform playerTarget;

        private int failedAttempts = 0;
        private bool canTakeover = false;
        private bool isInRange = false;

        private System.Action<bool> onTakeoverAttempted;
        private System.Action onBossTakenOver;

        public void Initialize(System.Action<bool> onAttempted, System.Action onTakenOver)
        {
            onTakeoverAttempted = onAttempted;
            onBossTakenOver = onTakenOver;

            bossController = GetComponent<BossController>();
        }

        public void UpdateTakeover(Transform player)
        {
            playerTarget = player;

            if (!bossController.IsFatigued())
            {
                // Boss yorgun değilse ele geçirme mümkün değil
                if (takeoverPrompt != null)
                    takeoverPrompt.Hide();
                return;
            }

            // Mesafe kontrolü
            float distance = Vector2.Distance(transform.position, player.position);
            isInRange = distance <= takeoverRange;

            // UI güncelle
            if (takeoverPrompt != null)
            {
                if (isInRange && canTakeover)
                {
                    takeoverPrompt.Show("X - ELE GEÇİR");
                }
                else if (isInRange && !canTakeover)
                {
                    takeoverPrompt.Show($"DENEME: {failedAttempts}/{failedAttemptsNeeded}");
                }
                else
                {
                    takeoverPrompt.Hide();
                }
            }

            // Ele geçirme input kontrolü
            if (isInRange && Input.GetKeyDown(takeoverKey))
            {
                AttemptTakeover();
            }
        }

        private void AttemptTakeover()
        {
            if (!bossController.IsFatigued())
                return;

            if (canTakeover)
            {
                // Başarılı ele geçirme!
                SuccessTakeover();
            }
            else
            {
                // Başarısız deneme
                FailedTakeover();
            }
        }

        private void SuccessTakeover()
        {
            // Boss ele geçirildi!
            failedAttempts = 0;
            canTakeover = false;

            if (takeoverPrompt != null)
                takeoverPrompt.Hide();

            onBossTakenOver?.Invoke();
        }

        private void FailedTakeover()
        {
            failedAttempts++;

            if (failedAttempts >= failedAttemptsNeeded)
            {
                // Artık ele geçirilebilir!
                canTakeover = true;
            }

            // Callback (başarısız)
            onTakeoverAttempted?.Invoke(false);
        }

        // Collision-based takeover (simbiyot boss'a temas ederse)
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!bossController.IsFatigued())
                return;

            if (collision.CompareTag("Player") || collision.CompareTag("Symbiote"))
            {
                // Player temas etti, ele geçirme denemesi yap
                if (Input.GetKey(takeoverKey))
                {
                    AttemptTakeover();
                }
            }
        }

        public int GetFailedAttempts()
        {
            return failedAttempts;
        }

        public bool CanTakeover()
        {
            return canTakeover && bossController.IsFatigued();
        }

        public void ResetTakeover()
        {
            failedAttempts = 0;
            canTakeover = false;
        }
    }
}

