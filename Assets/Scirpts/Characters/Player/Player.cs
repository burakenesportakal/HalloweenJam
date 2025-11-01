using System;
using System.Collections;
using UnityEngine;


public class Player : Entity
{
    public static event Action OnPlayerDeath;
    public PlayerInputSet input;

    //player states
    public Player_AttackState attackState { get; private set; }
    public Player_AttachedState attachedState { get; private set; }
    public Player_ControlledState controlledState { get; private set; }
    public Player_DeadState deadState {  get; private set; }
    public Player_FallState fallState {  get; private set; }
    public Player_IdleState idleState {  get; private set; }
    public Player_JumpState jumpState {  get; private set; }
    public Player_RunState moveState { get; private set; }
    
    public Enemy currentControlledEnemy { get; private set; }

    [Header("Movement Details")]
    public float moveSpeed;
    public float jumpForce = 5;
    [Range(0, 1)]
    public float inAirMoveMultiplier = 0.7f;
    public Vector2 moveInput { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        input = new PlayerInputSet();

        idleState = new Player_IdleState(this, stateMachine, "idle");
        moveState = new Player_RunState(this, stateMachine, "move");
        jumpState = new Player_JumpState(this, stateMachine, "jumpFall");
        fallState = new Player_FallState(this, stateMachine, "jumpFall");
        deadState = new Player_DeadState(this, stateMachine, "dead");
        attackState = new Player_AttackState(this, stateMachine, "attack");
        attachedState = new Player_AttachedState(this, stateMachine, "attached");
        controlledState = new Player_ControlledState(this, stateMachine, "controlled");
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(idleState);
    }

    public override void EntityDeath()
    {
        base.EntityDeath();
        OnPlayerDeath?.Invoke();
        stateMachine.ChangeState(deadState);
    }

    private void OnEnable()
    {
        input.Enable();

        input.Player.Movement.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        input.Player.Movement.canceled += ctx => moveInput = Vector2.zero;
    }
    private void OnDisable()
    {
        input.Disable();
    }

    public void AttachToEnemy(Enemy enemy)
    {
        if (enemy != null && !enemy.IsControlled())
        {
            currentControlledEnemy = enemy;
        }
    }

    public void SetControlledEnemy(Enemy enemy)
    {
        currentControlledEnemy = enemy;
    }
}

