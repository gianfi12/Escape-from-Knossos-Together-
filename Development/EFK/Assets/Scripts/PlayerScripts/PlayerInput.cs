﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PlayerControllerMap))]

public class PlayerInput : MonoBehaviourPun
{
    private PlayerControllerMap _playerController;
    private VoiceController _voiceController;
    private Animator _animator;
    private Vector2 _movement;
    //indicates if the player can move, so if it is in the scene or it is disable, if false it is also not visible and so
    //it shouldn't been seen from the agent moving in the map
    private bool _canMove=true;
    private float _lastDir;

    [SerializeField] private GameObject _playerUI;

    // Start is called before the first frame update
    void Start() {
        try {
            //Set active the UI for each player
            if (photonView.IsMine) _playerUI.SetActive(true);
        }
        catch (NullReferenceException) {
            Debug.Log("Voice controls disabled");
        }
    }


    private void Awake()
    {
        _playerController = GetComponent<PlayerControllerMap>();
        _voiceController = GetComponent<VoiceController>();
        _animator = GetComponent<Animator>();
    }
    private void FixedUpdate()
    {
        if (PhotonNetwork.IsConnected)
        {
            if (photonView.IsMine)
            {
                if (Math.Abs(_movement.x) > Double.Epsilon || Math.Abs(_movement.y) > Double.Epsilon)
                {
                    _playerController.Move(new Vector3(_movement.x, _movement.y, 0f));
                    _lastDir = SetDirection();
                }
            }
        }
        else
        {
            if (Math.Abs(_movement.x) > Double.Epsilon || Math.Abs(_movement.y) > Double.Epsilon)
            {
                _playerController.Move(new Vector3(_movement.x, _movement.y, 0f));
                _lastDir = SetDirection();
            }
        }

        PlayerInteraction playerInteraction = transform.gameObject.GetComponent<PlayerInteraction>();
        if (Input.GetButtonDown(KeyCode.E.ToString()) && playerInteraction.HasPreviousValue){}
        {
            InteractableObject interactableObject = playerInteraction.PreviousInteraction.GetComponent<InteractableObject>();
            interactableObject.Interact(transform.gameObject);
        }
    }

    private void Update() 
    {
        if(_canMove)
        {
            _movement.x = Input.GetAxisRaw("Horizontal");
            _movement.y = Input.GetAxisRaw("Vertical");
        }
        else
        {
            _movement.x = 0f;
            _movement.y = 0f;
        }
        
        _animator.SetFloat("Speed", _movement.SqrMagnitude());
        _animator.SetFloat("Horizontal", _movement.x);
        _animator.SetFloat("Vertical", _movement.y);
        _animator.SetFloat("Direction", _lastDir);
        
        try {
            if (photonView.IsMine) {
                if (Input.GetButtonDown("Voice")) {
                    _voiceController.enableVoice();
                }
                else if (Input.GetButtonUp("Voice")) {
                    _voiceController.disableVoice();
                }
            }
        }
        catch (NullReferenceException) {}
    }

    public bool CanMove
    {
        get => _canMove;
        set => _canMove = value;
    }

    private int SetDirection()
    {
        if (Mathf.Abs(_movement.x) > Double.Epsilon && Math.Abs(_movement.x) > Mathf.Abs(_movement.y))
        {
            if (_movement.x > Double.Epsilon) return 3; //right
            return 4; //left
        }
        else
        {
            if (_movement.y > Double.Epsilon) return 2; //up
            return 1; //down
        }
    }
}
