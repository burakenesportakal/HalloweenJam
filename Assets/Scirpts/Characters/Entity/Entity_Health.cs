using System;
using UnityEngine;
using UnityEngine.UI;
public class Entity_Health : MonoBehaviour
{
    //private Slider healthSlider;
    private Entity_VFX entityVFX;
    private Entity entity;

    [SerializeField] protected float currentHealth;
    [SerializeField] protected float maxHealth = 100;
    [SerializeField] protected bool isDead;

    [Header("Knockback Details")]
    [SerializeField] private Vector2 knockbackPower = new Vector2(1.5f, 2.5f);
    [SerializeField] private Vector2 heavyKnockbackPower = new Vector2(7, 7);
    [SerializeField] private float knockbackDuration = .2f;
    [SerializeField] private float heavyKnockbackDuration = .5f;
    // Heavy Damage
    [SerializeField] private float heavyDamageThreshold = .3f;

    protected virtual void Awake()
    {
        entityVFX = GetComponent<Entity_VFX>();
        entity = GetComponent<Entity>();
        //healthSlider = GetComponentInChildren<Slider>();

        currentHealth = maxHealth;
        //UpdateHealthSlider();
    }
    public virtual void TakeDamage(float damage, Transform damageDealer)
    {
        if (isDead) return;

        Vector2 knockback = CalculateKnockback(damage, damageDealer);
        float duration = CalculateDuration(damage);

        entity?.ReciveKnockback(knockback, duration);
        entityVFX?.PlayOnDamageVFX();
        ReduceHP(damage);
    }
    protected virtual void ReduceHP(float damage)
    {
        currentHealth -= damage;
        //UpdateHealthSlider();

        if (currentHealth <= 0)
            Die();
    }
    private void Die()
    {
        isDead = true;
        entity.EntityDeath();
    }

    public bool IsDead() => isDead;

    //private void UpdateHealthSlider() => healthSlider.value = currentHealth / maxHealth;

    private Vector2 CalculateKnockback(float damage, Transform damageDealer)
    {
        int direction = transform.position.x > damageDealer.position.x ? 1 : -1;
        Vector2 knockback = IsHeavyDamage(damage) ? heavyKnockbackPower : knockbackPower;
        knockback.x *= direction;

        return knockback;
    }
    private float CalculateDuration(float damage) => IsHeavyDamage(damage) ? heavyKnockbackDuration : knockbackDuration;
    private bool IsHeavyDamage(float damage) => damage / maxHealth > heavyDamageThreshold;  
}
