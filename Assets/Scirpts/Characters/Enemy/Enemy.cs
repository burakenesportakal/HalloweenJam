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
    [SerializeField] protected Transform detectionPoint;
    private PlayerController detectedPlayer = null;
    private bool isControlled = false;
    private PlayerController controllingPlayer = null;

    [Header("Attack")]
    [SerializeField] protected GameObject projectilePrefab;
    [SerializeField] protected Transform firePoint;
    [SerializeField] protected float projectileSpeed = 10f;
    [SerializeField] protected float attackCooldown = 2f;
    private float lastAttackTime = 0f;
    private bool isAttacking = false;

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
        HandlePlayerDetection();
        
        // Player detect edildiyse saldır
        if (detectedPlayer != null && !detectedPlayer.IsDead())
        {
            HandleRangedAttack();
        }
        else
        {
            HandlePatrol();
        }
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

        // Patrol mantığı - sadece A ve B noktaları arasında gezin
        Vector3 targetPoint = movingToB ? patrolPointB.position : patrolPointA.position;
        Vector2 direction = (targetPoint - transform.position).normalized;

        // Hedefe yakınsa yön değiştir
        if (Vector2.Distance(transform.position, targetPoint) < 0.5f)
        {
            movingToB = !movingToB;
        }

        // Hareket et
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

    protected virtual void HandlePlayerDetection()
    {
        if (detectionPoint == null) return;

        // Player'ı detection range içinde ara
        Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(detectionPoint.position, detectionRange, playerLayer);
        detectedPlayer = null;

        foreach (var col in nearbyColliders)
        {
            PlayerController player = col.GetComponent<PlayerController>();
            if (player == null || player.IsDead()) continue;

            // Player'ın enemy'nin önünde mi kontrol et (sırtı dönükse göremez)
            Vector2 toPlayer = (player.transform.position - transform.position);
            
            // Mesafe kontrolü
            float distance = toPlayer.magnitude;
            if (distance > detectionRange) continue;
            
            toPlayer.Normalize();
            
            // Enemy'nin önünde mi kontrol et
            // facingDirection = 1 ise sağa, -1 ise sola bakıyor
            // toPlayer.x > 0 ise player sağda, toPlayer.x < 0 ise player solda
            // Enemy'nin önünde olması için: toPlayer.x * facingDirection > 0
            float dotProduct = toPlayer.x * facingDirection;
            
            // dotProduct > 0 = player enemy'nin önünde (görebilir)
            // dotProduct <= 0 = player enemy'nin arkasında (göremez)
            if (dotProduct > 0)
            {
                detectedPlayer = player;
                break;
            }
        }
    }

    protected virtual void HandleRangedAttack()
    {
        if (detectedPlayer == null || detectedPlayer.IsDead()) return;

        float distanceToPlayer = Vector2.Distance(transform.position, detectedPlayer.transform.position);

        // Player attack range içindeyse saldır ve dur
        if (distanceToPlayer <= attackRange)
        {
            // Ateş etme kontrolü
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                FireProjectile();
                lastAttackTime = Time.time;
            }
            
            // Attack range içindeyken dur
            SetVelocity(0, rb.linearVelocity.y);
            isAttacking = true;
            
            // Animasyon
            if (anim != null)
            {
                anim.SetBool("isMoving", false);
            }
        }
        else
        {
            // Attack range dışındaysa player'a doğru takip et
            Vector2 directionToPlayer = (detectedPlayer.transform.position - transform.position).normalized;
            
            if (isGrounded && !isAttacking)
            {
                SetVelocity(directionToPlayer.x * moveSpeed, rb.linearVelocity.y);
                HandleFlip(directionToPlayer.x);
            }

            isAttacking = false;
        }
    }

    public virtual void FireProjectile()
    {
        if (projectilePrefab == null || firePoint == null) return;
        if (isDead) return;

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        
        Vector2 direction;
        
        // Eğer kontrol ediliyorsa, önüne ateş et (facing direction)
        if (isControlled)
        {
            direction = Vector2.right * facingDirection;
        }
        // Eğer kontrol edilmiyorsa, player'a doğru ateş et
        else
        {
            direction = detectedPlayer != null 
                ? (detectedPlayer.transform.position - firePoint.position).normalized 
                : Vector2.right * facingDirection;
        }

        Projectile projScript = projectile.GetComponent<Projectile>();
        if (projScript != null)
        {
            int damage = GetAttackDamage();
            projScript.Initialize(direction, projectileSpeed, damage, enemyType);
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

        isAttacking = true;
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

    public void SetControlled(bool controlled, PlayerController player)
    {
        isControlled = controlled;
        controllingPlayer = player;

        if (controlled)
        {
            // Kontrol edildiğinde AI durdur
            detectedPlayer = null;
            SetVelocity(0, rb.linearVelocity.y);
        }
    }

    public bool IsControlled()
    {
        return isControlled;
    }

    public override void Die()
    {
        base.Die();
        
        // Player detection'ı durdur
        detectedPlayer = null;
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

        // Patrol range gizmo (mavi çember) - kaldırıldı
        // Gizmos.color = Color.blue;
        // Gizmos.DrawWireSphere(startPosition, patrolRange);

        // Detection ray gizmo (yeşil çizgi)
        if (detectionPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(detectionPoint.position, detectionPoint.position + Vector3.right * facingDirection * detectionRange);
        }
    }
}

