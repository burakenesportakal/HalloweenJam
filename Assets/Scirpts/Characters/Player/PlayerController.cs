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

        // SpriteRenderer flipX'i sıfırla (transform rotation kullanıyoruz)
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = false;
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

        // Gizlenme durumunda sadece interaction'a izin ver (gizlenmeden çıkabilmek için)
        // Diğer hareketler gizlenme durumunda yasak
        if (!isHidden)
        {
            CheckGround();
            HandleMovement();
            HandleJump();
            HandleAttack();
        }
        
        // Interaction her zaman çalışabilir (gizlenmeden çıkmak için)
        HandleInteraction();

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
        
        // Enemy kontrolü sırasında kamera rotation'ını sabit tut
        if (isControllingEnemy)
        {
            FixCameraRotation();
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

        // Attack jump devam ediyorsa sadece enemy kontrolü yap
        if (isInAttackJump)
        {
            CheckEnemyAttachment();
            return;
        }

        // Attack başladı - charge başlat (sol mouse butonu)
        // NOT: Havadayken de attack jump yapılabilir (enemy'ye atlama için)
        if (Input.GetMouseButtonDown(0))
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
    }

    private void LaunchAttack()
    {
        // Debug: Attack jump başlatılıyor
        Debug.Log($"LaunchAttack: isControllingEnemy = {isControllingEnemy}, isCarrying = {isCarrying}, isGrounded = {isGrounded}, isInAttackJump = {isInAttackJump}");
        
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
        
        Debug.Log($"LaunchAttack: Attack jump başlatıldı! isInAttackJump = {isInAttackJump}");
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

            // ÖNEMLİ: Enemy player'ı görüyorsa (takip ediyorsa) yapışma yok
            // Enemy player'a bakıyorsa, player enemy'nin önündedir, yapışamaz
            if (hitEnemy.IsDetectingPlayer(this))
            {
                continue; // Enemy player'ı görüyor/takip ediyor, yapışma yok
            }

            // Enemy'nin arkası player'a dönük mü kontrol et
            Vector2 enemyPos = hitEnemy.transform.position;
            Vector2 playerPos = transform.position;
            Vector2 toPlayer = (playerPos - enemyPos);

            if (toPlayer.magnitude < 0.01f) continue;

            toPlayer.Normalize();
            int enemyFacingDirection = hitEnemy.facingDirection;

            // Enemy'nin arkası player'a dönük mü kontrol et
            // dotProduct < 0 = player enemy'nin arkasında (sırtına dönük)
            float dotProduct = toPlayer.x * enemyFacingDirection;
            if (dotProduct >= 0) continue; // Enemy'nin önündeyiz veya yanındayız, yapışma yok

            // Sadece enemy'nin sırtı player'a dönükse yapışabilir
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

        // Kamera rotation'ını sabit tut (kamera player'ın child'ı ise)
        FixCameraRotation();

        // Enemy'yi kontrol et
        enemy.SetControlled(true, this);
    }
    
    private void FixCameraRotation()
    {
        // ÖNEMLİ: Player enemy'nin child'ı olduğunda rotation'ı sıfırla
        // Bu sayede Cinemachine'in follow target rotation'ı da etkilenmez
        transform.localRotation = Quaternion.identity;
        
        // Eğer Cinemachine kullanılıyorsa, player'ın child'ları arasında kamera'yı kontrol et
        Camera mainCamera = GetComponentInChildren<Camera>();
        if (mainCamera != null)
        {
            // Kamera'nın rotation'ını sabit tut (world rotation)
            Quaternion cameraWorldRotation = mainCamera.transform.rotation;
            mainCamera.transform.rotation = cameraWorldRotation;
            
            // Kamera'nın scale'ini de sabit tut (flip'ten etkilenmemesi için)
            Vector3 cameraScale = mainCamera.transform.localScale;
            if (cameraScale.x < 0)
            {
                mainCamera.transform.localScale = new Vector3(
                    Mathf.Abs(cameraScale.x),
                    cameraScale.y,
                    cameraScale.z
                );
            }
        }
    }

    private void AttachToEnemy()
    {
        if (attachedEnemy == null || attachedEnemy.IsDead())
        {
            DetachFromEnemy();
            return;
        }

        // Enemy'nin attachment point'ini kullan (sabit nokta)
        Transform attachmentPoint = attachedEnemy.GetAttachmentPoint();
        if (attachmentPoint != null)
        {
            // Player enemy'nin child'ı olduğu için, attachment point'in local position'ını kullan
            transform.localPosition = attachmentPoint.localPosition;
            transform.localRotation = Quaternion.identity;
        }
        else
        {
            // Fallback: Attachment point yoksa eski yöntemi kullan
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
            }
        }
        
        // Kamera rotation'ını sürekli sabit tut (enemy flip attığında etkilenmemesi için)
        FixCameraRotation();
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

        // Enemy animasyonlarını güncelle
        attachedEnemy.UpdateAnimations();

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
            // Enemy'yi öldür (currentHealth 0 olur)
            attachedEnemy.Die();

            // Debug: Enemy'nin currentHealth'inin 0 olduğunu kontrol et
            Debug.Log($"Enemy çıkış: CurrentHealth = {attachedEnemy.GetCurrentHealth()}, IsDead = {attachedEnemy.IsDead()}");
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

        // Attack sistemi flag'lerini tamamen sıfırla - yeni attack jump yapılabilmesi için
        isInAttackJump = false;
        isCharging = false;
        chargeTime = 0f;
        
        // Debug: Flag'lerin sıfırlandığını kontrol et
        Debug.Log($"DetachFromEnemy: isInAttackJump = {isInAttackJump}, isControllingEnemy = {isControllingEnemy}, isCharging = {isCharging}");
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

    public bool IsHidden()
    {
        return isHidden;
    }

    public void ExitHiding()
    {
        if (isHidden)
        {
            isHidden = false;
            gameObject.tag = originalTag;
            gameObject.layer = originalLayer;
            if (anim != null)
            {
                anim.SetBool("isHidden", false);
            }
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

    public bool IsControllingEnemy()
    {
        return isControllingEnemy;
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
