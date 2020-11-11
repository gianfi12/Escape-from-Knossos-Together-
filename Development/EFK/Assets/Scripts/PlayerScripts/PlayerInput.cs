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
    }
    private void FixedUpdate()
    {
        _horizontal = Input.GetAxisRaw("Horizontal");
        _vertical = Input.GetAxisRaw("Vertical");
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
}
