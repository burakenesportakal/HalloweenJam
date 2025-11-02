using UnityEngine;

public class Enemy : Entity
{
    [Header("Movement")]
    [SerializeField] protected float moveSpeed = 3f;
    [SerializeField] protected float patrolRange = 5f;
    [SerializeField] protected Transform patrolPointA;
    [SerializeField] protected Transform patrolPointB;
    private Vector3 startPosition;
    private bool movingToB = true;

    [Header("Player Detection")]
    [SerializeField] protected float detectionRange = 8f;
    [SerializeField] protected float attackRange = 6f;
    [SerializeField] protected LayerMask playerLayer;
    [SerializeField] protected LayerMask enemyLayer;
    [SerializeField] protected Transform detectionPoint;
    private PlayerController detectedPlayer = null;
    private Enemy detectedEnemy = null;
    private bool isControlled = false;
    private PlayerController controllingPlayer = null;
    private float loseTargetRange = 15f; // Player/Enemy çok uzaktaysa takibi bırak
    private bool hasAttackedSameType = false; // Kontrol edilen enemy aynı tipteki bir enemy'ye saldırdı mı?

    [Header("Attack")]
    [SerializeField] protected GameObject projectilePrefab;
    [SerializeField] protected Transform firePoint;
    [SerializeField] protected float projectileSpeed = 10f;
    [SerializeField] private float attackCooldown = 2f;
    private float lastAttackTime = 0f;
    private bool isAttacking = false;

    [Header("Player Attachment")]
    [SerializeField] protected Transform attachmentPoint; // Player'ın yapışacağı sabit nokta

    [Header("Enemy Type")]
    [SerializeField] protected int enemyType = 0; // 0, 1, 2 - farklı renkler ve güçler için

    [Header("Ground Check")]
    [SerializeField] protected LayerMask groundLayer;
    [SerializeField] protected float groundCheckDistance = 0.2f;
    [SerializeField] protected Transform groundCheckPoint;
    protected bool isGrounded = false;

    protected override void Awake()
    {
        base.Awake();
        startPosition = transform.position;
    }

    protected override void Start()
    {
        base.Start();

        // Patrol point'ler yoksa oluştur
        if (patrolPointA == null || patrolPointB == null)
        {
            CreatePatrolPoints();
        }
    }

    protected override void Update()
    {
        base.Update();

        // Ölü enemy hiçbir şey yapmaz
        if (isDead) return;

        // Kontrol ediliyorsa AI çalışmasın
        if (isControlled)
        {
            return;
        }

        CheckGround();
        HandleTargetDetection();

        // Hedef detect edildiyse saldır (player veya farklı tipte enemy)
        if ((detectedPlayer != null && !detectedPlayer.IsDead()) || (detectedEnemy != null && !detectedEnemy.IsDead()))
        {
            HandleRangedAttack();
        }
        else
        {
            HandlePatrol();
        }

        // Animasyonları güncelle (AI modunda)
        UpdateAnimations();
    }

    private void CreatePatrolPoints()
    {
        GameObject pointA = new GameObject("PatrolPointA");
        pointA.transform.position = startPosition + Vector3.left * patrolRange;
        pointA.transform.SetParent(transform.parent);
        patrolPointA = pointA.transform;

        GameObject pointB = new GameObject("PatrolPointB");
        pointB.transform.position = startPosition + Vector3.right * patrolRange;
        pointB.transform.SetParent(transform.parent);
        patrolPointB = pointB.transform;
    }

