using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueVisualizator : MonoBehaviour
{
    [SerializeField]
    private OptionsTemplate _optionsPrefab;

    [SerializeField]
    private TextMeshProUGUI _textMeshPro;

    [SerializeField]
    private Animator _animator;

    private int _chosenOption = -1;

    private bool _wantToProceed;
    private bool _isBusy;


    private void Start() => _animator = GetComponent<Animator>();

    public void PlayDialogue(ref Dialogue dialogue)
    {
        if (_isBusy || GameStateMachine.Current != GameState.Overworld)
            return;

        GameStateMachine.SetState(GameState.Dialogue);
        StartCoroutine(nameof(DialogueCycle), dialogue);
    }

    private IEnumerator DialogueCycle(Dialogue dialogue)
    {
        _isBusy = true;
        _animator.SetTrigger("Open");

        float charDelay = 0.1f / Dialogues.DialogueSpeed;

        int charMax;
        int charCounter;

        int lineMax = dialogue.Lines.Count;
        int lineCounter = 0;

        string currentLine;

        bool lineWithOptions;

        string[] currentOptions;
        string[] currentResults;

        while (lineCounter < lineMax)
        {
            _textMeshPro.text = "";

            _wantToProceed = false;
            lineWithOptions = dialogue.FirstOption[lineCounter] != "";

            currentLine = dialogue.Lines[lineCounter];
            charMax = currentLine.Length;
            charCounter = 0;

            while(charCounter < charMax)
            {
                _textMeshPro.text += currentLine[charCounter];
                charCounter++;

                yield return new WaitForSeconds(charDelay);
            }

            if (lineWithOptions)
            {
                currentOptions = new string[3]
                {
                    dialogue.FirstOption[lineCounter],
                    dialogue.SecondOption[lineCounter],
                    dialogue.ThirdOption[lineCounter]
                };

                InstantiateOptions(ref currentOptions);
            }

            while (!_wantToProceed)
            {
                if (!lineWithOptions && 
                    (Input.GetKeyDown(Controls.Get(InputAction.Confirm)) || 
                     Input.GetKeyDown(Controls.GetAlt(InputAction.Confirm)) || 
                     Input.GetMouseButtonUp(0)))
                    _wantToProceed = true;
     
                yield return null;
            }

            if (lineWithOptions) 
            {
                currentResults = new string[3]
                {
                    dialogue.FirstResult[lineCounter],
                    dialogue.SecondResult[lineCounter],
                    dialogue.ThirdResult[lineCounter]
                };

                Dialogues.ApplyResult(currentResults[_chosenOption]);
                _chosenOption = -1;
            }

            lineCounter++;
        }

        if (dialogue.OneTime)
            Dialogues.FinishedOneTimeDialogue(dialogue);

        _animator.SetTrigger("Close");
        _isBusy = false;

        GameStateMachine.SetState(GameState.Overworld);
    }

    private void InstantiateOptions(ref string[] options)
    {
        _animator.SetTrigger("MoveLeft");

        OptionsTemplate optionsTemplate = Instantiate(_optionsPrefab, transform);
        optionsTemplate.SetupOptions(ref options);
        optionsTemplate.SubscribeButtons((int index) => OnOptionClick(index));
        optionsTemplate.transform.SetParent(transform.parent);
    }

    public void OnOptionClick(int index)
    {
        _chosenOption = index - 1;
        _wantToProceed = true;

        _animator.SetTrigger("MoveRight");
    }
}