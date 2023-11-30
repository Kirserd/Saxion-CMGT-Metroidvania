using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Enemy : Damageable
{
    #region INSPECTOR

    [Header("Distance Predicates")]
    [SerializeField] protected float AggroDist;
    [SerializeField] protected float StopAggroDist;

    [Space(15)]

    [Header("Gravity")]
    [HideInInspector] protected float GravityScale;

    [Space(15)]

    [Header("Movement")]
    [SerializeField] protected float MaxSpeed;
    [SerializeField] protected float AccelAmount;
    [SerializeField] protected float DeccelAmount;

    [Space(15)]

    [Header("Knockback")]
    [SerializeField] protected Vector2 KnockbackModifier;
    [SerializeField] protected float KnockbackSpeed;
    [Space(5)]
    [SerializeField] protected float KnockbackAttackTime;
    [Space(5)]
    [SerializeField] protected float KnockbackEndTime;
    [SerializeField] protected Vector2 KnockbackEndSpeed;
    [Range(0f, 1f)]
    [SerializeField] protected float KnockbackEndRunLerp;

    #endregion

    #region STATES
    public bool IsKnockback { get; protected set; }
    public bool IsAggro { get; protected set; }
    public bool IsWillingToAttack { get; protected set; }

    public bool IsFacingRight { get; protected set; }

    protected bool _isKnockbackAttacking;
    protected bool _isPreventedFromMoving;

    #endregion

    #region MOVEMENT
    protected Transform Target;
    protected Vector2 DirectionToDestination;
    #endregion

    #region COMPONENTS
    protected Rigidbody2D Rigidbody;
    protected Collider2D Collider;
    #endregion

    protected override void Start()
    {
        base.Start();
        Target = PlayerLinks.instance.transform;
        GravityScale = -Physics2D.gravity.y;
        Rigidbody = GetComponent<Rigidbody2D>();
        Collider = GetComponent<Collider2D>();
    }

    public override bool DealDamage(float amount, Vector3 callerPosition)
    {
        bool dealtDamage = base.DealDamage(amount, callerPosition);
        if (dealtDamage)
        {
            Vector2 knockbackDirection = (transform.position - callerPosition).normalized;
            Knockback(knockbackDirection * KnockbackModifier);
        }
        return dealtDamage;
    }


    protected virtual void Update()
    {
        CheckAggro();
        if(IsAggro)
            DirectionToDestination = (Target.position - transform.position).normalized;
        else
            DirectionToDestination = Vector2.zero;

        CheckDirectionToFace(DirectionToDestination.x > 0);
    }

    private void CheckAggro()
    {
        if (!IsAggro && CompareDistance(Target.position, AggroDist))
            IsAggro = true;
        else if (IsAggro && !CompareDistance(Target.position, StopAggroDist))
            IsAggro = false;
    }

    protected bool CompareDistance(Vector3 target, float predicate) 
        => Vector3.Distance(transform.position, target) <= predicate;

    private void FixedUpdate()
    {
        if (_isPreventedFromMoving)
            return;

        if (!IsKnockback)
        {
           Move(1);
        }
        else if (_isKnockbackAttacking)
           Move(KnockbackEndRunLerp);
    }

    protected virtual void Move(float lerpAmount){}

    protected void Turn()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        IsFacingRight = !IsFacingRight;
    }

    public void CheckDirectionToFace(bool isMovingRight)
    {
        if (isMovingRight != IsFacingRight)
            Turn();
    }

    public void SetGravityScale(float scale) => Rigidbody.gravityScale = scale;

    public void Knockback(Vector2 dir)
    {
        IsKnockback = true;

        StartCoroutine(nameof(StartKnockback), dir);
    }

    private IEnumerator StartKnockback(Vector2 dir)
    {
        float startTime = Time.time;

        _isKnockbackAttacking = true;

        SetGravityScale(0);

        while (Time.time - startTime <= KnockbackAttackTime)
        {
            Rigidbody.velocity = dir * KnockbackSpeed;
            yield return null;
        }

        startTime = Time.time;

        _isKnockbackAttacking = false;

        SetGravityScale(GravityScale);
        Rigidbody.velocity = KnockbackEndSpeed * dir;

        while (Time.time - startTime <= KnockbackEndTime)
        {
            yield return null;
        }

        IsKnockback = false;
    }

    protected override void Death()
    {
        base.Death();
        Destroy(gameObject);
    }
}
