using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviourPun
{
    [SerializeField] private Image _pushToTalk;
    [SerializeField] private GameObject _playerUI;
    
    private float _speed = 5.0f;
    private float _horizontalMovement;
    private float _verticalMovement;
    private PhotonVoiceView _photonVoiceView;
    private Recorder _recorder;
    
    private void Awake()
    {
        //Set the recorder
        _photonVoiceView = GetComponent<PhotonVoiceView>();
        _photonVoiceView.UsePrimaryRecorder = true;
        StartCoroutine(SetUpRecorder());
    }

    // Start is called before the first frame update
    void Start()
    {
        //Set active the UI for each player
        if (photonView.IsMine) _playerUI.SetActive(true);
    }

    private IEnumerator SetUpRecorder()
    {
        yield return null;
        _recorder = _photonVoiceView.RecorderInUse;
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine) TakeInput();
    }

    private void TakeInput()
    {
        if (Input.GetButtonDown("Voice"))
        {
            _recorder.TransmitEnabled = true;
            _pushToTalk.color = Color.white;
        }
        else if (Input.GetButtonUp("Voice"))
        {
            _recorder.TransmitEnabled = false;
            _pushToTalk.color = Color.black;
        }
        
        _horizontalMovement = Input.GetAxisRaw("Horizontal");
        _verticalMovement = Input.GetAxisRaw("Vertical");

        if (Mathf.Abs(_horizontalMovement) < float.Epsilon)
            _horizontalMovement = 0;
        if (Mathf.Abs(_verticalMovement) < float.Epsilon)
            _verticalMovement = 0;
        if (_horizontalMovement != 0)
            _verticalMovement = 0f;
    }
    
    private void FixedUpdate()
    {
        float newX = transform.position.x + _speed * _horizontalMovement * Time.fixedDeltaTime;
        float newY = transform.position.y + _speed * _verticalMovement * Time.fixedDeltaTime;
        

        transform.position = new Vector3(newX, newY, transform.position.z);
    }
    
}
