using UnityEngine;

public class Enemy_DeadState : EnemyState
{
    public Enemy_DeadState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        anim.enabled = false;

        rb.gravityScale = 10;
        rb.linearVelocity = new Vector2(2, 15);

        enemy.GetComponent<Collider2D>().enabled = false;

        stateMachine.OffStateMachine();
    }
}
