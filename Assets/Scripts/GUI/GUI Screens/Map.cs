using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField]
    private Camera _markerRenderer;

    private Animator _animator;
    private bool _mapOpened; 

    private void Start() =>  _animator = GetComponent<Animator>();      

    private void Update()
    {
        if (GameStateMachine.Current == GameState.Dialogue || GameStateMachine.Current == GameState.Pause)
        {
            if (_mapOpened)
                SetState(false);

            return;
        }

        if (!_mapOpened)
        {
            if (Input.GetKeyDown(Controls.Get(InputAction.Map)) ||
                Input.GetKeyDown(Controls.GetAlt(InputAction.Map)))
                SetState(true);
        }
        else if (Input.GetKeyUp(Controls.Get(InputAction.Map)) ||
                 Input.GetKeyUp(Controls.GetAlt(InputAction.Map)))
            SetState(false);
    }

    private void SetState(bool state)
    {
        _mapOpened = state;
        _markerRenderer.enabled = state;

        _animator.SetTrigger(state? "Open" : "Close");
        GameStateMachine.SetState(state ? GameState.Map : GameState.Overworld);
    }
}
