using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueVisualizator : MonoBehaviour
{
    [SerializeField]
    private Dialogue _motivation, _demotivation;

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
        if (_isBusy)
            return;

        StartCoroutine(nameof(DialogueCycle), dialogue);
    }

    private IEnumerator DialogueCycle(Dialogue dialogue)
    {
        _isBusy = true;
        _animator.SetTrigger("Open");

        if (dialogue.OneTime)
            Dialogues.FinishedOneTimeDialogue(dialogue);

        float charDelay = 0.1f / Dialogues.DialogueSpeed;

        int charMax;
        int charCounter;

        int lineMax = dialogue.Lines.Count;
        int lineCounter = 0;

        string currentLine;

        bool lineWithOptions;

        string[] currentOptions;
        Dialogue[] currentResults;

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
                if(dialogue.ThirdOption[lineCounter] != "")
                    currentOptions = new string[3]
                   {
                        dialogue.FirstOption[lineCounter],
                        dialogue.SecondOption[lineCounter],
                        dialogue.ThirdOption[lineCounter]
                   };
                else if(dialogue.SecondOption[lineCounter] != "")
                    currentOptions = new string[2]
                    {
                        dialogue.FirstOption[lineCounter],
                        dialogue.SecondOption[lineCounter]
                    };
                else 
                    currentOptions = new string[1]
                    {
                        dialogue.FirstOption[lineCounter]
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
                currentResults = new Dialogue[3]
                {
                    dialogue.FirstResult[lineCounter],
                    dialogue.SecondResult[lineCounter],
                    dialogue.ThirdResult[lineCounter]
                };

                if (currentResults[_chosenOption] is not null)
                {
                    dialogue = currentResults[_chosenOption];
                    lineMax = dialogue.Lines.Count;
                    lineCounter = -1;
                    _chosenOption = -1;
                }
            }

            if (lineCounter + 1 >= lineMax)
            {
                if (dialogue.result != DialogueResult.None)
                {
                    if (dialogue.result == DialogueResult.Motivation)
                    {
                        dialogue = _motivation;
                        GameProgress.Motivations++;
                    }
                    else dialogue = _demotivation;

                    lineMax = dialogue.Lines.Count;
                    lineCounter = -1;
                }
            }

            lineCounter++;
        }

        _animator.SetTrigger("Close");
<<<<<<< Updated upstream
=======
        GameStateMachine.SetState(GameState.Overworld);
        
>>>>>>> Stashed changes
        _isBusy = false;
    }

    private void InstantiateOptions(ref string[] options)
    {
        _animator.SetTrigger("MoveLeft");

        OptionsTemplate optionsTemplate = Instantiate(_optionsPrefab, transform);
        optionsTemplate.SetupOptions(ref options);
        optionsTemplate.SubscribeButtons(this);
        optionsTemplate.transform.SetParent(transform.parent);
    }

    public void OnOptionClick(int index)
    {
        _chosenOption = index;
        _wantToProceed = true;

        _animator.SetTrigger("MoveRight");
    }
}