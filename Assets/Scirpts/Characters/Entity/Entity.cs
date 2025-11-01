using UnityEngine;

public class Entity : MonoBehaviour
{
    [Header("Components")]
    public Animator anim { get; protected set; }
    public Rigidbody2D rb { get; protected set; }
    public CapsuleCollider2D capsuleCollider { get; protected set; }
    public SpriteRenderer spriteRenderer { get; protected set; }

    [Header("Health")]
    [SerializeField] protected int maxHealth = 100;
    protected int currentHealth;
    protected bool isDead = false;

    [Header("Facing")]
    protected bool facingRight = true;
    public int facingDirection { get; protected set; } = 1;

    protected virtual void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        currentHealth = maxHealth;
        
        // Rotation'ı sıfırla (sprite flip için rotation kullanmıyoruz)
        transform.rotation = Quaternion.identity;
        
        // Facing direction'ı başlangıç değerine ayarla
        facingRight = true;
        facingDirection = 1;
    }

    protected virtual void Start()
    {
        // Override in derived classes
    }

    protected virtual void Update()
    {
        // Override in derived classes
    }

    public virtual void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public virtual void Heal(int amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    public virtual void Die()
    {
        if (isDead) return;

        isDead = true;
        currentHealth = 0;

        // Animasyon trigger'ı
        if (anim != null)
        {
            anim.SetTrigger("death");
        }

        // Collider'ı trigger yap (ölü enemy'ye girilebilir)
        if (capsuleCollider != null)
        {
            capsuleCollider.isTrigger = true;
        }

        // Rigidbody'yi durdur
        if (rb != null)
        {
            rb.simulated = false;
            rb.gravityScale = 0f;
            rb.linearVelocity = Vector2.zero;
        }
    }

    public bool IsDead()
    {
        return isDead;
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public void Flip()
    {
        // SpriteRenderer varsa flipX kullan, yoksa rotation kullan
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }
        else
        {
            transform.Rotate(0, 180, 0);
        }
        
        facingRight = !facingRight;
        facingDirection *= -1;
    }

    public void HandleFlip(float xVelocity)
    {
        if (xVelocity > 0 && !facingRight) Flip();
        else if (xVelocity < 0 && facingRight) Flip();
    }

    public void SetVelocity(float xVelocity, float yVelocity)
    {
        if (rb == null) return;
        rb.linearVelocity = new Vector2(xVelocity, yVelocity);
    }

    protected virtual void OnDrawGizmos()
    {
        // Override in derived classes
    }
}
