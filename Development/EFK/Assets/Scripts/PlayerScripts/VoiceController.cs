using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine;
using UnityEngine.UI;

public class VoiceController : MonoBehaviourPun
{
    [SerializeField] private Image _pushToTalk;
    
    private PhotonVoiceView _photonVoiceView;
    private Recorder _recorder;
    [SerializeField] private Color disactiveColor;
    
    private void Awake()
    {
        //Set the recorder
        _photonVoiceView = GetComponent<PhotonVoiceView>();
        _photonVoiceView.UsePrimaryRecorder = true;
        PhotonVoiceNetwork.Instance.AutoLeaveAndDisconnect = true;
        StartCoroutine(SetUpRecorder());
    }

    private IEnumerator SetUpRecorder()
    {
        yield return null;
        _recorder = _photonVoiceView.RecorderInUse;
    }

    internal void enableVoice() {
        _recorder.TransmitEnabled = true;
        _pushToTalk.color = Color.white;
    }

    internal void disableVoice() {
        _recorder.TransmitEnabled = false;
        _pushToTalk.color = disactiveColor;
    }
}
