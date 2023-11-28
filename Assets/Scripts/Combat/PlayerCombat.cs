using System.Collections;
using UnityEngine;

public class PlayerCombat : Damageable
{
	[SerializeField]
	private float _attackDamage;
	[SerializeField]
	private float _attackCooldown;
	[SerializeField]
	private float _attackTime;
	[Space(5)]
	[SerializeField]
	private bool _weaponEquipped;

	[SerializeField]
	private Attack _attack;

	private Vector2 _moveInput;
	private bool _isOnCD;

    private void Update()
	{
		#region INPUT HANDLER

		_moveInput.x = Input.GetKey(Controls.Get(InputAction.Right)) || Input.GetKey(Controls.GetAlt(InputAction.Right)) ? 1f :
					   Input.GetKey(Controls.Get(InputAction.Left)) || Input.GetKey(Controls.GetAlt(InputAction.Left)) ? -1f : 0f;
		_moveInput.y = Input.GetKey(Controls.Get(InputAction.Up)) || Input.GetKey(Controls.GetAlt(InputAction.Up)) ? 1f :
					   Input.GetKey(Controls.Get(InputAction.Down)) || Input.GetKey(Controls.GetAlt(InputAction.Down)) ? -1f : 0f;

		if (Input.GetKeyDown(Controls.Get(InputAction.Attack)) || Input.GetKeyDown(Controls.GetAlt(InputAction.Attack)))
			OnAttackInput();

        #endregion
    }

	private void OnAttackInput()
    {
		if (!_weaponEquipped || _isOnCD || PlayerLinks.instance.PlayerMovement.IsDashing)
			return;

		Vector2 normalizedMoveInput;
		Vector3 offset;
		
		if (_moveInput == Vector2.zero)
			normalizedMoveInput = (Vector2.right * transform.localScale.x).normalized;
		else
			normalizedMoveInput = _moveInput.normalized;

		offset = new Vector3(normalizedMoveInput.x, normalizedMoveInput.y) * 1.2f;

		Attack attack = Instantiate(_attack, transform.position + offset, Quaternion.LookRotation(Vector3.forward, normalizedMoveInput));
		attack.transform.SetParent(transform);
		attack.AddDamageMask(gameObject);
		attack.OnAttackLanded += OnAttackLanded;

		StartCoroutine(nameof(AttackCooldown));
		StartCoroutine(nameof(SetIsAttacking));
	}

	private void OnAttackLanded(float zAxisRotation)
    {
		#region PLAYER MOVEMENT | KNOCKBACK
		Vector2 knockbackDirection = Quaternion.AngleAxis(zAxisRotation, Vector3.forward) * -Vector2.up;
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

    protected override void Death()
    {
        base.Death();
		PlayerLinks.instance.PlayerAnimator.SetDeathState();

		PlayerLinks.instance.PlayerAnimator.enabled = false;
		PlayerLinks.instance.PlayerMovement.enabled = false;
		PlayerLinks.instance.PlayerInteractor.enabled = false;
		PlayerLinks.instance.PlayerCombat.enabled = false;
	}
}