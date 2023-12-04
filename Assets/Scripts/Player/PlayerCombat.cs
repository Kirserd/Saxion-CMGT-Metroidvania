using System.Collections;
using UnityEngine;

public class PlayerCombat : Damageable
{
	[SerializeField]
	private Attack _attack;
	[SerializeField]
	private float _attackDamage;
	[SerializeField]
	private float _attackCooldown;
	[SerializeField]
	private float _attackTime;

	private Vector2 _moveInput;
	private bool _isOnCD;

    private void Update()
	{
		#region INPUT HANDLER

		_moveInput.x = Input.GetKey(Controls.Get(InputAction.Right)) || Input.GetKey(Controls.GetAlt(InputAction.Right)) ? 1f :
					   Input.GetKey(Controls.Get(InputAction.Left)) || Input.GetKey(Controls.GetAlt(InputAction.Left)) ? -1f : 0f;
		_moveInput.y = Input.GetKey(Controls.Get(InputAction.Up)) || Input.GetKey(Controls.GetAlt(InputAction.Up)) ? 1f :
					   Input.GetKey(Controls.Get(InputAction.Down)) || Input.GetKey(Controls.GetAlt(InputAction.Down)) ? -1f : 0f;

		if (GameProgress.HasWeapon && Input.GetKeyDown(Controls.Get(InputAction.Attack)) || Input.GetKeyDown(Controls.GetAlt(InputAction.Attack)))
			OnAttackInput();

        #endregion
    }

	private void OnCollisionStay2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("Enemy"))
			DealDamage(4, collision.transform.position);
	}

	public override bool DealDamage(float amount, Vector3 callerPosition)
    {
        bool dealtDamage = base.DealDamage(amount, callerPosition);
		if (dealtDamage)
		{
			Vector2 knockbackDirection = (transform.position - callerPosition).normalized;
			PlayerLinks.instance.PlayerMovement.Knockback(knockbackDirection * new Vector2(3, 0.5f));
		}
		return dealtDamage;
	}

    private void OnAttackInput()
    {
		if (_isOnCD || PlayerLinks.instance.PlayerMovement.IsDashing)
			return;

		Vector2 normalizedMoveInput;
		Vector3 offset;
		
		if (_moveInput == Vector2.zero)
			normalizedMoveInput = (Vector2.right * transform.localScale.x).normalized;
		else if(_moveInput.y != 0)
			normalizedMoveInput = _moveInput * Vector2.up;
		else
			normalizedMoveInput = _moveInput * Vector2.right;

		offset = new Vector3(normalizedMoveInput.x, normalizedMoveInput.y) * 0.2f;
		normalizedMoveInput = new Vector2(normalizedMoveInput.y, -normalizedMoveInput.x);

		Attack attack = Instantiate(_attack, transform.position + offset, Quaternion.LookRotation(Vector3.forward, normalizedMoveInput));
		attack.DamageAmount = _attackDamage * (GameProgress.HasDamageUp? 1.6f : 1f);
		attack.transform.SetParent(transform);
		attack.AddDamageMask(gameObject);
		attack.OnAttackLanded += OnAttackLanded;

		StartCoroutine(nameof(AttackCooldown));
		StartCoroutine(nameof(SetIsAttacking));
	}

	private void OnAttackLanded(float zAxisRotation)
    {
		#region PLAYER MOVEMENT | KNOCKBACK
		Vector2 knockbackDirection = Quaternion.AngleAxis(zAxisRotation, Vector3.forward) * Vector2.right;
		PlayerLinks.instance.PlayerMovement.Knockback(knockbackDirection);
        #endregion
    }

	private IEnumerator SetIsAttacking()
	{
		#region PLAYER MOVEMENT | IS ATTACKING
		PlayerMovement playerMovement = PlayerLinks.instance.PlayerMovement;
		playerMovement.Rigidbody.velocity *= 0.5f;
		playerMovement.IsAttacking = true;
		yield return new WaitForSeconds(_attackTime);
		playerMovement.IsAttacking = false;
		#endregion
	}

	private IEnumerator AttackCooldown()
    {
		_isOnCD = true;
		yield return new WaitForSeconds(_attackCooldown);
		_isOnCD = false;
	}

	public void SetMaxHP(float amount) 
    {
		MaxHP = amount;
		HP = amount;
    }

	protected override void Death()
    {
        base.Death();
		PlayerLinks.instance.PlayerAnimator.SetDeathState();

		PlayerLinks.instance.PlayerAnimator.enabled = false;
		PlayerLinks.instance.PlayerMovement.enabled = false;
		PlayerLinks.instance.PlayerInteractor.enabled = false;
		PlayerLinks.instance.PlayerCombat.enabled = false;

		//TODO: animation instead of turning off the renderer
		GetComponent<Renderer>().enabled = false;
	}
}