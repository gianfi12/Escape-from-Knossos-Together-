
using UnityEngine;

public class ResetLever: InteractableObject
{
    public ColorButtonPanel EntrancePanel;
    public override void Interact(GameObject player)
    {
        EntrancePanel.ResetButtons();
    }
}
