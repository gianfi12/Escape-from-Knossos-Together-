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
    private Animator _animator;
    private Vector2 _movement;
    //indicates if the player can move, so if it is in the scene or it is disable, if false it is also not visible and so
    //it shouldn't been seen from the agent moving in the map
    public bool _canMove = true;
    private float _lastDir;
    private PlayerInteraction _playerInteraction;

    [SerializeField] private GameObject _playerUI;
    [SerializeField] private GameObject diaryPanel;
    [SerializeField] private GameObject diaryImage;
    private bool isDiaryActive;

    // Start is called before the first frame update
    void Start() {
        try {
            //Set active the UI for each player
            if (!photonView.IsMine) _playerUI.SetActive(false);
        }
        catch (NullReferenceException) {
            Debug.Log("Voice controls disabled");
        }
        
        if (!PhotonNetwork.IsConnected) _playerUI.SetActive(true);
    }


    private void Awake()
    {
        _playerController = GetComponent<PlayerControllerMap>();
        _voiceController = GetComponent<VoiceController>();
        _animator = GetComponent<Animator>();
        _playerInteraction = GetComponent<PlayerInteraction>();
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
                    //_lastDir = SetDirection();
                }
            }
        }
        else
        {
            if (Math.Abs(_movement.x) > Double.Epsilon || Math.Abs(_movement.y) > Double.Epsilon)
            {
                _playerController.Move(new Vector3(_movement.x, _movement.y, 0f));
                //_lastDir = SetDirection();
            }
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
        //_animator.SetFloat("Direction", _lastDir);

        try {
            if (photonView.IsMine) {
                if (Input.GetButtonDown("Voice")) {
                    _voiceController.enableVoice();
                }
                else if (Input.GetButtonUp("Voice")) {
                    _voiceController.disableVoice();
                }

                if (Input.GetButtonDown("Map") )
                {
                    if (!isDiaryActive)
                    {
                        diaryPanel.SetActive(true);
                        diaryImage.SetActive(false);
                        isDiaryActive = true; 
                    }
                    else
                    {
                        diaryPanel.SetActive(false);
                        diaryImage.SetActive(true);
                        isDiaryActive = false;
                    }
                
                }
            }
        }
        catch (NullReferenceException) {}

        if (!PhotonNetwork.IsConnected)
        {
            if (Input.GetButtonDown("Map"))
            {
                diaryPanel.SetActive(true);
                diaryImage.SetActive(false);
            }
    
            else if (Input.GetButtonUp("Map"))
            {
                diaryPanel.SetActive(false);
                diaryImage.SetActive(true);
            }
        }

        if (Input.GetButtonDown("Interact")) {
            _playerInteraction.InteractWithTarget(transform.gameObject);
        }
    }

    public bool CanMove
    {
        get => _canMove;
        set => _canMove = value;
    }

    private float SetDirection()
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
