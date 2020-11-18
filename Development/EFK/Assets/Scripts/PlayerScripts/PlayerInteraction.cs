﻿
using System;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float interactionDistance;
    [SerializeField] private LayerMask interactionLayer;
    [SerializeField] private GameObject interactiveText;
    private Transform _previousInteraction;
    private bool _hasPreviousValue = false;
    private GameObject _instatiatedText;
    private Collider2D _playerCollider;

    private void Awake()
    {
        _playerCollider = transform.GetComponent<Collider2D>();
    }

    private void Update()
    {
        Vector3 direction3D = GetComponent<PlayerControllerMap>().Movement;
        Vector2 direction = new Vector2(direction3D.x, direction3D.y);
        Bounds bounds = transform.GetComponent<Collider2D>().bounds;
        RaycastHit2D hit = Physics2D.CircleCast(bounds.center, bounds.extents.y/2, direction, interactionDistance, interactionLayer);
        if (hit)
        {
            Transform trans = hit.transform; 
            if (trans.gameObject.layer==8)
            {
                if (!_hasPreviousValue || trans.GetInstanceID() != _previousInteraction.transform.GetInstanceID())
                {
                    Material shader = hit.transform.GetComponent<SpriteRenderer>().material;
                    shader.SetFloat("_Thickness",5f);
                    //hit.transform.GetComponent<InteractableObject>().Interact(transform.gameObject);
                    Destroy(_instatiatedText);
                    _instatiatedText = Instantiate(interactiveText);
                    _instatiatedText.transform.parent = trans;
                    _instatiatedText.transform.position = trans.position + new Vector3(13.5f,-2.7f,0f);
                    _previousInteraction = hit.transform;
                    _hasPreviousValue = true;
                }
            }
        }else if (_instatiatedText)
        {
            Material shader = _previousInteraction.GetComponent<SpriteRenderer>().material;
            shader.SetFloat("_Thickness",0f);
            Destroy(_instatiatedText);
            _hasPreviousValue = false;
        }
    }

    public Transform PreviousInteraction => _previousInteraction;

    public bool HasPreviousValue => _hasPreviousValue;
}
