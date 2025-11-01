using UnityEngine;

namespace HalloweenJam.Boss
{
    /// <summary>
    /// Boss'un ana koordinatör sistemi - Tüm boss sistemlerini yönetir
    /// </summary>
    public class BossController : MonoBehaviour
    {
        [Header("Sub Systems")]
        [SerializeField] private BossMovementController movementController;
        [SerializeField] private BossAttackController attackController;
        [SerializeField] private BossFatigueSystem fatigueSystem;
        [SerializeField] private BossTakeoverSystem takeoverSystem;

        [Header("References")]
        [SerializeField] private Transform playerTarget; // Player referansı
        [SerializeField] private Collider2D bossCollider; // Collider toggle için

        [Header("Boss Settings")]
        [SerializeField] private float detectionRange = 10f;
        [SerializeField] private LayerMask playerLayer;

        public enum BossState
        {
            Patrolling,     // İleri-geri gidip gelme
            Chasing,        // Player'ı takip etme (gerekiyorsa)
            Attacking,      // Saldırı modunda
            Fatigued,       // Yoruldu, ele geçirme mümkün
            TakenOver       // Ele geçirildi
        }

        public BossState CurrentState { get; private set; } = BossState.Patrolling;

        private void Awake()
        {
            // Sub-system referanslarını kontrol et
            if (movementController == null)
                movementController = GetComponent<BossMovementController>();
            
            if (attackController == null)
                attackController = GetComponent<BossAttackController>();
            
            if (fatigueSystem == null)
                fatigueSystem = GetComponent<BossFatigueSystem>();
            
            if (takeoverSystem == null)
                takeoverSystem = GetComponent<BossTakeoverSystem>();

            // Collider kontrolü
            if (bossCollider == null)
                bossCollider = GetComponent<Collider2D>();

            // Player bulma (tag ile)
            if (playerTarget == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                    playerTarget = player.transform;
            }
        }

        private void Start()
        {
            InitializeBoss();
        }

        private void Update()
        {
            if (CurrentState == BossState.TakenOver)
                return;

            UpdateBossState();
            UpdateSubSystems();
        }

        private void InitializeBoss()
        {
            // Başlangıç ayarları
            SetBossState(BossState.Patrolling);
            
            // Collider ayarı (başlangıçta trigger - simbiyot içinden geçebilir)
            if (bossCollider != null)
                bossCollider.isTrigger = true;

            // Sub-system'leri başlat
            if (movementController != null)
                movementController.Initialize(playerTarget);

            if (attackController != null)
                attackController.Initialize(playerTarget, OnAttackComplete);

            if (fatigueSystem != null)
                fatigueSystem.Initialize(OnBossFatigued, OnFatigueEnded);

            if (takeoverSystem != null)
                takeoverSystem.Initialize(OnTakeoverAttempted, OnBossTakenOver);
        }

        private void UpdateBossState()
        {
            // Player tespiti
            bool playerInRange = IsPlayerInRange();

            if (CurrentState == BossState.Fatigued)
            {
                // Yorulma durumunda ele geçirme sistemi aktif
                if (takeoverSystem != null)
                    takeoverSystem.UpdateTakeover(playerTarget);
                return;
            }

            if (playerInRange && CurrentState != BossState.Attacking)
            {
                // Saldırı kontrolü
                if (attackController != null && attackController.CanAttack())
                {
                    SetBossState(BossState.Attacking);
                    attackController.StartAttack();
                }
                else if (CurrentState != BossState.Attacking)
                {
                    // Hareket modu
                    SetBossState(BossState.Patrolling);
                }
            }
            else
            {
                SetBossState(BossState.Patrolling);
            }
        }

        private void UpdateSubSystems()
        {
            if (CurrentState == BossState.Attacking && attackController != null)
            {
                attackController.UpdateAttack();
            }
            else if (CurrentState == BossState.Patrolling && movementController != null)
            {
                movementController.UpdateMovement();
            }

            // Fatigue sistemini sürekli güncelle
            if (fatigueSystem != null)
                fatigueSystem.UpdateFatigue();
        }

        private bool IsPlayerInRange()
        {
            if (playerTarget == null)
                return false;

            float distance = Vector2.Distance(transform.position, playerTarget.position);
            return distance <= detectionRange;
        }

        private void SetBossState(BossState newState)
        {
            if (CurrentState == newState)
                return;

            BossState previousState = CurrentState;
            CurrentState = newState;

            OnStateChanged(previousState, newState);
        }

        private void OnStateChanged(BossState previousState, BossState newState)
        {
            // State değişikliklerinde yapılacak işlemler
            switch (newState)
            {
                case BossState.Attacking:
                    if (movementController != null)
                        movementController.StopMovement();
                    break;

                case BossState.Patrolling:
                    if (attackController != null)
                        attackController.StopAttack();
                    if (movementController != null)
                        movementController.StartMovement();
                    break;

                case BossState.Fatigued:
                    EnableTakeoverCollision();
                    break;
            }
        }

        private void OnAttackComplete()
        {
            // Saldırı bitti, hareket moduna geç
            if (CurrentState == BossState.Attacking)
            {
                SetBossState(BossState.Patrolling);

                // Saldırı sayısını artır (fatigue sistemine bildir)
                if (fatigueSystem != null)
                    fatigueSystem.OnAttackCompleted();
            }
        }

        private void OnBossFatigued()
        {
            // Boss yoruldu, ele geçirme mümkün
            SetBossState(BossState.Fatigued);
            EnableTakeoverCollision();

            // Hareket ve saldırıyı durdur
            if (movementController != null)
                movementController.StopMovement();
            if (attackController != null)
                attackController.StopAttack();
        }

        private void OnFatigueEnded()
        {
            // Yorulma bitti, normal moda dön
            DisableTakeoverCollision();

            if (CurrentState == BossState.Fatigued)
            {
                SetBossState(BossState.Patrolling);
            }
        }

        private void OnTakeoverAttempted(bool successful)
        {
            if (!successful)
            {
                // Başarısız ele geçirme - fatigue sistemi resetlenecek
                if (fatigueSystem != null)
                    fatigueSystem.OnTakeoverFailed();
            }
        }

        private void OnBossTakenOver()
        {
            // Boss ele geçirildi!
            SetBossState(BossState.TakenOver);
            
            // Tüm sistemleri durdur
            if (movementController != null)
                movementController.StopMovement();
            if (attackController != null)
                attackController.StopAttack();
            if (fatigueSystem != null)
                fatigueSystem.Stop();

            // Collider'ı açık tut (artık normal collider)
            if (bossCollider != null)
                bossCollider.isTrigger = false;

            // Win condition'a bildir
            if (BossWinHandler.Instance != null)
                BossWinHandler.Instance.OnBossTakenOver();
        }

        private void EnableTakeoverCollision()
        {
            // Collider'ı normal yap (ele geçirme teması mümkün)
            if (bossCollider != null)
                bossCollider.isTrigger = false;
        }

        private void DisableTakeoverCollision()
        {
            // Collider'ı trigger yap (simbiyot içinden geçebilir)
            if (bossCollider != null)
                bossCollider.isTrigger = true;
        }

        public Transform GetPlayerTarget()
        {
            return playerTarget;
        }

        public bool IsFatigued()
        {
            return CurrentState == BossState.Fatigued;
        }

        public bool IsTakenOver()
        {
            return CurrentState == BossState.TakenOver;
        }
    }
}