    protected virtual void CheckGround()
    {
        if (groundCheckPoint == null) return;

        isGrounded = Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckDistance, groundLayer);
    }

    protected virtual void HandlePatrol()
    {
        if (patrolPointA == null || patrolPointB == null) return;

        // Patrol mantığı - sadece A ve B noktaları arasında sürekli gezin
        Vector3 targetPoint = movingToB ? patrolPointB.position : patrolPointA.position;
        Vector2 direction = (targetPoint - transform.position);

        // Hedefe yakınsa yön değiştir
        if (direction.magnitude < 0.5f)
        {
            movingToB = !movingToB;
            // Yeni hedefi al
            targetPoint = movingToB ? patrolPointB.position : patrolPointA.position;
            direction = (targetPoint - transform.position);
        }

        direction.Normalize();

        // Hareket et - sürekli hareket etmeli
        if (isGrounded && !isAttacking)
        {
            SetVelocity(direction.x * moveSpeed, rb.linearVelocity.y);
            HandleFlip(direction.x);
        }

        // Animasyon
        if (anim != null)
        {
            anim.SetBool("isMoving", Mathf.Abs(direction.x) > 0.1f && !isAttacking);
        }
    }

    protected virtual void HandleTargetDetection()
    {
        if (detectionPoint == null) return;

        // Eğer zaten bir hedef varsa, önce onun hala geçerli olup olmadığını kontrol et
        if (detectedPlayer != null && !detectedPlayer.IsDead())
        {
            // ÖNEMLİ: Player gizlenmişse takibi bırak
            if (detectedPlayer.IsHidden())
            {
                detectedPlayer = null;
                return;
            }
            
            float distanceToPlayer = Vector2.Distance(transform.position, detectedPlayer.transform.position);
            // Eğer player çok uzaktaysa (loseTargetRange dışındaysa) takibi bırak
            // Ama eğer hala detectionRange içindeyse takip etmeye devam et
            if (distanceToPlayer > loseTargetRange)
            {
                detectedPlayer = null;
            }
            else
            {
                // Hala takip ediyoruz, player'a dön (arkasına geçse bile)
                return;
            }
        }

        if (detectedEnemy != null && !detectedEnemy.IsDead())
        {
            float distanceToEnemy = Vector2.Distance(transform.position, detectedEnemy.transform.position);
            // Eğer enemy çok uzaktaysa takibi bırak
            if (distanceToEnemy > loseTargetRange)
            {
                detectedEnemy = null;
            }
            else
            {
                // Hala takip ediyoruz, enemy'ye dön
                return;
            }
        }

        // Yeni hedef ara - hem player hem enemy kontrol et
        PlayerController foundPlayer = null;
        Enemy foundEnemy = null;
        float closestPlayerDistance = float.MaxValue;
        float closestEnemyDistance = float.MaxValue;

        // Player'ları kontrol et - sadece önündeki player'ı gör (ilk tespit için)
        // Bir kez tespit edildiyse detectionRange içinde her zaman takip edilir
        Collider2D[] nearbyPlayers = Physics2D.OverlapCircleAll(detectionPoint.position, detectionRange, playerLayer);
        foreach (var col in nearbyPlayers)
        {
            PlayerController player = col.GetComponent<PlayerController>();
            if (player == null || player.IsDead()) continue;
            
            // ÖNEMLİ: Gizlenmiş player'ı göremez
            if (player.IsHidden()) continue;

            Vector2 toPlayer = (player.transform.position - transform.position);
            float distance = toPlayer.magnitude;
            if (distance > detectionRange) continue;

            toPlayer.Normalize();

            // İlk tespitte sadece önündeki player'ı gör (arkası dönükse göremez)
            // dotProduct > 0 = player enemy'nin önünde
            float dotProduct = toPlayer.x * facingDirection;

            if (dotProduct > 0 && distance < closestPlayerDistance)
            {
                foundPlayer = player;
                closestPlayerDistance = distance;
            }
        }

        // Farklı tipte enemy'leri kontrol et
        Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(detectionPoint.position, detectionRange, enemyLayer);
        foreach (var col in nearbyEnemies)
        {
            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy == null || enemy.IsDead() || enemy == this) continue;

            // Kontrol edilen enemy kontrolü
            if (enemy.IsControlled())
            {
                // Eğer kontrol edilen enemy aynı tipteyse ve aynı tipteki enemy'ye saldırmamışsa, saldırma
                if (enemy.GetEnemyType() == enemyType && !enemy.HasAttackedSameType())
                {
                    continue; // Aynı tipteki kontrol edilen enemy'ye saldırma (saldırmadığı sürece)
                }
                // Farklı tipteki kontrol edilen enemy'ye saldırabilir veya aynı tipteki ama saldırmış olan enemy'ye saldırabilir
            }

            if (enemy.GetEnemyType() == enemyType) continue; // Aynı tipte enemy'ye saldırma (kontrol edilmeyen)

            Vector2 toEnemy = (enemy.transform.position - transform.position);
            float distance = toEnemy.magnitude;
            if (distance > detectionRange) continue;

            // Farklı tipte enemy'ler birbirlerini her yönden görebilir
            if (distance < closestEnemyDistance)
            {
                foundEnemy = enemy;
                closestEnemyDistance = distance;
            }
        }

        // Hedef seçimi: Player varsa öncelikli, yoksa enemy
        if (foundPlayer != null)
        {
            detectedPlayer = foundPlayer;
            detectedEnemy = null;
        }
        else if (foundEnemy != null)
        {
            detectedEnemy = foundEnemy;
            detectedPlayer = null;
        }
    }

    protected virtual void HandleRangedAttack()
    {
        // Hedef belirle (player veya enemy)
        Transform target = null;
        if (detectedPlayer != null && !detectedPlayer.IsDead() && !detectedPlayer.IsHidden())
        {
            target = detectedPlayer.transform;
        }
        else if (detectedEnemy != null && !detectedEnemy.IsDead())
        {
            target = detectedEnemy.transform;
        }

        if (target == null)
        {
            // Hedef yoksa (player gizlendiyse) attack durdur
            isAttacking = false;
            return;
        }

        float distanceToTarget = Vector2.Distance(transform.position, target.position);
        
        // Hedefe yönel (hem attack range içinde hem dışında)
        Vector2 directionToTarget = (target.position - transform.position).normalized;
        
        // ÖNEMLİ: Her zaman hedefe dön (attack range içinde olsa bile)
        // Bu sayede enemy arkasına bakıp önüne ateş etmez
        HandleFlip(directionToTarget.x);

        // Hedef attack range içindeyse saldır ve dur
        if (distanceToTarget <= attackRange)
        {
            // Attack range içindeyken dur
            SetVelocity(0, rb.linearVelocity.y);
            
            // Animasyon
            if (anim != null)
            {
                anim.SetBool("isMoving", false);
            }
            
            // Ateş etme kontrolü (cooldown kontrolü FireProjectile içinde)
            // NOT: isAttacking flag'i FireProjectile içinde set ediliyor (gerçekten ateş edildiyse)
            bool didFire = FireProjectile();
            
            // Eğer ateş edildiyse isAttacking true yap (aksi halde false kalır, takılı kalmaz)
            if (!didFire)
            {
                isAttacking = false; // Cooldown bitmediyse takılı kalma
            }
        }
        else
        {
            // Attack range dışındaysa hedefe doğru takip et
            if (isGrounded && !isAttacking)
            {
                SetVelocity(directionToTarget.x * moveSpeed, rb.linearVelocity.y);
            }

            isAttacking = false;
        }
    }

    public virtual bool FireProjectile()
    {
        if (projectilePrefab == null || firePoint == null) return false;
        if (isDead) return false;

        // Cooldown kontrolü
        if (Time.time < lastAttackTime + attackCooldown) return false;

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        Vector2 direction;

        // Eğer kontrol ediliyorsa, önüne ateş et (facing direction)
        if (isControlled)
        {
            direction = Vector2.right * facingDirection;
        }
        // Eğer kontrol edilmiyorsa, hedefe doğru ateş et (player veya enemy)
        else
        {
            Transform target = null;
            if (detectedPlayer != null && !detectedPlayer.IsDead())
            {
                target = detectedPlayer.transform;
            }
            else if (detectedEnemy != null && !detectedEnemy.IsDead())
            {
                target = detectedEnemy.transform;
            }

            direction = target != null
                ? (target.position - firePoint.position).normalized
                : Vector2.right * facingDirection;
        }

        Projectile projScript = projectile.GetComponent<Projectile>();
        if (projScript != null)
        {
            int damage = GetAttackDamage();
            // Eğer enemy player tarafından kontrol ediliyorsa, projectile'a bilgiyi ilet
            // Kontrol edilen enemy'nin referansını da ilet (aynı tip kontrolü için)
            Enemy controlledEnemyRef = isControlled ? this : null;
            projScript.Initialize(direction, projectileSpeed, damage, enemyType, isControlled, controlledEnemyRef);
        }
        else
        {
            Rigidbody2D projRb = projectile.GetComponent<Rigidbody2D>();
            if (projRb != null)
            {
                projRb.linearVelocity = direction * projectileSpeed;
            }
        }

        // Ateş animasyonu trigger'ı
        if (anim != null)
        {
            anim.SetTrigger("attack");
        }

        // Cooldown'u güncelle
        lastAttackTime = Time.time;

        // isAttacking flag'ini ayarla (UpdateAnimations'da animasyon bitince resetlenir)
        isAttacking = true;
        
        // Gerçekten ateş edildi, true döndür
        return true;
    }

    protected virtual int GetAttackDamage()
    {
        // Enemy tipine göre hasar (0: 10, 1: 15, 2: 20)
        return 10 + (enemyType * 5);
    }

    public int GetEnemyType()
    {
        return enemyType;
    }

    public float GetMoveSpeed()
    {
        return moveSpeed;
    }

    public void UpdateAnimations()
    {
        if (anim == null) return;
        if (isDead) return;

        // Attack animasyonu bitmiş mi kontrol et
        if (isAttacking)
        {
            AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
            // "Fire" veya "attack" state'inde mi kontrol et
            if (stateInfo.IsName("Fire") || stateInfo.IsName("Attack"))
            {
                // Animasyon bitmişse isAttacking'i resetle
                if (stateInfo.normalizedTime >= 1.0f)
                {
                    isAttacking = false;
                }
            }
            else
            {
                // Attack state'inde değilse resetle
                isAttacking = false;
            }
        }

        // Hareket animasyonu kontrolü
        bool isMoving = Mathf.Abs(rb.linearVelocity.x) > 0.1f && !isAttacking;
        anim.SetBool("isMoving", isMoving);
    }

    public void SetControlled(bool controlled, PlayerController player)
    {
        isControlled = controlled;
        controllingPlayer = player;

        if (controlled)
        {
            // Kontrol edildiğinde AI durdur
            detectedPlayer = null;
            detectedEnemy = null;
            hasAttackedSameType = false; // Reset flag
            SetVelocity(0, rb.linearVelocity.y);
        }
        else
        {
            // Kontrol bırakıldığında flag'i resetle
            hasAttackedSameType = false;
        }
    }

    public void SetHasAttackedSameType(bool attacked)
    {
        hasAttackedSameType = attacked;
    }

    public bool HasAttackedSameType()
    {
        return hasAttackedSameType;
    }

    public bool IsControlled()
    {
        return isControlled;
    }

    public Transform GetAttachmentPoint()
    {
        return attachmentPoint;
    }

    public PlayerController GetDetectedPlayer()
    {
        return detectedPlayer;
    }

    public bool IsDetectingPlayer(PlayerController player)
    {
        return detectedPlayer != null && detectedPlayer == player && !detectedPlayer.IsDead();
    }

    public override void TakeDamage(int damage)
    {
        // Hasar almadan önce hedef tespit et (arkası dönükken bile)
        if (!isDead && !isControlled)
        {
            // Önce player'ı ara (önünde olmasa bile)
            Collider2D[] nearbyPlayers = Physics2D.OverlapCircleAll(detectionPoint != null ? detectionPoint.position : transform.position, detectionRange * 2f, playerLayer);

            foreach (var col in nearbyPlayers)
            {
                PlayerController player = col.GetComponent<PlayerController>();
                if (player != null && !player.IsDead() && !player.IsHidden())
                {
                    // Hasar alınca player'ı tespit et ve ona dön
                    detectedPlayer = player;
                    detectedEnemy = null;
                    Vector2 toPlayer = (player.transform.position - transform.position).normalized;

                    // Player'a dön
                    if (toPlayer.x > 0 && !facingRight) Flip();
                    else if (toPlayer.x < 0 && facingRight) Flip();

                    // Hedef tespit edildi ama hasar vermeye devam et!
                    break; // return yerine break kullan
                }
            }

            // Player bulunamadıysa, farklı tipte enemy ara
            if (detectedPlayer == null)
            {
                Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(detectionPoint != null ? detectionPoint.position : transform.position, detectionRange * 2f, enemyLayer);
                foreach (var col in nearbyEnemies)
                {
                    Enemy enemy = col.GetComponent<Enemy>();
                    if (enemy != null && !enemy.IsDead() && enemy != this && !enemy.IsControlled())
                    {
                        if (enemy.GetEnemyType() != enemyType)
                        {
                            // Hasar alınca farklı tipte enemy'yi tespit et ve ona dön
                            detectedEnemy = enemy;
                            detectedPlayer = null;
                            Vector2 toEnemy = (enemy.transform.position - transform.position).normalized;

                            // Enemy'ye dön
                            if (toEnemy.x > 0 && !facingRight) Flip();
                            else if (toEnemy.x < 0 && facingRight) Flip();

                            break; // return yerine break kullan
                        }
                    }
                }
            }
        }

        // ÖNEMLİ: Her durumda hasar ver!
        base.TakeDamage(damage);
    }

    public override void Die()
    {
        base.Die();

        // Target detection'ı durdur
        detectedPlayer = null;
        detectedEnemy = null;
        isControlled = false;
        controllingPlayer = null;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Player enemy'nin üstüne çıktığında kontrol et
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null && !isDead && !isControlled)
        {
            // Player enemy'nin üstünde mi kontrol et
            if (collision.contacts[0].point.y > transform.position.y)
            {
                // Player enemy'nin üstünde ve ayrılırsa enemy ölür
                // Bu kontrol DetachFromEnemy'de yapılıyor
            }
        }
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        // Detection range gizmo (sarı çember)
        if (detectionPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(detectionPoint.position, detectionRange);
        }

        // Attack range gizmo (kırmızı çember)
        if (detectionPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(detectionPoint.position, attackRange);
        }

        // Detection ray gizmo (yeşil çizgi)
        if (detectionPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(detectionPoint.position, detectionPoint.position + Vector3.right * facingDirection * detectionRange);
        }

        // Detected target gizmo (mavi çizgi)
        if (detectedPlayer != null && !detectedPlayer.IsDead())
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, detectedPlayer.transform.position);
        }
        else if (detectedEnemy != null && !detectedEnemy.IsDead())
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, detectedEnemy.transform.position);
        }
    }
}

