using UnityEngine;

public class MotivationTrigger : MonoBehaviour
{
    private static int _predicate = 3;
    private Animator _animator;

    private void Awake() => _animator = GetComponent<Animator>();
    private void Start() => GameProgress.OnMotivationAcquired += CheckMotivations;

    private void CheckMotivations()
    {
        if (GameProgress.Motivations >= _predicate)
        {
            _animator.SetTrigger("Motivated");
            GameProgress.OnMotivationAcquired -= CheckMotivations;
        }
    }
}

