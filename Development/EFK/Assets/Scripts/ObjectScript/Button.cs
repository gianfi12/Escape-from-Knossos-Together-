
using System;
using UnityEngine;

public class Button:InteractableObject
{
    private Animator _animator;

    private void Awake()
    {
        _animator = gameObject.GetComponent<Animator>();
    }

    public override void Interact(GameObject player)
    {
        _animator.SetTrigger("Pressed");
    }
}
