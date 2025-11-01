using UnityEngine;

public class Enemy_Health : Entity_Health
{
    private Enemy enemy => GetComponent<Enemy>();
    public override void TakeDamage(float damage, Transform damageDealer)
    {
        base.TakeDamage(damage, damageDealer);

        if (isDead) return;
        
        // Enter battle state if attacked by player
        if(damageDealer.CompareTag("Player"))
        {
            enemy.TryEnterBattleState(damageDealer);
        }
        // Enter battle state if attacked by a controlled enemy (and we can attack them back)
        else
        {
            Enemy attackerEnemy = damageDealer.GetComponent<Enemy>();
            if (attackerEnemy != null && attackerEnemy.IsControlled() && attackerEnemy.HasAttackedWhileControlled())
            {
                // Can now attack back - enter battle state targeting the player controlling the enemy
                Player controllingPlayer = attackerEnemy.GetControllingPlayer();
                if (controllingPlayer != null)
                {
                    enemy.TryEnterBattleState(controllingPlayer.transform);
                }
            }
        }
    }
}
