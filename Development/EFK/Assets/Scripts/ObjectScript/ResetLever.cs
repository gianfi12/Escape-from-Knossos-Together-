
using UnityEngine;

public class ResetLever: InteractableObject
{
    public EntrancePanel EntrancePanel;
    public override void Interact(GameObject player)
    {
        EntrancePanel.ResetButtons();
    }
}
