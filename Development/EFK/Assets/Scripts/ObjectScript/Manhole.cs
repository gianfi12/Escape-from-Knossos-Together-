﻿
using System;
using System.Data.SqlTypes;
using UnityEngine;

public class Manhole : InteractableObject
{
    private SpriteRenderer _playerRenderer;
    private CapsuleCollider2D _playerCollider;
    private BoxCollider2D _playerFeetCollider;
    private bool _hasBeenSetted;
    private PlayerInput _playerInput;
    private PlayerInteraction _playerInteraction;
    private float _timeOfActivation;
    private Animator _animator;

    [SerializeField] private float minTimeInWardrobe;

    private void Awake()
    {
        _animator = gameObject.GetComponent<Animator>();
    }

    public override void Interact(GameObject player)
    {
        if (!_hasBeenSetted)
        {
            _hasBeenSetted = true;
            _playerInput = player.GetComponent<PlayerInput>();
            _playerInteraction = player.GetComponent<PlayerInteraction>();
            _playerRenderer = player.GetComponent<SpriteRenderer>();
            _playerCollider = player.GetComponent<CapsuleCollider2D>();
            _playerFeetCollider = player.GetComponentInChildren<BoxCollider2D>();
        }

        if(!_hasBeenActivated && _playerInput.CanMove)
        {
            _hasBeenActivated = true;
            _animator.SetBool("isEmpty",false);
            _playerInput.CanMove = false;
            _playerInteraction.canChangeLastInteractableObejct = false;
            _playerRenderer.enabled = false;
            _playerCollider.enabled = false;
            _playerFeetCollider.enabled = false;
            _timeOfActivation = Time.time;
        }
        else if(_hasBeenActivated && (Time.time - _timeOfActivation) > minTimeInWardrobe) {
            _hasBeenActivated = false;
            _animator.SetBool("isEmpty",true);
            _playerInput.CanMove = true;
            _playerInteraction.canChangeLastInteractableObejct = true;
            _playerRenderer.enabled = true;
            _playerCollider.enabled = true;
            _playerFeetCollider.enabled = true;
        }
    }
}