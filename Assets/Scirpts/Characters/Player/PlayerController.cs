using UnityEngine;

public class PlayerController : Entity
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    private Vector2 moveInput;

    [Header("Attack Jump")]
    [SerializeField] private float minJumpForce = 5f;
    [SerializeField] private float maxJumpForce = 15f;
    [SerializeField] private float maxChargeTime = 2f;
    [SerializeField] private Vector2 attackDirectionMultiplier = new Vector2(1f, 1f);
    private float chargeTime = 0f;
    private bool isCharging = false;
    private bool isInAttackJump = false;

    [Header("Detection")]
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float enemyCheckDistance = 3f;
    [SerializeField] private Transform enemyCheckPoint;
    private RaycastHit2D enemyHit;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private Transform groundCheckPoint;
    private bool isGrounded = false;

    [Header("Enemy Control")]
    private Enemy attachedEnemy = null;
    private bool isControllingEnemy = false;

    [Header("Carry System")]
    [SerializeField] private float carryCheckRadius = 1.5f;
    private Enemy carriedEnemy = null;
    private bool isCarrying = false;

    [Header("Hiding System")]
    [SerializeField] private string hiddenTag = "Hidden";
    [SerializeField] private int hiddenLayer = 8;
    private string originalTag;
    private int originalLayer;
    private bool isInHidingSpot = false;
    private bool isHidden = false;
    private float originalGravityScale = 3f; // Orijinal gravity scale'i sakla

    protected override void Awake()
    {
        base.Awake();
        
        // Rotation'ı sıfırla (sprite flip için rotation kullanmıyoruz)
        transform.rotation = Quaternion.identity;
        
        // Facing direction'ı başlangıç değerine ayarla
        facingRight = true;
        facingDirection = 1;
    }

    protected override void Start()
    {
        base.Start();
        originalTag = gameObject.tag;
        originalLayer = gameObject.layer;
        
        // Orijinal gravity scale'i kaydet
        if (rb != null)
        {
            originalGravityScale = rb.gravityScale;
        }
        
        // SpriteRenderer'ı başlangıç durumuna ayarla
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = false; // Sağa bakıyor başlangıçta
        }
    }

    protected override void Update()
    {
        base.Update();

        // Input'ları al
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        // Enemy kontrolü sırasında normal hareketi durdur
        if (isControllingEnemy)
        {
            ControlEnemy();
            UpdateAnimations();
            return;
        }

        // Taşıma sırasında hareket
        if (isCarrying)
        {
            HandleCarryingMovement();
        }

        // Gizlenme durumunda hareket etme
        if (!isHidden)
        {
            CheckGround();
            HandleMovement();
            HandleJump();
            HandleAttack();
            HandleInteraction();
        }

        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        // Yapışık durumda pozisyonu güncelle
        if (attachedEnemy != null && !isControllingEnemy)
        {
            AttachToEnemy();
        }

        // Taşıma sırasında enemy pozisyonunu güncelle
        if (isCarrying && carriedEnemy != null)
        {
            UpdateCarriedEnemyPosition();
        }
    }

    private void CheckGround()
    {
        if (groundCheckPoint == null) return;

        isGrounded = Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckDistance, groundLayer);

        // Yere düştüyse attack jump'ı bitir
        if (isInAttackJump && isGrounded && rb.linearVelocity.y <= 0)
        {
            isInAttackJump = false;
        }
    }

    private void HandleMovement()
    {
        if (isInAttackJump) return; // Attack jump sırasında hareket etme

        float xVelocity = moveInput.x * moveSpeed;
        float yVelocity = rb.linearVelocity.y;

        SetVelocity(xVelocity, yVelocity);
        HandleFlip(xVelocity);
    }

    private void HandleJump()
    {
        if (isInAttackJump) return; // Attack jump sırasında normal jump yapma

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            SetVelocity(rb.linearVelocity.x, jumpForce);
        }
    }

    private void HandleAttack()
    {
        // Enemy kontrolü sırasında attack jump yapma
        if (isControllingEnemy || isCarrying) return;

        // Attack başladı - charge başlat (sol mouse butonu)
        if (Input.GetMouseButtonDown(0) && isGrounded)
        {
            isCharging = true;
            chargeTime = 0f;
        }

        // Attack basılı tutuluyor - charge artır
        if (isCharging && Input.GetMouseButton(0))
        {
            chargeTime += Time.deltaTime;
            chargeTime = Mathf.Clamp(chargeTime, 0f, maxChargeTime);
        }

        // Attack bırakıldı - atlama yap
        if (isCharging && Input.GetMouseButtonUp(0))
        {
            LaunchAttack();
            isCharging = false;
        }

        // Attack input'u bırakıldıysa charge'ı sıfırla
        if (isCharging && !Input.GetMouseButton(0))
        {
            isCharging = false;
            chargeTime = 0f;
        }

        // Attack jump sırasında enemy kontrolü
        if (isInAttackJump)
        {
            CheckEnemyAttachment();
        }
    }

    private void LaunchAttack()
    {
        float chargeRatio = chargeTime / maxChargeTime;
        float jumpForceValue = Mathf.Lerp(minJumpForce, maxJumpForce, chargeRatio);

        Vector2 attackDirection = new Vector2(facingDirection * attackDirectionMultiplier.x, attackDirectionMultiplier.y);
        
        // Enemy detect edildiyse ona doğru ayarla
        CheckEnemyInFront();
        if (enemyHit.collider != null)
        {
            Enemy detectedEnemy = enemyHit.collider.GetComponent<Enemy>();
            if (detectedEnemy != null && !detectedEnemy.IsDead())
            {
                Vector2 enemyPos = enemyHit.collider.transform.position;
                Vector2 playerPos = transform.position;
                Vector2 toEnemy = (enemyPos - playerPos).normalized;
                
                // Enemy yönüne blend yap
                Vector2 baseDirection = new Vector2(facingDirection * attackDirectionMultiplier.x, attackDirectionMultiplier.y);
                Vector2 normalizedBase = baseDirection.normalized;
                Vector2 blended = Vector2.Lerp(normalizedBase, toEnemy, 0.3f);
                float magnitude = baseDirection.magnitude;
                attackDirection = blended * magnitude;
            }
        }

        float xVelocity = attackDirection.x * jumpForceValue;
        float yVelocity = attackDirection.y * jumpForceValue;
        
        SetVelocity(xVelocity, yVelocity);
        HandleFlip(xVelocity);
        
        isInAttackJump = true;
        chargeTime = 0f;
    }

    private void CheckEnemyInFront()
    {
        if (enemyCheckPoint == null) return;

        enemyHit = Physics2D.Raycast(enemyCheckPoint.position, Vector2.right * facingDirection, enemyCheckDistance, enemyLayer);
    }

    private void CheckEnemyAttachment()
    {
        if (!isInAttackJump) return;

        // Yakındaki enemy'leri bul
        Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(transform.position, 2f, enemyLayer);
        
        foreach (var col in nearbyColliders)
        {
            Enemy hitEnemy = col.GetComponent<Enemy>();
            if (hitEnemy == null || hitEnemy.IsDead() || hitEnemy.IsControlled()) continue;
            
            // Trigger collider olan enemy'ye yapışma (ölü enemy)
            if (col.isTrigger) continue;
            
            // Mesafe kontrolü
            float distance = Vector2.Distance(transform.position, hitEnemy.transform.position);
            if (distance > 2f) continue;
            
            // Enemy'nin arkası player'a dönük mü kontrol et
            Vector2 enemyPos = hitEnemy.transform.position;
            Vector2 playerPos = transform.position;
            Vector2 toPlayer = (playerPos - enemyPos);
            
            if (toPlayer.magnitude < 0.01f) continue;
            
            toPlayer.Normalize();
            int enemyFacingDirection = hitEnemy.facingDirection;
            
            // Enemy'nin arkası player'a dönük mü kontrol et
            float dotProduct = toPlayer.x * enemyFacingDirection;
            if (dotProduct > 0) continue; // Enemy'nin önündeyiz, yapışma yok
            
            // Yapışma başlat
            AttachToEnemy(hitEnemy);
            return;
        }
    }

    private void AttachToEnemy(Enemy enemy)
    {
        if (enemy == null || enemy.IsDead() || enemy.IsControlled())
        {
            return;
        }

        attachedEnemy = enemy;
        isControllingEnemy = true;
        
        // Attack sistemi flag'lerini sıfırla
        isInAttackJump = false;
        isCharging = false;
        chargeTime = 0f;
        
        // Player'ı enemy'nin child'ı yap
        transform.SetParent(enemy.transform);
        
        // Rigidbody'yi kinematic yap
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.freezeRotation = true;
        
        // Rotation'ı sıfırla (sprite flip için rotation kullanmıyoruz)
        transform.localRotation = Quaternion.identity;
        
        // Enemy'yi kontrol et
        enemy.SetControlled(true, this);
    }

    private void AttachToEnemy()
    {
        if (attachedEnemy == null || attachedEnemy.IsDead())
        {
            DetachFromEnemy();
            return;
        }

        // Pozisyonu enemy'nin sırtında tut
        CapsuleCollider2D enemyCol = attachedEnemy.capsuleCollider;
        if (enemyCol != null && capsuleCollider != null)
        {
            float enemyHalfWidth = enemyCol.bounds.extents.x;
            float playerHalfWidth = capsuleCollider.bounds.extents.x;
            float enemyHalfHeight = enemyCol.bounds.extents.y;
            float playerHalfHeight = capsuleCollider.bounds.extents.y;
            
            // Enemy'nin arkasına (sırtına) konumlan
            float xOffset = -attachedEnemy.facingDirection * (enemyHalfWidth + playerHalfWidth * 0.3f);
            float yOffset = enemyHalfHeight * 0.8f + playerHalfHeight * 0.5f;
            
            transform.localPosition = new Vector3(xOffset, yOffset, transform.localPosition.z);
            transform.localRotation = Quaternion.identity;
            
            // SpriteRenderer flip'ini düzgün ayarla
            if (spriteRenderer != null)
            {
                // Enemy'nin facing direction'ına göre player'ın sprite'ını ayarla
                // Enemy sağa bakıyorsa (facingDirection = 1), player enemy'nin arkasında (solda) olmalı
                // Enemy sola bakıyorsa (facingDirection = -1), player enemy'nin arkasında (sağda) olmalı
                // Player her zaman enemy'nin arkasında, o yüzden enemy'nin tersine bakmalı
                spriteRenderer.flipX = (attachedEnemy.facingDirection > 0);
            }
        }
    }

    private void ControlEnemy()
    {
        if (attachedEnemy == null || attachedEnemy.IsDead())
        {
            DetachFromEnemy();
            return;
        }

        // Enemy hareket kontrolü
        if (moveInput.x != 0)
        {
            float enemySpeed = attachedEnemy.GetMoveSpeed();
            attachedEnemy.SetVelocity(moveInput.x * enemySpeed, attachedEnemy.rb.linearVelocity.y);
            attachedEnemy.HandleFlip(moveInput.x);
        }
        else
        {
            attachedEnemy.SetVelocity(0, attachedEnemy.rb.linearVelocity.y);
        }

        // Enemy saldırısı (ranged attack - önüne ateş et)
        if (Input.GetMouseButtonDown(0))
        {
            if (attachedEnemy != null && !attachedEnemy.IsDead())
            {
                attachedEnemy.FireProjectile();
            }
        }

        // Interaction ile ayrıl (X tuşu)
        if (Input.GetKeyDown(KeyCode.X))
        {
            DetachFromEnemy();
        }
    }

    private void DetachFromEnemy()
    {
        if (attachedEnemy != null)
        {
            attachedEnemy.SetControlled(false, null);
            attachedEnemy.Die(); // Enemy ölür
        }

        // Parent'tan ayrıl
        transform.SetParent(null);
        
        attachedEnemy = null;
        isControllingEnemy = false;
        
        // Physics'i geri aç - orijinal gravity scale'i geri yükle
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = originalGravityScale; // Orijinal değeri geri yükle
        rb.freezeRotation = false;
        
        // Velocity'yi tamamen sıfırla
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        
        // Rotation'ı sıfırla (sprite flip için rotation kullanmıyoruz)
        transform.rotation = Quaternion.identity;
        
        // Attack sistemi flag'lerini sıfırla
        isInAttackJump = false;
        isCharging = false;
        chargeTime = 0f;
    }

    private void HandleInteraction()
    {
        if (!Input.GetKeyDown(KeyCode.X)) return;

        // Enemy kontrolü sırasında ayrılma kontrolü zaten ControlEnemy'de yapılıyor
        if (isControllingEnemy) return;

        // Gizlenme yeri kontrolü
        if (isInHidingSpot)
        {
            ToggleHiding();
            return;
        }

        // Ölü enemy taşıma kontrolü
        if (!isCarrying)
        {
            CheckDeadEnemyPickup();
        }
        else
        {
            DropCarriedEnemy();
        }
    }

    private void ToggleHiding()
    {
        isHidden = !isHidden;

        if (isHidden)
        {
            gameObject.tag = hiddenTag;
            gameObject.layer = hiddenLayer;
            if (anim != null)
            {
                anim.SetBool("isHidden", true);
            }
        }
        else
        {
            gameObject.tag = originalTag;
            gameObject.layer = originalLayer;
            if (anim != null)
            {
                anim.SetBool("isHidden", false);
            }
        }
    }

    private void CheckDeadEnemyPickup()
    {
        // Yakındaki ölü enemy'leri bul
        Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(transform.position, carryCheckRadius, enemyLayer);
        
        foreach (var col in nearbyColliders)
        {
            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy == null || !enemy.IsDead()) continue;
            
            // Ölü enemy'yi taşı
            PickupEnemy(enemy);
            return;
        }
    }

    private void PickupEnemy(Enemy enemy)
    {
        if (enemy == null || !enemy.IsDead()) return;

        carriedEnemy = enemy;
        isCarrying = true;

        // Enemy'yi player'ın child'ı yap
        enemy.transform.SetParent(transform);
        
        // Enemy'nin collider'ını trigger yap
        if (enemy.capsuleCollider != null)
        {
            enemy.capsuleCollider.isTrigger = true;
        }

        // Enemy'nin rigidbody'sini kinematic yap
        if (enemy.rb != null)
        {
            enemy.rb.bodyType = RigidbodyType2D.Kinematic;
            enemy.rb.linearVelocity = Vector2.zero;
        }
    }

    private void HandleCarryingMovement()
    {
        float xVelocity = moveInput.x * moveSpeed * 0.5f; // Yavaş hareket
        float yVelocity = rb.linearVelocity.y;

        SetVelocity(xVelocity, yVelocity);
        HandleFlip(xVelocity);
    }

    private void UpdateCarriedEnemyPosition()
    {
        if (carriedEnemy == null) return;

        CapsuleCollider2D playerCol = capsuleCollider;
        if (playerCol != null && carriedEnemy.capsuleCollider != null)
        {
            float playerHalfHeight = playerCol.bounds.extents.y;
            float enemyHalfHeight = carriedEnemy.capsuleCollider.bounds.extents.y;
            
            // Player'ın üstünde taşı
            float yOffset = playerHalfHeight + enemyHalfHeight + 0.2f;
            carriedEnemy.transform.localPosition = new Vector3(0, yOffset, 0);
            carriedEnemy.transform.localRotation = Quaternion.identity;
        }
    }

    private void DropCarriedEnemy()
    {
        if (carriedEnemy == null) return;

        // Parent'tan ayrıl
        carriedEnemy.transform.SetParent(null);

        // Eğer gizlenme yerindeyse enemy'yi yok et
        if (isInHidingSpot)
        {
            Destroy(carriedEnemy.gameObject);
        }
        else
        {
            // Collider'ı geri aç
            if (carriedEnemy.capsuleCollider != null)
            {
                carriedEnemy.capsuleCollider.isTrigger = true; // Ölü enemy trigger kalır
            }
        }

        carriedEnemy = null;
        isCarrying = false;
    }

    public void SetInHidingSpot(bool inSpot, HidingSpot spot)
    {
        isInHidingSpot = inSpot;
        
        // Eğer gizlenme yerindeyse ve enemy taşıyorsak enemy'yi yok et
        if (inSpot && isCarrying && carriedEnemy != null)
        {
            Destroy(carriedEnemy.gameObject);
            carriedEnemy = null;
            isCarrying = false;
        }
    }

    public bool IsCarrying()
    {
        return isCarrying;
    }

    public Enemy GetCarriedEnemy()
    {
        return carriedEnemy;
    }

    private void UpdateAnimations()
    {
        if (anim == null) return;

        // Idle/Move
        bool isMoving = Mathf.Abs(moveInput.x) > 0.1f && !isControllingEnemy && !isCarrying && !isInAttackJump;
        anim.SetBool("isMoving", isMoving);

        // Jump/Fall - sadece havadayken
        bool isJumping = isGrounded ? false : (rb.linearVelocity.y > 0.1f);
        bool isFalling = isGrounded ? false : (rb.linearVelocity.y < -0.1f);
        
        anim.SetBool("isJumping", isJumping);
        anim.SetBool("isFalling", isFalling);

        // Attached
        anim.SetBool("isAttached", isControllingEnemy);

        // Carrying
        anim.SetBool("isCarrying", isCarrying);
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        // Enemy check gizmo
        if (enemyCheckPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(enemyCheckPoint.position, enemyCheckPoint.position + Vector3.right * facingDirection * enemyCheckDistance);
        }

        // Ground check gizmo
        if (groundCheckPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(groundCheckPoint.position, groundCheckPoint.position + Vector3.down * groundCheckDistance);
        }

        // Carry check gizmo
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, carryCheckRadius);
    }
}
