using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine;

public class PlayerController : MonoBehaviourPun
{
    private float _speed = 5.0f;
    private float _horizontalMovement;
    private float _verticalMovement;
    private PhotonVoiceView _photonVoiceView;
    private Recorder _recorder;
    
    private void Awake()
    {
        _photonVoiceView = GetComponent<PhotonVoiceView>();
        _photonVoiceView.UsePrimaryRecorder = true;
    }

    // Start is called before the first frame update
    void Start()
    {
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
        }
        else if (Input.GetButtonUp("Voice"))
        {
            _recorder.TransmitEnabled = false;
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
