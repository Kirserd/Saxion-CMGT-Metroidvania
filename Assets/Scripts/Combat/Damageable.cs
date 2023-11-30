using UnityEngine;
using System.Collections;

public class Damageable : MonoBehaviour, IDamageable
{
    [SerializeField]
    [Space(5)]
    private float _invincibilityFrames = 0.4f;

    [SerializeField]
    private float _maxHp;
    public float MaxHP { get; protected set; }

    private float _hp;
    public float HP 
    {
        get => _hp; 
        protected set
        {
            if (value <= 0)
            {
                _hp = 0;
                Death();
            }
            else if (value >= MaxHP)
                _hp = MaxHP;
            else
                _hp = value;         
        } 
    }

    protected bool _isInvincible;
    protected bool _isDead;

    protected virtual void Start()
    {
        MaxHP = _maxHp;
        HP = MaxHP;
        _isDead = false;
        _isInvincible = false;
    }

    public virtual bool DealDamage(float amount, Vector3 callerPosition)
    {
        if (_isDead || _isInvincible)
            return false;

        HP -= amount;
        StartCoroutine(nameof(InvincibilityFrames));
        return true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Attack attack = collision.gameObject.GetComponent<Attack>();
        if (attack is null || !attack.IsDealingDamageTo(gameObject))
            return;

        DealDamage(attack.DamageAmount, collision.transform.position);
        attack.AddDamageMask(collision.gameObject);
        attack.OnAttackLanded?.Invoke(attack.transform.rotation.eulerAngles.z);
    }

    private IEnumerator InvincibilityFrames()
    {
        _isInvincible = true;
        yield return new WaitForSeconds(_invincibilityFrames);
        _isInvincible = false;
    }

    protected virtual void Death() => _isDead = true;
}