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
        AIR_JUMP
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

    private void Start() => _animator = GetComponent<Animator>();

    public void Interact(PlayerInteractor caller)
    {
        PlayerMovement playerMovement = PlayerLinks.instance.PlayerMovement;
        switch (_type)
        {
            case (PowerUpType.DASH):
                playerMovement.DashUnlocked = true;
                break;
            case (PowerUpType.WALL_JUMP):
                playerMovement.WallJumpUnlocked = true;
                break;
            case (PowerUpType.GLIDE):
                playerMovement.GlideUnlocked = true;
                break;
            case (PowerUpType.AIR_JUMP):
                playerMovement.AirJumpUnlocked = true;
                break;
            default:
                break;
        }
        Destroy();
    }
    protected virtual void Destroy() => StartCoroutine(nameof(AnimateDestroy));

    private IEnumerator AnimateDestroy()
    {
        _animator.SetTrigger(_trigger);
        yield return new WaitForSeconds(_length);
        Destroy(gameObject);
    }
}
