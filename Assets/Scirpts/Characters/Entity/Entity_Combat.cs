using UnityEngine;

public class Entity_Combat : MonoBehaviour
{
    private Entity_VFX entityVFX;
    public Collider2D[] targetColliders;

    [Header("Target Detection")]
    [SerializeField] private Transform targetCheck;
    [SerializeField] private float targetCheckRadius;
    [SerializeField] LayerMask whatIsTarget;

    [Header("Combat Details")]
    [SerializeField] private float attackDamage = 10;

    private void Awake()
    {
        entityVFX = GetComponent<Entity_VFX>();
    }
    public void PerformAttack()
    {
        Enemy attackerEnemy = GetComponent<Enemy>();
        bool isControlledAttacker = attackerEnemy != null && attackerEnemy.IsControlled();
        
        foreach (var target in GetDetectedColliders())
        {
            Enemy targetEnemy = target.GetComponent<Enemy>();
            
            // If attacking a controlled enemy, check if we can attack them
            if (targetEnemy != null && targetEnemy.IsControlled())
            {
                // Can only attack controlled enemy if:
                // 1. We are also controlled (controlled enemy vs controlled enemy)
                // 2. The controlled enemy has attacked us first (tracked via HasAttackedWhileControlled)
                if (!isControlledAttacker && !targetEnemy.HasAttackedWhileControlled())
                {
                    continue; // Skip this target - can't attack controlled enemy yet
                }
            }
            
            Entity_Health targetHealth = target.GetComponent<Entity_Health>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(attackDamage, transform);
                
                // If attacker is controlled, mark that it has attacked
                if (isControlledAttacker)
                {
                    attackerEnemy.SetHasAttackedWhileControlled(true);
                    
                    // If target is also an enemy, mark that they can now attack back
                    if (targetEnemy != null && !targetEnemy.IsControlled())
                    {
                        // Allow this enemy to attack the controlled enemy
                        Enemy controlledEnemy = attackerEnemy;
                        if (targetEnemy.GetComponent<Enemy>() != null)
                        {
                            // Target can now attack the controlled enemy
                        }
                    }
                }
            }
        }
    }

    protected Collider2D[] GetDetectedColliders()
    {
        return targetColliders = Physics2D.OverlapCircleAll(targetCheck.position, targetCheckRadius, whatIsTarget);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(targetCheck.position, targetCheckRadius);
    }
}