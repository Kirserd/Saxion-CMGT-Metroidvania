using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class OptionsTemplate : MonoBehaviour
{
    [SerializeField]
    private Button _buttonPrefab;

    [SerializeField]
    private int _distanceBetween, _buttonHeight;

    private Button[] _options;
    private Animator[] _animators;
    private int _selection = 0;

    private void Update()
    {
        if (_options is null)
            return;

        if (_selection < _options.Length - 1 &&
          (Input.GetKeyDown(Controls.Get(InputAction.Down)) ||
           Input.GetKeyDown(Controls.GetAlt(InputAction.Down))))
            _selection++;

        if (_selection > 0 &&
           (Input.GetKeyDown(Controls.Get(InputAction.Up)) ||
            Input.GetKeyDown(Controls.GetAlt(InputAction.Up))))
            _selection--;

        if (Input.GetKeyUp(Controls.Get(InputAction.Confirm)) ||
            Input.GetKeyUp(Controls.GetAlt(InputAction.Confirm)))
            _options[_selection].onClick.Invoke();

        if (_animators is null)
            return;

        UpdateSelection();
    }

    public void SetupOptions(ref string[] options)
    {
        _options = new Button[options.Length];
        _animators = new Animator[options.Length];

        float startY = (_buttonHeight + _distanceBetween) * 0.5f * (options.Length - 1);

        for (int i = 0; i < options.Length; i++)
        {
            _options[i] = Instantiate(_buttonPrefab, transform);
            _animators[i] = _options[i].GetComponent<Animator>();
            _options[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = options[i];

            RectTransform rectTransform = (_options[i].transform as RectTransform);

            float yPos = startY - (_buttonHeight + _distanceBetween) * i;
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, yPos);
        }
    }

    public void SubscribeButtons(DialogueVisualizator caller)
    {
        _options[0].onClick.AddListener(() => caller.OnOptionClick(0));
        _options[0].onClick.AddListener(() => Destroy(gameObject));
        if (_options.Length == 1)
            return;

        _options[1].onClick.AddListener(() => caller.OnOptionClick(1));
        _options[1].onClick.AddListener(() => Destroy(gameObject));
        if (_options.Length == 2)
            return;

        _options[2].onClick.AddListener(() => caller.OnOptionClick(2));
        _options[2].onClick.AddListener(() => Destroy(gameObject));
    }

    private void UpdateSelection()
    {
        for (int i = 0; i < _options.Length; i++)
        {
            if(i == _selection)
                _animators[i].SetTrigger("Select");
            
            else
                _animators[i].SetTrigger("Deselect");
        }
    } 
}
