using UnityEngine;

public class Damageable : MonoBehaviour, IDamageable
{
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

    private bool _isDead;

    protected virtual void Start()
    {
        MaxHP = _maxHp;
        HP = MaxHP;
        _isDead = false;
    }

    public bool DealDamage(float amount)
    {
        if (_isDead)
            return false;

        HP -= amount;
        return true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Attack attack = collision.gameObject.GetComponent<Attack>();
        if (attack is null || !attack.IsDealingDamageTo(gameObject))
            return;

        DealDamage(attack.DamageAmount);
        attack.AddDamageMask(collision.gameObject);
        attack.OnAttackLanded?.Invoke(attack.transform.rotation.eulerAngles.z);
    }

    protected virtual void Death() => _isDead = true;
}