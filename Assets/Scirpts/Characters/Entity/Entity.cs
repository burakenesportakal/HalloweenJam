using System;
using System.Collections;
using UnityEngine;
public class Entity : MonoBehaviour
{
    public Animator anim {  get; private set; }
    public Rigidbody2D rb { get; private set; }
    protected StateMachine stateMachine;

    private bool facingRight = true;
    public int facingDirection { get; private set; } = 1;

    [Header("Collision Detection")]
    [SerializeField] protected LayerMask whatIsGround;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform wallCheck;
    public bool groundDetected { get; private set; }
    public bool wallDetected { get; private set; }
    
    [Header("Enemy Detection")]
    [SerializeField] protected LayerMask whatIsEnemy;
    [SerializeField] private float enemyCheckDistance;
    [SerializeField] private Transform enemyCheck;
    public bool enemyDetected { get; private set; }
    public RaycastHit2D enemyHit { get; private set; }

    private bool isKnocked;
    private Coroutine knockbackCoroutine;

    protected virtual void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        stateMachine = new StateMachine();
    }

    protected virtual void Start() { }
    protected virtual void Update()
    {
        HandleCollisionDetection();
        stateMachine.UpdateActiveState();
    }
    public void CurrentStateAnimationTrigger()
    {
        stateMachine.currentState.AnimationTrigger();
    }
    public virtual void EntityDeath()
    {

    }
    public void ReciveKnockback(Vector2 knockback, float duraiton)
    {
        if (knockback == null)
            StopCoroutine(knockbackCoroutine);

        knockbackCoroutine = StartCoroutine(KnockbackCoroutine(knockback, duraiton));
    }
    private IEnumerator KnockbackCoroutine(Vector2 knockback, float duraiton)
    {
        isKnocked = true;
        rb.linearVelocity = knockback;

        yield return new WaitForSeconds(duraiton);

        rb.linearVelocity = Vector2.zero;
        isKnocked = false;
    }
    public void SetVelocity(float xVelocity, float yVelocity)
    {
        if (isKnocked) return;

        rb.linearVelocity = new Vector2(xVelocity, yVelocity);
        HandleFlip(xVelocity);
    }
    public void HandleFlip(float xVelocity)
    {
        if (xVelocity > 0 && facingRight == false) Flip();
        else if (xVelocity < 0 && facingRight) Flip();
    }
    public void Flip()
    {
        transform.Rotate(0, 180, 0);
        facingRight = !facingRight;
        facingDirection = facingDirection * -1;
    }
    public void HandleCollisionDetection()
    {
        groundDetected = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
        wallDetected = Physics2D.Raycast(wallCheck.position, Vector2.right, wallCheckDistance, whatIsGround);
        
        if (enemyCheck != null)
        {
            enemyHit = Physics2D.Raycast(enemyCheck.position, Vector2.right * facingDirection, enemyCheckDistance, whatIsEnemy);
            enemyDetected = enemyHit.collider != null && enemyHit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy");
        }
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, groundCheck.position + new Vector3(0, -groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, wallCheck.position + new Vector3(wallCheckDistance * facingDirection, 0));
        
        if (enemyCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(enemyCheck.position, enemyCheck.position + new Vector3(enemyCheckDistance * facingDirection, 0));
            Gizmos.color = Color.white;
        }

    }
}
