using UnityEngine;

public class Enemy : Entity
{
    public Enemy_IdleState idleState;
    public Enemy_MoveState moveState;
    public Enemy_AttackState attackState;
    public Enemy_BattleState battleState;
    public Enemy_DeadState deadState;

    [Header("Battle details")]
    public float battleMoveSpeed = 3;
    public float attackDistance = 2;
    public float battleTimeDuration = 5;
    public float minRetreatDistance = 1;
    public Vector2 retreatVelocity;

    [Header("Movement details")]
    public float idleTime = 2;
    public float moveSpeed = 1.4f;
    [Range(0, 2)]
    public float moveAnimSpeedMultiplier = 1;

    [Header("Player detection")]
    [SerializeField] private LayerMask whatIsPlayer;
    [SerializeField] private Transform playerCheck;
    [SerializeField] private float playerCheckDistance = 10;
    public Transform player { get; private set; }

    [Header("Control System")]
    private bool isControlled = false;
    private Player controllingPlayer;
    private bool hasAttackedWhileControlled = false;
    public override void EntityDeath()
    {
        base.EntityDeath();

        stateMachine.ChangeState(deadState);
    }
    private void HandlePlayerDeath()
    {
        stateMachine.ChangeState(idleState);
    }
    public void TryEnterBattleState(Transform player)
    {
        if (stateMachine.currentState == battleState || stateMachine.currentState == attackState)
            return;

        this.player = player;
        stateMachine.ChangeState(battleState);
    }
    public Transform GetPlayerReference()
    {
        if (player == null)
            player = PlayerDetected().transform;

        return player;
    }
    public RaycastHit2D PlayerDetected()
    {
        RaycastHit2D hit =
            Physics2D.Raycast(playerCheck.position, Vector2.right * facingDirection, playerCheckDistance, whatIsPlayer | whatIsGround);

        if (hit.collider == null || hit.collider.gameObject.layer != LayerMask.NameToLayer("Player"))
            return default;

        return hit;
    }
    public void SetControlled(bool controlled, Player player)
    {
        isControlled = controlled;
        controllingPlayer = player;
        hasAttackedWhileControlled = false; // Reset when control changes
        
        // Update animator parameter
        if (anim != null)
        {
            anim.SetBool("isControlled", controlled);
        }
        
        if (controlled)
        {
            // Disable AI state machine
            stateMachine.OffStateMachine();
        }
        else
        {
            // Re-enable state machine if not dead
            Entity_Health health = GetComponent<Entity_Health>();
            if (health != null && !health.IsDead())
            {
                // State machine will be re-enabled if needed
            }
        }
    }
    
    public void SetHasAttackedWhileControlled(bool hasAttacked)
    {
        hasAttackedWhileControlled = hasAttacked;
    }
    
    public bool HasAttackedWhileControlled()
    {
        return hasAttackedWhileControlled;
    }

    public bool IsControlled()
    {
        return isControlled;
    }
    
    public Player GetControllingPlayer()
    {
        return controllingPlayer;
    }

    public bool IsDead()
    {
        Entity_Health health = GetComponent<Entity_Health>();
        return health != null && health.IsDead();
    }

    protected override void Update()
    {
        // Don't run AI state machine if controlled
        if (isControlled)
        {
            HandleCollisionDetection();
            return;
        }

        base.Update();
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(playerCheck.position, new Vector3(playerCheck.position.x + (facingDirection * playerCheckDistance), playerCheck.position.y));
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(playerCheck.position, new Vector3(playerCheck.position.x + (facingDirection * attackDistance), playerCheck.position.y));
        Gizmos.color = Color.green;
        Gizmos.DrawLine(playerCheck.position, new Vector3(playerCheck.position.x + (facingDirection * minRetreatDistance), playerCheck.position.y));

    }
    private void OnEnable()
    {
        Player.OnPlayerDeath += HandlePlayerDeath;
    }
    private void OnDisable()
    {
        Player.OnPlayerDeath -= HandlePlayerDeath;
    }
}
