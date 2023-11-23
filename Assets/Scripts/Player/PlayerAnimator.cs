using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerAnimator : MonoBehaviour
{
    private PlayerMovement _movement;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;

    [Header("Particle FX")]
    [SerializeField] private GameObject _jumpFX;
    [SerializeField] private GameObject _landFX;

    public bool _startedJumping {  private get; set; }
    public bool _justLanded { private get; set; }

    public float _currentVelocityY;

    private void Start()
    {
        _movement = GetComponent<PlayerMovement>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _animator = _spriteRenderer.GetComponent<Animator>();
    }

    private void LateUpdate()
    {
      //  CheckAnimationState(); TODO
    }

    private void CheckAnimationState()
    {
        if (_startedJumping)
        {
            _animator.SetTrigger("Jump");

            //GameObject obj = Instantiate(jumpFX, transform.position - (Vector3.up * transform.localScale.y / 2), Quaternion.Euler(-90, 0, 0)); TODO
            //Destroy(obj, 1);

            _startedJumping = false;
            return;
        }

        if (_justLanded)
        {
            _animator.SetTrigger("Land");

            //GameObject obj = Instantiate(landFX, transform.position - (Vector3.up * transform.localScale.y / 1.5f), Quaternion.Euler(-90, 0, 0)); TODO
            //Destroy(obj, 1);

            _justLanded = false;
            return;
        }

        _animator.SetFloat("Vel Y", _movement.Rigidbody.velocity.y);
        _animator.SetFloat("Vel X", _movement.Rigidbody.velocity.x);
    }
}
