using UnityEngine;
using System.Collections;

public class WalkNLeapEnemy : Enemy
{
    #region INSPECTOR
    [Space(20)]
 
    [Header("Attacks")]
    [SerializeField]
    private float _attackDist;

    [Space(15)]

    [Header("Jump")]
    public float JumpHeight;
    public float JumpTimeToApex;
    public float JumpCD;
    [Space(5)]
    public float JumpPreparationTime;

    [HideInInspector] public float JumpForce;
    [HideInInspector] public float GravityStrength;

    #endregion

    #region STATES

    public bool IsJumping { get; protected set; }

    public float LastOnGroundTime { get; private set; }

    public float LastWantedJumpTime { get; private set; }

    protected bool _isOnJumpCD;

    protected bool _isJumpCut;
    protected bool _isJumpFalling;

    protected bool _isFalling;

    #endregion

    #region CHECKS PARAMETERS
    [Header("Checks")]
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private Vector2 _groundCheckSize = new Vector2(0.49f, 0.03f);
    [Space(5)]
    [SerializeField] private Transform _wallPredictPoint;
    [SerializeField] private Transform _noGroundPredictPoint;
    [SerializeField] private Vector2 _wallCheckSize = new Vector2(0.5f, 1f);
    [Header("Layers & Tags")]
    [SerializeField] private LayerMask _groundLayer;
    #endregion

    protected override void Start()
    {
        base.Start();
        GravityStrength = -(2 * JumpHeight) / (JumpTimeToApex * JumpTimeToApex);
        GravityScale = GravityStrength / Physics2D.gravity.y;
        JumpForce = Mathf.Abs(GravityStrength) * JumpTimeToApex;
    }
    protected override void Update()
    {
        #region TIMERS
        LastOnGroundTime -= Time.deltaTime;
        LastWantedJumpTime -= Time.deltaTime;
        #endregion

        base.Update();
        CheckAttack();

        if (IsWillingToAttack)
            Attack();

        #region JUMP CHECKS
        if (IsJumping && Rigidbody.velocity.y < 0)
        {
            IsJumping = false;
            _isJumpFalling = true;
        }

        if (LastOnGroundTime > 0 && !IsJumping)
        {
            _isJumpCut = false;
            _isJumpFalling = false;
        }

        if (!IsKnockback && LastWantedJumpTime > 0)
        {
            if (CanJump())
            {
                Rigidbody.velocity *= Vector2.right;
                IsJumping = true;
                _isJumpCut = false;
                _isJumpFalling = false;
                Jump();
            }
        }
        #endregion

        #region COLLISION CHECKS
        if (!IsKnockback && !IsJumping)
        {
            if (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer))
                LastOnGroundTime = 0.05f;

            if (LastOnGroundTime > 0 && !IsKnockback && IsAggro)
            {
                if(Physics2D.OverlapBox(_wallPredictPoint.position, _wallCheckSize, 0, _groundLayer) ||
                  !Physics2D.OverlapBox(_noGroundPredictPoint.position, _wallCheckSize, 0, _groundLayer))
                {
                    if (Mathf.Abs(DirectionToDestination.x) > -DirectionToDestination.y)
                        LastWantedJumpTime = 0.05f;
                    else
                        DirectionToDestination = IsFacingRight ? Vector2.right : Vector2.left ;
                }
            } 
        }
        #endregion

        #region GRAVITY
        _isFalling = Rigidbody.velocity.y < 0;

        if (!_isKnockbackAttacking)
        {
            if (_isJumpCut)
            {
                SetGravityScale(GravityScale * 3);
                Rigidbody.velocity = new Vector2(Rigidbody.velocity.x, Mathf.Max(Rigidbody.velocity.y, -18));
            }
            else
                SetGravityScale(GravityScale);
        }
        else
            SetGravityScale(0);
        #endregion
    }

    protected virtual void Attack()
    {
        IsWillingToAttack = false;
        //TODO
    }
    private void CheckAttack() => IsWillingToAttack = CompareDistance(Target.position, _attackDist);

    #region MOVEMENT

    protected override void Move(float lerpAmount)
    {
        if (IsWillingToAttack)
            return;
        
        base.Move(lerpAmount);
        float targetSpeed = DirectionToDestination.x * MaxSpeed;
        targetSpeed = Mathf.Lerp(Rigidbody.velocity.x, targetSpeed, lerpAmount);

        #region Calculate AccelRate
        float accelRate;
        accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? AccelAmount : DeccelAmount;
        #endregion

        float speedDif = targetSpeed - Rigidbody.velocity.x;

        float movement = speedDif * accelRate;

        Rigidbody.AddForce(movement * Vector2.right, ForceMode2D.Force);
    }

    private void Jump()
    {
        LastWantedJumpTime = 0;
        LastOnGroundTime = 0;

        StartCoroutine(nameof(PerformJump));
        StartCoroutine(nameof(JumpCooldown));
    }

    private IEnumerator PerformJump()
    {
        _isPreventedFromMoving = true;
        yield return new WaitForSeconds(JumpPreparationTime);

        #region Perform Jump
        float force = JumpForce;
        if (Rigidbody.velocity.y < 0)
            force -= Rigidbody.velocity.y;

        Rigidbody.AddForce(Vector2.up * force, ForceMode2D.Impulse);
        #endregion

        _isPreventedFromMoving = false;
    }
    private IEnumerator JumpCooldown()
    {
        _isOnJumpCD = true;
        yield return new WaitForSeconds(JumpCD);
        _isOnJumpCD = false;
    }

    #endregion

    #region CHECKS
    private bool CanJump() => LastOnGroundTime > 0 && !_isOnJumpCD;
    #endregion

    #region DEBUG
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(_groundCheckPoint.position, _groundCheckSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(_wallPredictPoint.position, _wallCheckSize);
        Gizmos.DrawWireCube(_noGroundPredictPoint.position, _wallCheckSize);
    }
    #endregion
}
