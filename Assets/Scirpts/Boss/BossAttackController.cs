using UnityEngine;

namespace HalloweenJam.Boss
{
    /// <summary>
    /// Boss saldırı sistemi - Burst saldırı pattern'i (5-6 mermi, dur, tekrar)
    /// </summary>
    public class BossAttackController : MonoBehaviour
    {
        [Header("Attack Settings")]
        [SerializeField] private int bulletsPerBurst = 6;
        [SerializeField] private float timeBetweenBullets = 0.1f;
        [SerializeField] private float attackCooldown = 1.5f; // Saldırılar arası bekleme
        [SerializeField] private float attackRange = 15f;

        [Header("Projectile Settings")]
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private Transform firePoint; // Mermi çıkış noktası (silah ucundan mermi çıkar)
        [SerializeField] private float bulletSpeed = 20f;

        [Header("Spread Settings")]
        [SerializeField] private float spreadAngle = 30f; // Üçgen alan açısı (derece)
        [SerializeField] private bool useRandomSpread = true; // Rastgele yayılım

        [Header("Aiming")]
        [SerializeField] private bool useAimPoint = false; // AimPoint kullanılsın mı? (genelde false - boss'un kendisi döner)
        [SerializeField] private Transform aimPoint; // Boss'un player'a doğru dönecek kısmı (sadece silah ayrıysa kullan)
        [SerializeField] private float aimSmoothing = 5f;

        private Transform playerTarget;
        private System.Action onAttackComplete;

        private bool isAttacking = false;
        private int bulletsShot = 0;
        private float lastBulletTime = 0f;
        private float lastAttackTime = 0f;
        private bool canAttack = true;

        public void Initialize(Transform player, System.Action onComplete)
        {
            playerTarget = player;
            onAttackComplete = onComplete;

            // Fire point kontrolü
            if (firePoint == null)
            {
                firePoint = new GameObject("FirePoint").transform;
                firePoint.SetParent(transform);
                firePoint.localPosition = Vector3.forward;
            }

            // Aim point kontrolü (sadece kullanılıyorsa)
            if (!useAimPoint)
                aimPoint = null;
        }

        public void UpdateAttack()
        {
            if (!isAttacking || playerTarget == null)
                return;

            // Player'a doğru hedef al
            AimAtPlayer();

            // Burst saldırısını yönet
            if (Time.time >= lastBulletTime + timeBetweenBullets)
            {
                ShootBullet();
                bulletsShot++;

                if (bulletsShot >= bulletsPerBurst)
                {
                    // Burst tamamlandı
                    CompleteAttack();
                }
                else
                {
                    lastBulletTime = Time.time;
                }
            }
        }

        public bool CanAttack()
        {
            if (!canAttack)
                return false;

            if (Time.time < lastAttackTime + attackCooldown)
                return false;

            if (playerTarget == null)
                return false;

            // Player menzilde mi?
            float distance = Vector2.Distance(transform.position, playerTarget.position);
            if (distance > attackRange)
                return false;

            return true;
        }

        public void StartAttack()
        {
            if (isAttacking || !CanAttack())
                return;

            isAttacking = true;
            bulletsShot = 0;
            lastBulletTime = Time.time;
            lastAttackTime = Time.time;
            canAttack = false;
        }

        public void StopAttack()
        {
            isAttacking = false;
            bulletsShot = 0;
        }

        private void AimAtPlayer()
        {
            if (playerTarget == null)
                return;

            // Boss'un kendisi player'a doğru döner (silah karakterle bütün)
            Vector2 direction = (playerTarget.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Smooth rotation (tüm boss döner)
            Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, aimSmoothing * Time.deltaTime);

            // Eğer aim point kullanılıyorsa (silah ayrıysa - nadir durum)
            if (useAimPoint && aimPoint != null)
            {
                aimPoint.rotation = Quaternion.Slerp(aimPoint.rotation, targetRotation, aimSmoothing * Time.deltaTime);
            }
        }

        private void ShootBullet()
        {
            if (bulletPrefab == null || firePoint == null)
            {
                Debug.LogWarning("BossAttackController: Bullet prefab veya fire point atanmamış!");
                return;
            }

            // Ana yön (player'a doğru)
            Vector2 baseDirection = (playerTarget.position - firePoint.position).normalized;
            float baseAngle = Mathf.Atan2(baseDirection.y, baseDirection.x) * Mathf.Rad2Deg;

            // Spread açısı (üçgen alan - rastgele)
            float spread = 0f;
            if (useRandomSpread)
            {
                // Rastgele açı (-spreadAngle/2 ile +spreadAngle/2 arası)
                spread = Random.Range(-spreadAngle / 2f, spreadAngle / 2f);
            }
            else
            {
                // Düzgün dağılım (burst içinde eşit açılarla)
                float angleStep = spreadAngle / (bulletsPerBurst - 1);
                spread = -spreadAngle / 2f + (bulletsShot * angleStep);
            }

            // Final açı
            float finalAngle = baseAngle + spread;
            Vector2 direction = new Vector2(Mathf.Cos(finalAngle * Mathf.Deg2Rad), Mathf.Sin(finalAngle * Mathf.Deg2Rad));

            // Mermi oluştur
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            
            // Mermi yönü (spread açısıyla)
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            if (bulletRb != null)
            {
                // Unity 6 için linearVelocity kullanıyoruz
                bulletRb.linearVelocity = direction * bulletSpeed;
            }

            // Rotasyon (merminin görsel yönü)
            bullet.transform.rotation = Quaternion.AngleAxis(finalAngle, Vector3.forward);

            // Mermi ayarları (damage, tag vb.) - projenize göre ayarlayın
            // bullet.tag = "BossBullet";
            // bullet.layer = LayerMask.NameToLayer("BossProjectiles");
        }

        private void CompleteAttack()
        {
            isAttacking = false;
            bulletsShot = 0;
            
            // Cooldown başlat
            StartCoroutine(AttackCooldownCoroutine());
        }

        private System.Collections.IEnumerator AttackCooldownCoroutine()
        {
            yield return new WaitForSeconds(attackCooldown);
            canAttack = true;

            // Saldırı tamamlandı, callback çağır
            onAttackComplete?.Invoke();
        }
    }
}

