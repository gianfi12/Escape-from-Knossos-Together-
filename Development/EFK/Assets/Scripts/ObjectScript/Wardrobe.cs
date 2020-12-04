
using System;
using System.Data.SqlTypes;
using UnityEngine;

public class Wardrobe : InteractableObject
{
    private SpriteRenderer _playerRenderer;
    private bool _hasBeenSetted;
    private PlayerInput _playerInput;
    private PlayerInteraction _playerInteraction;
    private float _timeOfActivation;

    [SerializeField] private float minTimeInWardrobe;
    
    public override void Interact(GameObject player)
    {
        if (!_hasBeenSetted)
        {
            _hasBeenSetted = true;
            _playerInput = player.GetComponent<PlayerInput>();
            _playerInteraction = player.GetComponent<PlayerInteraction>();
            _playerRenderer = player.GetComponent<SpriteRenderer>();

        }

        if(!_hasBeenActivated && _playerInput.CanMove)
        {
            _hasBeenActivated = true;
            _playerInput.CanMove = false;
            _playerInteraction.canChangeLastInteractableObejct = false;
            _playerRenderer.enabled = false;
            _timeOfActivation = Time.time;
        }
        else if(_hasBeenActivated && (Time.time - _timeOfActivation) > minTimeInWardrobe) {
            _hasBeenActivated = false;
            _playerInput.CanMove = true;
            _playerInteraction.canChangeLastInteractableObejct = true;
            _playerRenderer.enabled = true;
        }
    }
}