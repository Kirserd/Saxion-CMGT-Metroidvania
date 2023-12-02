using UnityEngine;

public class OnTriggerEnterDialogue : DialogueStarter
{
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject == PlayerLinks.instance.gameObject)
            PlayDialogue();
    }
}
