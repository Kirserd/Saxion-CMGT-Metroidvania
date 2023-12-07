using UnityEngine;

public class BossDoor : MonoBehaviour
{
    private static int _predicate = 5;
    private Animator _animator;

    private void Awake() => _animator = GetComponent<Animator>();
    private void Start() => GameProgress.OnImportantDialogueAcquired += CheckPredicate;

    private void CheckPredicate()
    {
        if (GameProgress.ImportantDialogues >= _predicate)
        {
            _animator.SetTrigger("Open");
            GameProgress.OnImportantDialogueAcquired -= CheckPredicate;
        }
    }
}