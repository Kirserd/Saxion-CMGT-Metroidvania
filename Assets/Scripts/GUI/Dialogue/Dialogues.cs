using UnityEngine;
using System.Collections.Generic;

public static class Dialogues
{
    public static float DialogueSpeed = 5;

    private static HashSet<Dialogue> _playedOneTimeDialogues;

    public static void PlayDialogue(ref Dialogue dialogue)
    {
        if (dialogue.OneTime && _playedOneTimeDialogues.Contains(dialogue))
            return;

        GameObject dialogueBox = GameObject.FindGameObjectWithTag("DialogueBox");
        DialogueVisualizator visualizator = dialogueBox.GetComponent<DialogueVisualizator>();
        visualizator.PlayDialogue(ref dialogue);
    }

    public static void ApplyResult(string result)
    {
        //TODO
    }

    public static void FinishedOneTimeDialogue(Dialogue dialogue) => _playedOneTimeDialogues.Add(dialogue);
}
