using UnityEngine;

public interface IDamageable
{
    void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirect);
    void TakeDamage(float damage);

}
