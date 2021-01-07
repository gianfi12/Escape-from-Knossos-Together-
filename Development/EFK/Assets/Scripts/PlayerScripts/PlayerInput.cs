using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Photon.Pun;
using UnityEngine;
using Random = System.Random;

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
    public bool _isHidden;
    private float _lastDir;
    private PlayerInteraction _playerInteraction;

    [SerializeField] private GameObject _playerUI;
    [SerializeField] private GameObject diaryPanel;
    [SerializeField] private GameObject diaryImage;
    [SerializeField] private GameObject exitGamePrefab;
    private GameObject _exitGamePrefabInstance;
    private bool isReading;
    private float normalSpeed;
    private float readingSpeed = 2f;
    private AudioSource[] mapSounds;
    private AudioSource radioLoop;
    private AudioSource[] radioBursts;
    private AudioSource[] radioOnOff;
    System.Random random = new System.Random();

    public bool CanMove {
        get => _canMove;
        set => _canMove = value;
    }

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
        normalSpeed = GetComponent<PlayerControllerMap>().Speed;
    }


    private void Awake()
    {
        _playerController = GetComponent<PlayerControllerMap>();
        _voiceController = GetComponent<VoiceController>();
        _animator = GetComponent<Animator>();
        _playerInteraction = GetComponent<PlayerInteraction>();
        
        mapSounds = transform.Find("MapSounds").GetComponentsInChildren<AudioSource>();
        radioLoop = transform.Find("RadioSounds").GetComponent<AudioSource>();
        radioOnOff = radioLoop.transform.Find("OnOffSounds").GetComponents<AudioSource>();
        radioBursts = radioLoop.transform.Find("Bursts").GetComponents<AudioSource>();
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
        

        try {
            if (photonView.IsMine) {
                if (Input.GetButtonDown("Voice"))
                {
                    radioOnOff[random.Next(0, radioOnOff.Length)].Play();
                    radioLoop.PlayDelayed(0.1f);
                    StartCoroutine(StartBurst());
                    _voiceController.enableVoice();
                }
                else if (Input.GetButtonUp("Voice")) {
                    if (radioLoop.isPlaying) radioLoop.Stop();
                    radioOnOff[random.Next(0, radioOnOff.Length)].Play();
                    StopCoroutine(StartBurst());
                    _voiceController.disableVoice();
                }

                if (Input.GetButtonDown("Map"))
                {
                    diaryPanel.SetActive(true);
                    diaryImage.SetActive(false);
                    isReading = true;
                    _playerController.Speed = readingSpeed;
                    mapSounds[random.Next(0,18)].Play();
                }
    
                else if (Input.GetButtonUp("Map"))
                {
                    diaryPanel.SetActive(false);
                    diaryImage.SetActive(true);
                    isReading = false;
                    _playerController.Speed = normalSpeed;
                }
                if (Input.GetButtonDown("Interact")) {
                    _playerInteraction.InteractWithTarget(transform.gameObject);
                }
                if (Input.GetButtonDown("Cancel"))
                {
                    if (_exitGamePrefabInstance == null)
                    {
                        _exitGamePrefabInstance = Instantiate(exitGamePrefab);
                    }
                    else
                    {
                        Destroy(_exitGamePrefabInstance);
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
                isReading = true;
                _playerController.Speed = readingSpeed;
                mapSounds[random.Next(0,mapSounds.Length)].Play();
            }
    
            else if (Input.GetButtonUp("Map"))
            {
                diaryPanel.SetActive(false);
                diaryImage.SetActive(true);
                isReading = false;
                _playerController.Speed = normalSpeed;
            }
            if (Input.GetButtonDown("Interact")) {
                _playerInteraction.InteractWithTarget(transform.gameObject);
            }
            if (Input.GetButtonDown("Cancel"))
            {
                if (_exitGamePrefabInstance == null)
                {
                    _exitGamePrefabInstance = Instantiate(exitGamePrefab);
                }
                else
                {
                    Destroy(_exitGamePrefabInstance);
                }
            }
        }

        _animator.SetFloat("Speed", _movement.SqrMagnitude());
        _animator.SetFloat("Horizontal", _movement.x);
        _animator.SetBool("IsReading",isReading);
        //_animator.SetFloat("Vertical", _movement.y);
        //_animator.SetFloat("Direction", _lastDir);
    }

    IEnumerator StartBurst()
    {
        while (true)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.7f, 1.3f));
            radioBursts[random.Next(0,radioBursts.Length)].Play();
        }
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
