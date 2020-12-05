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
<<<<<<< HEAD
            PhotonNetwork.LoadLevel("FeedbackEnd");
=======
            PhotonNetwork.LeaveRoom();
            Destroy(FindObjectOfType<PhotonVoiceNetwork>().gameObject);
            //PhotonNetwork.LoadLevel("MainMenu");
            

>>>>>>> 48765572a7e0652b17f0c8a161bba93d54aff156
        }
        else
        {
            SceneManager.LoadScene("FeedbackEnd");
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