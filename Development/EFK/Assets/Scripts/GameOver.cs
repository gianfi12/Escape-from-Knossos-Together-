using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.PUN;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviourPunCallbacks
{
    public void ReloadStartingScene()
    {
        if (PhotonNetwork.IsConnected)
        {
            //PhotonNetwork.LoadLevel("FeedbackEnd");
            PhotonNetwork.LeaveRoom();
            //Destroy(FindObjectOfType<PhotonVoiceNetwork>().gameObject);
            //PhotonNetwork.LoadLevel("MainMenu");
        }
        else
        {
            SceneManager.LoadScene("FeedbackEnd");
        }
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        Destroy(FindObjectOfType<PhotonVoiceNetwork>().gameObject);
        Destroy(FindObjectOfType<NavMeshSurface2d>().gameObject);
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        SceneManager.LoadScene("FeedbackEnd");
        //Application.Quit();
    }
}