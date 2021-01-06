
using UnityEngine;

public class ResetLever: InteractableObject
{
    public ColorButtonPanel EntrancePanel;
    public override void Interact(GameObject player)
    {
        GetComponent<Animator>().SetTrigger("Activate");
        FindObjectOfType<AudioManager>().Play("Lever");
    }

    public void ResetPanel()
    {
        EntrancePanel.ResetButtons();
    }
}
