
using System;
using UnityEngine;

public class Button:InteractableObject
{
    private Animator _animator;
    private ButtonConsole _buttonConsole;
    
    
    private void Awake()
    {
        _animator = gameObject.GetComponent<Animator>();
        _buttonConsole = transform.parent.GetComponent<ButtonConsole>();
    }

    public override void Interact(GameObject player)
    {
        _animator.SetTrigger("Pressed");
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        _buttonConsole.updateResult(player.GetComponent<PlayerControllerMap>());
    }

    public void resetPressedStatus()
    {
        gameObject.layer = LayerMask.NameToLayer("Interactable");
    }
}
