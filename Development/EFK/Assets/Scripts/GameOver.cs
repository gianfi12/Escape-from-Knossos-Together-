using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.PUN;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class GameOver : MonoBehaviourPunCallbacks
{
    [SerializeField] private Text lostText;

    public void ReloadMainMenu()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            SceneManager.LoadScene("FeedbackEnd");
        }
    }

    public void PlayAgain()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GameManager gameManager = FindObjectOfType<GameManager>();
            gameManager.RestartGame();
        }
        else
        {
            lostText.text = "Waiting for the Host...";
        }
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        Destroy(FindObjectOfType<PhotonVoiceNetwork>().gameObject);
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        SceneManager.LoadScene("FeedbackEnd");
        //Application.Quit();
    }
}