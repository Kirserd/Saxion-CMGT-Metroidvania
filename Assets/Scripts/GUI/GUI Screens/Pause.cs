using UnityEngine;

public class Pause : MonoBehaviour
{
    private Animator _animator;
    private bool _pauseOpened;

    private void Start() => _animator = GetComponent<Animator>();


    private void Update()
    {
        if (Input.GetKeyDown(Controls.Get(InputAction.Return)) ||
            Input.GetKeyDown(Controls.GetAlt(InputAction.Return)))
            SetState(!_pauseOpened);
    }

    private void SetState(bool state)
    {
        if(state)
            TimeManager.instance.Pause();
        else
            TimeManager.instance.SetNormalTimeScale();

        _pauseOpened = state;
        _animator.SetTrigger(state ? "Open" : "Close");
        GameStateMachine.SetState(state ? GameState.Pause : GameStateMachine.Previous);
    }
}
