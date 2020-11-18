using System;
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
    private float _horizontal;
    private float _vertical;
    //indicates if the player can move, so if it is in the scene or it is disable, if false it is also not visible and so
    //it shouldn't been seen from the agent moving in the map
    private bool _canMove=true;
    private PlayerInteraction _playerInteraction;

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
        _playerInteraction = GetComponent<PlayerInteraction>();
    }
    private void FixedUpdate()
    {
        if(_canMove)
        {
            _horizontal = Input.GetAxisRaw("Horizontal");
            _vertical = Input.GetAxisRaw("Vertical");
        }
        else
        {
            _horizontal = 0f;
            _vertical = 0f;
        }
        if (PhotonNetwork.IsConnected)
        {
            if (photonView.IsMine)
            {
                if(Math.Abs(_horizontal)>Double.Epsilon || Math.Abs(_vertical)>Double.Epsilon) _playerController.Move(new Vector3(_horizontal,_vertical,0f));
            }
        }
        else
        {
            if(Math.Abs(_horizontal)>Double.Epsilon || Math.Abs(_vertical)>Double.Epsilon) _playerController.Move(new Vector3(_horizontal,_vertical,0f));
        }


        if (Input.GetButtonDown("Interact") && _playerInteraction.HasPreviousValue)
        {
            InteractableObject interactableObject = _playerInteraction.PreviousInteraction.GetComponent<InteractableObject>();
            interactableObject.Interact(transform.gameObject);
        }
    }

    private void Update() {
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
}
