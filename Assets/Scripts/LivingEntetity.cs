using UnityEngine;

public class LivingEntetity : MonoBehaviour, IDamageable {
    private IDamageable _damageableImplementation;

    public float startHealth;

    public float health { get; protected set; }
    protected bool dead;

    public event System.Action OnDeath;
    
    protected virtual void Start()
    {
        health = startHealth;
    }

    public virtual void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirect)
    {
        TakeDamage(damage);
    }

    public virtual void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0 && !dead)
        {
            Die();
        }

    }
    public virtual void Die()
    {
        dead = true;
        if (OnDeath != null)
        {
            OnDeath();
        }
        GameObject.Destroy(gameObject);
    }
}
