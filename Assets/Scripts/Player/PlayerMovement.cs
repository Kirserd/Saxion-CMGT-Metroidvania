using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
	public PlayerData Data;

	#region COMPONENTS
    public Rigidbody2D Rigidbody { get; private set; }
	public PlayerAnimator AnimatorHandler { get; private set; }

	#endregion

	#region STATE PARAMETERS
	public bool IsAttacking { get; set; }
	public bool IsFacingRight { get; private set; }
	public bool IsJumping { get; private set; }
	public bool IsWallJumping { get; private set; }
	public bool IsDashing { get; private set; }
	public bool IsSliding { get; private set; }
	public bool IsGliding { get; private set; }

	public float LastOnGroundTime { get; private set; }
	public float LastOnWallTime { get; private set; }
	public float LastOnWallRightTime { get; private set; }
	public float LastOnWallLeftTime { get; private set; }

	private int _jumpsLeft;
	private bool _isJumpCut;
	private bool _isJumpFalling;

	private float _wallJumpStartTime;
	private int _lastWallJumpDir;

	private bool _isFalling;

	private int _dashesLeft;
	private bool _dashRefilling;
	private Vector2 _lastDashDir;
	private bool _isDashAttacking;
	#endregion

	#region INPUT PARAMETERS
	private Vector2 _moveInput;

	public float LastPressedJumpTime { get; private set; }
	public float LastPressedDashTime { get; private set; }
	#endregion

	#region CHECK PARAMETERS
	[Header("Checks")] 
	[SerializeField] private Transform _groundCheckPoint;
	[SerializeField] private Vector2 _groundCheckSize = new Vector2(0.49f, 0.03f);
	[Space(5)]
	[SerializeField] private Transform _frontWallCheckPoint;
	[SerializeField] private Transform _backWallCheckPoint;
	[SerializeField] private Vector2 _wallCheckSize = new Vector2(0.5f, 1f);
    #endregion

    #region LAYERS & TAGS
    [Header("Layers & Tags")]
	[SerializeField] private LayerMask _groundLayer;
	#endregion

    private void Awake()
	{
		Rigidbody = GetComponent<Rigidbody2D>();
		AnimatorHandler = GetComponent<PlayerAnimator>();
	}

	private void Start()
	{
		SetGravityScale(Data.GravityScale);
		IsFacingRight = true;
	}

	private void Update()
	{
		#region TIMERS
		LastOnGroundTime -= Time.deltaTime;
		LastOnWallTime -= Time.deltaTime;
		LastOnWallRightTime -= Time.deltaTime;
		LastOnWallLeftTime -= Time.deltaTime;

		LastPressedJumpTime -= Time.deltaTime;
		LastPressedDashTime -= Time.deltaTime;
		#endregion

		#region INPUT HANDLER

		_moveInput.x = Input.GetKey(Controls.Get(InputAction.Right)) || Input.GetKey(Controls.GetAlt(InputAction.Right)) ? 1f :
					   Input.GetKey(Controls.Get(InputAction.Left)) || Input.GetKey(Controls.GetAlt(InputAction.Left)) ? -1f : 0f;
		_moveInput.y = Input.GetKey(Controls.Get(InputAction.Up)) || Input.GetKey(Controls.GetAlt(InputAction.Up)) ? 1f :
					   Input.GetKey(Controls.Get(InputAction.Down)) || Input.GetKey(Controls.GetAlt(InputAction.Down)) ? -1f : 0f;

		if (_moveInput.x != 0)
			CheckDirectionToFace(_moveInput.x > 0);

		if (!IsAttacking)
		{
			if (Input.GetKeyDown(Controls.Get(InputAction.Jump)) || Input.GetKeyDown(Controls.GetAlt(InputAction.Jump)))
				OnJumpInput();

			if (Input.GetKeyUp(Controls.Get(InputAction.Jump)) || Input.GetKeyUp(Controls.GetAlt(InputAction.Jump)))
				OnJumpUpInput();

			if (Input.GetKeyDown(Controls.Get(InputAction.Dash)) || Input.GetKeyDown(Controls.GetAlt(InputAction.Dash)))
				OnDashInput();

			if (CanGlide() && Input.GetKey(Controls.Get(InputAction.Jump)) || Input.GetKey(Controls.GetAlt(InputAction.Jump)))
				OnGlideInput();
			else
				OnGlideStopInput();
		}

		#endregion

		#region COLLISION CHECKS
		if (!IsDashing && !IsJumping)
		{
			if (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer))
			{
				if(LastOnGroundTime < -0.1f)
					AnimatorHandler._justLanded = true;

				_jumpsLeft = Data.JumpAmount;
				LastOnGroundTime = Data.CoyoteTime;
            }		

			if (((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && IsFacingRight)
				|| (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && !IsFacingRight)) && !IsWallJumping)
				LastOnWallRightTime = Data.CoyoteTime;

			if (((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && !IsFacingRight)
				|| (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && IsFacingRight)) && !IsWallJumping)
				LastOnWallLeftTime = Data.CoyoteTime;

			LastOnWallTime = Mathf.Max(LastOnWallLeftTime, LastOnWallRightTime);
		}
		#endregion

		#region JUMP CHECKS
		if (IsJumping && Rigidbody.velocity.y < 0)
		{
			IsJumping = false;
			_isJumpFalling = true;
		}

		if (IsWallJumping && Time.time - _wallJumpStartTime > Data.WallJumpTime)
			IsWallJumping = false;

		if (LastOnGroundTime > 0 && !IsJumping && !IsWallJumping)
        {
			_isJumpCut = false;
			_isJumpFalling = false;
		}

		if(LastOnGroundTime <= 0 && _jumpsLeft == Data.JumpAmount)
			_jumpsLeft--;

		if (!IsDashing && LastPressedJumpTime > 0)
		{
			if (CanJump())
			{
				Rigidbody.velocity *= Vector2.right;
				_jumpsLeft--;
				IsJumping = true;
				IsWallJumping = false;
				_isJumpCut = false;
				_isJumpFalling = false;
				Jump();

				AnimatorHandler._startedJumping = true;
			}
			else if (CanWallJump())
			{
				Rigidbody.velocity *= Vector2.right;
				IsWallJumping = true;
				IsJumping = false;
				_isJumpCut = false;
				_isJumpFalling = false;

				_wallJumpStartTime = Time.time;
				_lastWallJumpDir = (LastOnWallRightTime > 0) ? -1 : 1;

				WallJump(_lastWallJumpDir);
			}
			else if(CanAirJump())
			{
				Rigidbody.velocity *= Vector2.right;
				_jumpsLeft--;
				IsJumping = true;
				IsWallJumping = false;
				_isJumpCut = false;
				_isJumpFalling = false;
				Jump();

				AnimatorHandler._startedJumping = true;
			}
		}
		#endregion

		#region DASH CHECKS
		if (CanDash() && LastPressedDashTime > 0)
		{
			Sleep(Data.DashSleepTime); 

			if (_moveInput != Vector2.zero)
				_lastDashDir = _moveInput * Vector2.right;
			else
				_lastDashDir = IsFacingRight ? Vector2.right : Vector2.left;

			IsDashing = true;
			IsJumping = false;
			IsWallJumping = false;
			_isJumpCut = false;

			StartCoroutine(nameof(StartDash), _lastDashDir);
		}
		#endregion

		#region SLIDE CHECKS
		if (CanSlide() && ((LastOnWallLeftTime > 0 && _moveInput.x < 0) || (LastOnWallRightTime > 0 && _moveInput.x > 0)))
			IsSliding = true;
		else
			IsSliding = false;
		#endregion

		#region GRAVITY
		_isFalling = Rigidbody.velocity.y < 0;

		if (!_isDashAttacking)
		{
			if (IsGliding)
			{
				SetGravityScale(Data.GravityScale);
				Rigidbody.velocity = new Vector2(Rigidbody.velocity.x, Mathf.Max(Rigidbody.velocity.y, -Data.MaxGlideSpeed));
			}

			else if (IsSliding)
				SetGravityScale(0);

			else if (Rigidbody.velocity.y < 0 && _moveInput.y < 0)
			{
				SetGravityScale(Data.GravityScale * Data.FastFallGravityMult);
				Rigidbody.velocity = new Vector2(Rigidbody.velocity.x, Mathf.Max(Rigidbody.velocity.y, -Data.MaxFastFallSpeed));
			}
			else if (_isJumpCut)
			{
				SetGravityScale(Data.GravityScale * Data.JumpCutGravityMultiplier);
				Rigidbody.velocity = new Vector2(Rigidbody.velocity.x, Mathf.Max(Rigidbody.velocity.y, -Data.MaxFallSpeed));
			}
			else if ((IsJumping || IsWallJumping || _isJumpFalling) && Mathf.Abs(Rigidbody.velocity.y) < Data.JumpHangTimeThreshold)
				SetGravityScale(Data.GravityScale * Data.JumpHangGravityMultiplier);

			else if (Rigidbody.velocity.y < 0)
			{
				SetGravityScale(Data.GravityScale * Data.FallGravityMult);
				Rigidbody.velocity = new Vector2(Rigidbody.velocity.x, Mathf.Max(Rigidbody.velocity.y, -Data.MaxFallSpeed));
			}
			else
				SetGravityScale(Data.GravityScale);
		}
		else
			SetGravityScale(0);
		#endregion
    }

    private void FixedUpdate()
	{
		if (!IsDashing)
		{
			if (IsWallJumping)
				Run(Data.WallJumpRunLerp);
			else
				Run(1);
		}
		else if (_isDashAttacking)
			Run(Data.DashEndRunLerp);

		if (IsSliding)
			Slide();
    }

    #region INPUT CALLBACKS
    public void OnJumpInput() => LastPressedJumpTime = Data.JumpInputBufferTime;

	public void OnJumpUpInput()
	{
		if (CanJumpCut() || CanWallJumpCut())
			_isJumpCut = true;
	}

	public void OnDashInput() => LastPressedDashTime = Data.DashInputBufferTime;

	public void OnGlideInput() => IsGliding = LastPressedJumpTime <= 0;
	public void OnGlideStopInput() => IsGliding = false;
	#endregion

	#region GENERAL
	public void SetGravityScale(float scale) => Rigidbody.gravityScale = scale;
	private void Sleep(float duration) => StartCoroutine(nameof(PerformSleep), duration);

	private IEnumerator PerformSleep(float duration)
    {
		Time.timeScale = 0;
		yield return new WaitForSecondsRealtime(duration);  
		Time.timeScale = 1;
	}
    #endregion

    #region MOVEMENT

    #region RUN
    private void Run(float lerpAmount)
	{
		float targetSpeed = _moveInput.x * Data.RunMaxSpeed;
		targetSpeed = Mathf.Lerp(Rigidbody.velocity.x, targetSpeed, lerpAmount);

		#region Calculate AccelRate
		float accelRate;

		if (LastOnGroundTime > 0)
			accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.RunAccelAmount : Data.RunDeccelAmount;
		else
			accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.RunAccelAmount * Data.AccelerationInAir : Data.RunDeccelAmount * Data.DeccelerationInAir;
		#endregion

		#region Add Bonus Jump Apex Acceleration
		if ((IsJumping || IsWallJumping || _isJumpFalling) && Mathf.Abs(Rigidbody.velocity.y) < Data.JumpHangTimeThreshold)
		{
			accelRate *= Data.JumpHangAccelerationMultiplier;
			targetSpeed *= Data.JumpHangMaxSpeedMultiplier;
		}
		#endregion

		#region Conserve Momentum
		if(Data.ConserveMomentum && Mathf.Abs(Rigidbody.velocity.x) > Mathf.Abs(targetSpeed) 
			&& Mathf.Sign(Rigidbody.velocity.x) == Mathf.Sign(targetSpeed) 
			&& Mathf.Abs(targetSpeed) > 0.01f && LastOnGroundTime < 0)
			accelRate = 0; 

		#endregion

		float speedDif = targetSpeed - Rigidbody.velocity.x;

		float movement = speedDif * accelRate;

		Rigidbody.AddForce(movement * Vector2.right, ForceMode2D.Force);
	}

	private void Turn()
	{
		Vector3 scale = transform.localScale; 
		scale.x *= -1;
		transform.localScale = scale;

		IsFacingRight = !IsFacingRight;
	}
    #endregion

    #region JUMP
    private void Jump()
	{
		LastPressedJumpTime = 0;
		LastOnGroundTime = 0;

		#region Perform Jump
		float force = Data.JumpForce;
		if (Rigidbody.velocity.y < 0)
			force -= Rigidbody.velocity.y;

		Rigidbody.AddForce(Vector2.up * force, ForceMode2D.Impulse);
		#endregion
	}

	private void WallJump(int dir)
	{
		LastPressedJumpTime = 0;
		LastOnGroundTime = 0;
		LastOnWallRightTime = 0;
		LastOnWallLeftTime = 0;

		#region Perform Wall Jump
		Vector2 force = new Vector2(Data.WallJumpForce.x, Data.WallJumpForce.y);
		force.x *= dir;

		if (Mathf.Sign(Rigidbody.velocity.x) != Mathf.Sign(force.x))
			force.x -= Rigidbody.velocity.x;

		if (Rigidbody.velocity.y < 0)
			force.y -= Rigidbody.velocity.y;

		Rigidbody.AddForce(force, ForceMode2D.Impulse);
		#endregion
	}
	#endregion

	#region DASH
	private IEnumerator StartDash(Vector2 dir)
	{
		LastOnGroundTime = 0;
		LastPressedDashTime = 0;

		float startTime = Time.time;

		_dashesLeft--;
		_isDashAttacking = true;

		SetGravityScale(0);

		while (Time.time - startTime <= Data.DashAttackTime)
		{
			Rigidbody.velocity = dir.normalized * Data.DashSpeed;
			yield return null;
		}

		startTime = Time.time;

		_isDashAttacking = false;

		SetGravityScale(Data.GravityScale);
		Rigidbody.velocity = Data.DashEndSpeed * dir.normalized;

		while (Time.time - startTime <= Data.DashEndTime)
		{
			yield return null;
		}

		IsDashing = false;
	}

	private IEnumerator RefillDash(int amount)
	{
		_dashRefilling = true;
		yield return new WaitForSeconds(Data.DashRefillTime);
		_dashRefilling = false;
		_dashesLeft = Mathf.Min(Data.DashAmount, _dashesLeft + 1);
	}
	#endregion

	#region SLIDE
	private void Slide()
	{
		if(Rigidbody.velocity.y > 0)
		    Rigidbody.AddForce(-Rigidbody.velocity.y * Vector2.up,ForceMode2D.Impulse);
	
		float speedDif = Data.SlideSpeed - Rigidbody.velocity.y;	
		float movement = speedDif * Data.SlideAccel;

		movement = Mathf.Clamp(movement, -Mathf.Abs(speedDif)  * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime));

		Rigidbody.AddForce(movement * Vector2.up);
	}
	#endregion

	#region KNOCKBACK
	public void Knockback(Vector2 dir)
    {
		IsDashing = true;
		IsJumping = false;
		IsWallJumping = false;
		_isJumpCut = false;

		StartCoroutine(nameof(StartKnockback), dir);
	}
	private IEnumerator StartKnockback(Vector2 dir)
	{
		float startTime = Time.time;

		_isDashAttacking = true;

		SetGravityScale(0);

		while (Time.time - startTime <= Data.KnockbackAttackTime)
		{
			Rigidbody.velocity = dir * Data.KnockbackSpeed;
			yield return null;
		}

		startTime = Time.time;

		_isDashAttacking = false;

		SetGravityScale(Data.GravityScale);
		Rigidbody.velocity = Data.KnockbackEndSpeed * dir;

		while (Time.time - startTime <= Data.KnockbackEndTime)
		{
			yield return null;
		}

		IsDashing = false;
	}
	#endregion

	#endregion


	#region CHECKS
	public void CheckDirectionToFace(bool isMovingRight)
	{
		if (isMovingRight != IsFacingRight)
			Turn();
	}

    private bool CanJump() => LastOnGroundTime > 0 && _jumpsLeft == Data.JumpAmount;
	private bool CanAirJump() => GameProgress.HasAirJump && _jumpsLeft > 0;

	private bool CanWallJump() => GameProgress.HasWallJump && LastPressedJumpTime > 0 && LastOnWallTime > 0 && LastOnGroundTime <= 0 && (!IsWallJumping ||
								(LastOnWallRightTime > 0 && _lastWallJumpDir == 1) || (LastOnWallLeftTime > 0 && _lastWallJumpDir == -1));

	private bool CanJumpCut() => IsJumping && Rigidbody.velocity.y > 0;

	private bool CanWallJumpCut() => IsWallJumping && Rigidbody.velocity.y > 0;

	private bool CanDash()
	{
		if (!GameProgress.HasDash)
			return false;

		if (!IsDashing && _dashesLeft < Data.DashAmount && !_dashRefilling)
			StartCoroutine(nameof(RefillDash), 1);

		return _dashesLeft > 0;
	}

	private bool CanSlide() => GameProgress.HasWallJump && (LastOnWallTime > 0 && !IsJumping && !IsWallJumping && !IsDashing && LastOnGroundTime <= 0);

	private bool CanGlide() => GameProgress.HasGlide && _isFalling;
    #endregion


    #region DEBUG
    private void OnDrawGizmosSelected()
    {
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(_groundCheckPoint.position, _groundCheckSize);
		Gizmos.color = Color.blue;
		Gizmos.DrawWireCube(_frontWallCheckPoint.position, _wallCheckSize);
		Gizmos.DrawWireCube(_backWallCheckPoint.position, _wallCheckSize);
	}
    #endregion
}
