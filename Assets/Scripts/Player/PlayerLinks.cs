using UnityEngine;

[RequireComponent(typeof(PlayerAnimator))]
[RequireComponent(typeof(PlayerInteractor))]
[RequireComponent(typeof(PlayerMovement))]
public class PlayerLinks : MonoBehaviour
{
    public static PlayerLinks instance { get; private set; }

    public PlayerAnimator PlayerAnimator { get; private set; }
    public PlayerMovement PlayerMovement { get; private set; }
    public PlayerInteractor PlayerInteractor { get; private set; }

    private void Awake()
    {
        instance = this;
        PlayerAnimator = GetComponent<PlayerAnimator>();
        PlayerMovement = GetComponent<PlayerMovement>();
        PlayerInteractor = GetComponent<PlayerInteractor>();
    }
}
