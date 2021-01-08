using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pillar : InteractableObject {
    private PillarsRoomManager controller;
    private Animator animator;
    [SerializeField] private AudioSource[] pillarsSounds;
    [SerializeField] private AudioSource firstPillarSound;
    [SerializeField] private AudioSource firstPillarOn;
    [SerializeField] private AudioSource secondPillarOn;
    [SerializeField] private AudioSource lastPillarRight;
    [SerializeField] private AudioSource lastPillarWrong;
    

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

    public void PlayPillarSound()
    {
        System.Random random = new System.Random();
        pillarsSounds[random.Next(0,pillarsSounds.Length)].Play();
    }

    public void PlayFirstPillar()
    {
        firstPillarSound.Play();
    }
    
    public void PlayFirstPillarOn()
    {
        firstPillarOn.Play();
        firstPillarSound.Stop();
    }
    
    public void PlaySecondPillarOn()
    {
        secondPillarOn.Play();
    }
    
    public void PlayLastPillar(bool isRight)
    {
        if (isRight) lastPillarRight.Play();
        else lastPillarWrong.Play();
    }
}
