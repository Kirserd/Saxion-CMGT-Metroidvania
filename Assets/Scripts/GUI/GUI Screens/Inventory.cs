using UnityEngine;

public class Inventory : MonoBehaviour
{
    private Animator _animator;
    private bool _invOpened;

    private void Start() => _animator = GetComponent<Animator>();

    private void Update()
    {
        if (GameStateMachine.Current == GameState.Map || 
            GameStateMachine.Current == GameState.Dialogue || 
            GameStateMachine.Current == GameState.Pause )
        {
            if (_invOpened)
                SetState(false);

            return;
        }

        if (Input.GetKeyDown(Controls.Get(InputAction.Inventory)) ||
            Input.GetKeyDown(Controls.GetAlt(InputAction.Inventory)))
        {
            if (!_invOpened)
                SetState(true);
            else
                SetState(false);
        }
    }

    private void SetState(bool state)
    {
        _invOpened = state;

        _animator.SetTrigger(state ? "Open" : "Close");
        GameStateMachine.SetState(state ? GameState.Inventory : GameState.Overworld);
    }
}
