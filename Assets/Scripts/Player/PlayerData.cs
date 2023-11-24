using UnityEngine;

[CreateAssetMenu(menuName = "Player Data")]
public class PlayerData : ScriptableObject
{
	[Header("Gravity")]
	[HideInInspector] public float GravityStrength; 
	[HideInInspector] public float GravityScale; 
	[Space(5)]
	public float FallGravityMult;
	public float MaxFallSpeed;
	[Space(5)]
	public float FastFallGravityMult; 
	public float MaxFastFallSpeed; 

	[Space(20)]

	[Header("Run")]
	public float RunMaxSpeed; 
	public float RunAcceleration; 
	[HideInInspector] public float RunAccelAmount; 
	public float RunDecceleration; 
	[HideInInspector] public float RunDeccelAmount;
	[Space(5)]
	[Range(0f, 1)] public float AccelerationInAir; 
	[Range(0f, 1)] public float DeccelerationInAir;
	[Space(5)]
	public bool ConserveMomentum = true;

	[Space(20)]

	[Header("Jump")]
	public int JumpAmount;
	public float JumpHeight; 
	public float JumpTimeToApex; 
	[HideInInspector] public float JumpForce;

	[Header("Both Jumps")]
	public float JumpCutGravityMultiplier; 
	[Range(0f, 1)] public float JumpHangGravityMultiplier; 
	public float JumpHangTimeThreshold;
	[Space(0.5f)]
	public float JumpHangAccelerationMultiplier; 
	public float JumpHangMaxSpeedMultiplier; 				

	[Header("Wall Jump")]
	public Vector2 WallJumpForce; 
	[Space(5)]
	[Range(0f, 1f)] public float WallJumpRunLerp; 
	[Range(0f, 1.5f)] public float WallJumpTime; 
	public bool TurnOnWallJump;

	[Space(20)]

	[Header("Slide")]
	public float SlideSpeed;
	public float SlideAccel;

    [Header("Assists")]
	[Range(0.01f, 0.5f)] public float CoyoteTime;
	[Range(0.01f, 0.5f)] public float JumpInputBufferTime;

	[Space(20)]

	[Header("Dash")]
	public int DashAmount;
	public float DashSpeed;
	public float DashSleepTime; 
	[Space(5)]
	public float DashAttackTime;
	[Space(5)]
	public float DashEndTime;
	public Vector2 DashEndSpeed; 
	[Range(0f, 1f)] public float DashEndRunLerp;
	[Space(5)]
	public float DashRefillTime;
	[Space(5)]
	[Range(0.01f, 0.5f)] public float DashInputBufferTime;

	[Space(20)]

	[Header("Glide")]
	public float MaxGlideSpeed;


	private void OnValidate()
    {
		GravityStrength = -(2 * JumpHeight) / (JumpTimeToApex * JumpTimeToApex);
		GravityScale = GravityStrength / Physics2D.gravity.y;

		RunAccelAmount = (50 * RunAcceleration) / RunMaxSpeed;
		RunDeccelAmount = (50 * RunDecceleration) / RunMaxSpeed;

		JumpForce = Mathf.Abs(GravityStrength) * JumpTimeToApex;

		#region Variable Ranges
		RunAcceleration = Mathf.Clamp(RunAcceleration, 0.01f, RunMaxSpeed);
		RunDecceleration = Mathf.Clamp(RunDecceleration, 0.01f, RunMaxSpeed);
		#endregion
	}
}