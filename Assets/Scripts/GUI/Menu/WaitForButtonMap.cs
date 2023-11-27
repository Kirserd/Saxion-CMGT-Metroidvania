using UnityEngine;

public class WaitForButtonMap : MonoBehaviour
{
    private ControlsActions _caller;

    private int _action = -1;
    private bool _isAlt = false;
    public void SetMappingInfo(ControlsActions caller, int action, bool isAlt)
    {
        _caller = caller;
        _action = action; 
        _isAlt = isAlt; 
    }

    private void OnGUI()
    {
        if (_action == -1)
            return;

        Event e = Event.current;
        if (e.isKey)
        {
            if (_isAlt)
                Controls.SetAlt((InputAction)_action, e.keyCode);
            else
                Controls.Set((InputAction)_action, e.keyCode);

            _caller.OnControlChanged.Invoke((InputAction)_action);
            Destroy(gameObject);
        }
    }
}