using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pillar : InteractableObject {
    private PillarsRoomManager controller;
    private Animator animator;

    private void Start() {
        animator = GetComponent<Animator>();
    }

    public override void Interact(GameObject player) {     
        controller.PillarActivated(transform.GetSiblingIndex());
    }

    public void SetPillarsRoomManager(PillarsRoomManager pillarsRoomManager) {
        controller = pillarsRoomManager;
    }

    public void ResetPillar() {
        gameObject.layer = LayerMask.NameToLayer("Interactable");
        animator.SetBool("Lit", false);
    }

    public void DisableInteraction() {
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
    }
    public void EnableInteraction() {
        gameObject.layer = LayerMask.NameToLayer("Interactable");
    }

    public void LightUp() {
        animator.SetBool("Lit", true);
    }
}
