using UnityEngine;

public interface IDamageable
{
    public bool DealDamage(float amount, Vector3 callerPosition);
}
