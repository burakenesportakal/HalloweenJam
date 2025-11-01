using UnityEngine;

public class Player_AttackState : Player_AirState
{
    private float chargeTime = 0f;
    [SerializeField] private float maxChargeTime = 2f;
    [SerializeField] private float minJumpForce = 5f;
    [SerializeField] private float maxJumpForce = 15f;
    private bool hasLaunched = false;
    private Vector2 attackDirection;

    public Player_AttackState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        chargeTime = 0f;
        hasLaunched = false;
        
        // Always jump in facing direction with upward component
        // If enemy is detected and in front, slightly adjust direction towards enemy
        Vector2 baseDirection = new Vector2(player.facingDirection, 1f).normalized;
        
        if (player.enemyDetected && player.enemyHit.collider != null)
        {
            Vector2 enemyPos = player.enemyHit.collider.transform.position;
            Vector2 playerPos = player.transform.position;
            Vector2 toEnemy = (enemyPos - playerPos).normalized;
            
            // Only adjust if enemy is in front (same direction as facing)
            if (Mathf.Sign(toEnemy.x) == Mathf.Sign(player.facingDirection))
            {
                // Blend between facing direction and enemy direction (prioritize facing)
                attackDirection = Vector2.Lerp(baseDirection, toEnemy, 0.3f).normalized;
            }
            else
            {
                // Enemy is behind, just use facing direction
                attackDirection = baseDirection;
            }
        }
        else
        {
            // No enemy detected, jump in facing direction
            attackDirection = baseDirection;
        }
    }

    public override void Update()
    {
        base.Update();

        // Charge while holding attack button
        if (input.Player.Attack.IsPressed() && !hasLaunched)
        {
            chargeTime += Time.deltaTime;
            chargeTime = Mathf.Clamp(chargeTime, 0f, maxChargeTime);
        }
        else if (input.Player.Attack.WasReleasedThisFrame() && !hasLaunched)
        {
            LaunchAttack();
        }

        // Check for enemy contact
        if (hasLaunched && player.enemyDetected && player.enemyHit.collider != null)
        {
            Enemy hitEnemy = player.enemyHit.collider.GetComponent<Enemy>();
            if (hitEnemy != null)
            {
                player.AttachToEnemy(hitEnemy);
                stateMachine.ChangeState(player.attachedState);
                return;
            }
        }

        // Fall if grounded after launch
        if (hasLaunched && player.groundDetected)
        {
            stateMachine.ChangeState(player.fallState);
        }
    }

    private void LaunchAttack()
    {
        hasLaunched = true;
        float chargeRatio = chargeTime / maxChargeTime;
        float jumpForce = Mathf.Lerp(minJumpForce, maxJumpForce, chargeRatio);
        
        // Apply jump force in attack direction
        Vector2 launchVelocity = attackDirection * jumpForce;
        player.SetVelocity(launchVelocity.x, launchVelocity.y);
    }
}

