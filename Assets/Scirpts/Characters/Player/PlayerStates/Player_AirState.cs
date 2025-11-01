using UnityEngine;

public class Player_AirState : PlayerState
{
    public Player_AirState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Update()
    {
        base.Update();

        if (player.moveInput.x != 0)
            player.SetVelocity(player.moveInput.x * (player.moveSpeed * player.inAirMoveMultiplier), rb.linearVelocity.y);
        else
            player.SetVelocity(0, rb.linearVelocity.y);

        // Allow attack input in air
        if (input.Player.Attack.WasPressedThisFrame())
            stateMachine.ChangeState(player.attackState);
    }
}
