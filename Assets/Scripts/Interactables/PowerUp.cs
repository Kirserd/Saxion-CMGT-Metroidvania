using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class PowerUp : MonoBehaviour, Interactable
{
    private Animator _animator;

    enum PowerUpType
    {
        DASH,
        WALL_JUMP,
        GLIDE,
        AIR_JUMP,
        DAMAGE_UP,
        HEALTH_UP,
        WEAPON
    };

    [Space(5)]
    
    [Header("OnPickUp Animation Params")]
    [SerializeField]
    private string _trigger = "Dissolve";
    [SerializeField]
    private float _length = 0.4f;
    
    [Space(10)]
    
    [Header("PowerUp Params")]
    [SerializeField]
    private PowerUpType _type;

    private bool _isDead = false;

    private void Start() => _animator = GetComponent<Animator>();

    public void Interact(PlayerInteractor caller)
    {
        if (_isDead)
            return;

        switch (_type)
        {
            case (PowerUpType.DASH):
                GameProgress.HasDash = true;
                break;
            case (PowerUpType.WALL_JUMP):
                GameProgress.HasWallJump = true;
                break;
            case (PowerUpType.GLIDE):
                GameProgress.HasGlide = true;
                break;
            case (PowerUpType.AIR_JUMP):
                GameProgress.HasAirJump = true;
                break;
            case (PowerUpType.DAMAGE_UP):
                GameProgress.HasDamageUp = true;
                break;
            case (PowerUpType.HEALTH_UP):
                GameProgress.HasHPUp = true;
                PlayerLinks.instance.PlayerCombat.SetMaxHP
                (PlayerLinks.instance.PlayerCombat.MaxHP + 1);
                break;
            case (PowerUpType.WEAPON):
                GameProgress.HasWeapon = true;
                break;
            default:
                break;
        }
        _isDead = true;
        Destroy();
    }
    protected virtual void Destroy() => StartCoroutine(nameof(AnimateDestroy));

    private IEnumerator AnimateDestroy()
    {
        _animator.SetTrigger(_trigger);
        if (_length == -1)
            yield break;

        yield return new WaitForSeconds(_length);
        Destroy(gameObject);
    }
}
