using UnityEngine;
using TMPro;

public class ControlVisualizer : MonoBehaviour
{
    [SerializeField]
    private InputAction _action;

    [SerializeField]
    private bool _isAlternativeKey;

    [SerializeField]
    private TextMeshProUGUI _display;

    private void Start()
    {
        Refresh();
        ControlsActions.instance.OnControlChanged += TryRefresh;
    }

    private void TryRefresh(InputAction action)
    {
        if (action != _action)
            return;

        Refresh();
    }

    public void Refresh() => _display.text = (_isAlternativeKey ? Controls.GetAlt(_action) : Controls.Get(_action)).ToString();
}