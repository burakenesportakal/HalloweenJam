using UnityEngine;

public class Player_ControlledState : PlayerState
{
    private Enemy controlledEnemy;

    public Player_ControlledState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        controlledEnemy = player.currentControlledEnemy;
        if (controlledEnemy == null)
        {
            stateMachine.ChangeState(player.fallState);
            return;
        }

        // Position player relative to enemy
        Vector2 enemyPos = controlledEnemy.transform.position;
        Vector2 offset = new Vector2(0f, 0.5f);
        player.transform.position = enemyPos + offset;

        // Player is invisible/disabled while controlling
        player.rb.simulated = false;
    }

    public override void Update()
    {
        base.Update();

        if (controlledEnemy == null || controlledEnemy.IsDead())
        {
            Detach();
            return;
        }

        // Position player relative to enemy
        Vector2 enemyPos = controlledEnemy.transform.position;
        Vector2 offset = new Vector2(0f, 0.5f);
        player.transform.position = enemyPos + offset;

        // Control the enemy using player input
        Vector2 moveInput = player.moveInput;
        controlledEnemy.SetVelocity(moveInput.x * controlledEnemy.moveSpeed, controlledEnemy.rb.linearVelocity.y);

        // Handle enemy flipping
        if (moveInput.x != 0)
        {
            controlledEnemy.HandleFlip(moveInput.x);
        }

        // Attack input - make controlled enemy attack
        if (input.Player.Attack.WasPressedThisFrame())
        {
            Entity_Combat enemyCombat = controlledEnemy.GetComponent<Entity_Combat>();
            if (enemyCombat != null)
            {
                enemyCombat.PerformAttack();
            }
        }

        // Interaction input to detach
        if (input.Player.Interaction.WasPressedThisFrame())
        {
            Detach();
            return;
        }
    }

    public override void Exit()
    {
        base.Exit();
        
        // Re-enable player physics
        if (player.rb != null)
        {
            player.rb.simulated = true;
        }
    }

    private void Detach()
    {
        if (controlledEnemy != null)
        {
            controlledEnemy.SetControlled(false, null);
            controlledEnemy.EntityDeath(); // Kill the enemy
        }
        
        player.SetControlledEnemy(null);
        stateMachine.ChangeState(player.fallState);
    }
}

