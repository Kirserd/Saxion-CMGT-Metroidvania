using UnityEngine;

public class DialogueStarter : MonoBehaviour
{
    [SerializeField]
    protected Dialogue dialogue;

    protected void PlayDialogue() => Dialogues.PlayDialogue(ref dialogue);
}
