
using System;
using System.Data.SqlTypes;
using UnityEngine;

public class Wardrobe : InteractableObject
{
    private Vector3 _previousPlayerPosition;
    private SpriteRenderer _playerRenderer;
    private Collider2D _playerCollider;
    private bool _hasBeenSetted;
    private PlayerInput _playerInput;
    private Transform _playerTransform;
    private float _timeOfActivation;

    [SerializeField] private float minTimeInWardrobe;
    
    public override void Interact(GameObject player)
    {
        if (!_hasBeenSetted)
        {
            _hasBeenSetted = true;
            _playerInput = player.GetComponent<PlayerInput>();
            _playerRenderer = player.GetComponent<SpriteRenderer>();
            _playerCollider = player.GetComponent<Collider2D>();
            _playerTransform = player.transform;

        }

        if(!_hasBeenActivated && _playerInput.CanMove)
        {
            _hasBeenActivated = true;
            _playerInput.CanMove = false;
            _previousPlayerPosition = player.transform.position;
            _playerTransform.position = transform.position;
            _playerRenderer.enabled = false;
            _playerCollider.enabled = false;
            _timeOfActivation = Time.time;
        }
        else if(_hasBeenActivated && (Time.time - _timeOfActivation) > minTimeInWardrobe) {
            _hasBeenActivated = false;
            _playerInput.CanMove = true;
            _playerTransform.position = _previousPlayerPosition;
            _playerRenderer.enabled = true;
            _playerCollider.enabled = true;
        }
    }
}