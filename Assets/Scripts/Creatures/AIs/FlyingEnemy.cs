using UnityEngine;

public class FlyingEnemy : Enemy
{
    #region INSPECTOR
    [Space(20)]

    [Header("Attacks")]
    [SerializeField]
    private float _attackDist;

    #endregion

    protected override void Start()
    {
        base.Start();
        GravityScale = 0;
        SetGravityScale(GravityScale);
    }

    protected override void Update()
    {
        base.Update();
        CheckAttack();

        if (IsWillingToAttack)
            Attack();
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
        Vector2 targetSpeed = DirectionToDestination * MaxSpeed;
        targetSpeed = Vector2.Lerp(Rigidbody.velocity, targetSpeed, lerpAmount);

        #region Calculate AccelRate
        float accelRate;
        accelRate = (targetSpeed.magnitude > 0.01f) ? AccelAmount : DeccelAmount;
        #endregion

        Vector2 speedDif = targetSpeed - Rigidbody.velocity;
        Vector2 movement = speedDif * accelRate;

        Rigidbody.AddForce(movement, ForceMode2D.Force);
    }
    #endregion
}