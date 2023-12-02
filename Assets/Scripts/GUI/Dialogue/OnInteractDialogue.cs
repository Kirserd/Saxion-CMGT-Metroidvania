public class OnInteractDialogue : DialogueStarter, Interactable
{
    public void Interact(PlayerInteractor caller)
    {
        PlayDialogue();
        if (dialogue.OneTime)
            Destroy(gameObject);
    }
}