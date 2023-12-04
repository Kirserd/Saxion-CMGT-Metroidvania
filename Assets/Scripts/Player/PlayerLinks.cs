using UnityEngine;

[RequireComponent(typeof(PlayerAnimator))]
[RequireComponent(typeof(PlayerInteractor))]
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerCombat))]
public class PlayerLinks : MonoBehaviour
{
    public static PlayerLinks instance { get; private set; }

    public PlayerAnimator PlayerAnimator { get; private set; }
    public PlayerMovement PlayerMovement { get; private set; }
    public PlayerInteractor PlayerInteractor { get; private set; }
    public PlayerCombat PlayerCombat { get; private set; }

    private void Awake()
    {
        instance = this;
        PlayerAnimator = GetComponent<PlayerAnimator>();
        PlayerMovement = GetComponent<PlayerMovement>();
        PlayerInteractor = GetComponent<PlayerInteractor>();
        PlayerCombat = GetComponent<PlayerCombat>();

        GameStateMachine.OnStateChanged += GameStateValidation;
    }

    private void GameStateValidation(GameState state) => SetPlayerInput(state == GameState.Overworld || state == GameState.Map);

    public void SetPlayerInput(bool state) 
    {
        PlayerMovement.Rigidbody.velocity *= Vector2.up;

        PlayerMovement.enabled = state;
        PlayerCombat.enabled = state;
        PlayerInteractor.enabled = state;
    }

    private void OnDestroy() => GameStateMachine.OnStateChanged -= GameStateValidation;
}
