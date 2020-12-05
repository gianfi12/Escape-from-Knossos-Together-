using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.PUN;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviourPunCallbacks
{
    /*public void ReloadStartingScene()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.LeaveRoom();
            Destroy(FindObjectOfType<PhotonVoiceNetwork>().gameObject);
            //PhotonNetwork.LoadLevel("MainMenu");
            

        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        SceneManager.LoadScene("MainMenu");
    }*/

    public void QuitGame()
    {
        Application.Quit();
    }
}