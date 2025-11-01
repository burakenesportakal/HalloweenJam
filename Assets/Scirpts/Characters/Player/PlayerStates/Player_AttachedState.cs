using UnityEngine;

public class Player_AttachedState : PlayerState
{
    private Enemy attachedEnemy;
    private PhysicsMaterial2D highFrictionMaterial;
    private PhysicsMaterial2D originalMaterial;
    private CapsuleCollider2D capsuleCollider;

    public Player_AttachedState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        attachedEnemy = player.currentControlledEnemy;
        if (attachedEnemy == null)
        {
            stateMachine.ChangeState(player.fallState);
            return;
        }

        // Get capsule collider
        capsuleCollider = player.GetComponent<CapsuleCollider2D>();
        if (capsuleCollider != null)
        {
            // Store original material
            originalMaterial = capsuleCollider.sharedMaterial;
            
            // Create high friction material programmatically
            highFrictionMaterial = new PhysicsMaterial2D("HighFriction");
            highFrictionMaterial.friction = 10f; // Maximum friction
            highFrictionMaterial.bounciness = 0f;
            
            // Apply high friction material
            capsuleCollider.sharedMaterial = highFrictionMaterial;
        }

        // Stop player movement
        player.SetVelocity(0, 0);
        rb.gravityScale = 0f; // Disable gravity while attached

        // Mark enemy as controlled
        attachedEnemy.SetControlled(true, player);
    }

    public override void Update()
    {
        base.Update();

        if (attachedEnemy == null || attachedEnemy.IsDead())
        {
            Detach();
            return;
        }

        // Position player relative to enemy
        Vector2 enemyPos = attachedEnemy.transform.position;
        Vector2 offset = new Vector2(0f, 0.5f); // Offset above enemy
        player.transform.position = enemyPos + offset;

        // Check for interaction input to detach
        if (input.Player.Interaction.WasPressedThisFrame())
        {
            Detach();
            return;
        }

        // Transition to controlled state after a brief moment
        // This allows the player to immediately start controlling
        stateMachine.ChangeState(player.controlledState);
    }

    public override void Exit()
    {
        base.Exit();
        
        // Restore original physics material
        if (capsuleCollider != null && originalMaterial != null)
        {
            capsuleCollider.sharedMaterial = originalMaterial;
        }

        // Restore gravity
        if (rb != null)
        {
            rb.gravityScale = 1f;
        }
    }

    private void Detach()
    {
        if (attachedEnemy != null)
        {
            attachedEnemy.SetControlled(false, null);
            attachedEnemy.EntityDeath(); // Kill the enemy
        }
        
        player.SetControlledEnemy(null);
        stateMachine.ChangeState(player.fallState);
    }
}

