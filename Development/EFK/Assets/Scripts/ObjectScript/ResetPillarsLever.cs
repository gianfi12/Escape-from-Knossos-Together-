using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPillarsLever : InteractableObject
{
    public PillarsRoomManager pillarsRoom;
    public override void Interact(GameObject player) {
        GetComponent<Animator>().SetTrigger("Activate");
        FindObjectOfType<AudioManager>().Play("Lever");
        pillarsRoom.ResetActivatedPillars();
    }

    public void ResetPanel() {
        // lasciato perchè sto usando l'animator di reset lever che chiama questo con evento
    }

}
