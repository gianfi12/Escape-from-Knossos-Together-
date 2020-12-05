using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver:MonoBehaviour
{
    public void ReloadStartingScene()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.LoadLevel("FeedbackEnd");
        }
        else
        {
            SceneManager.LoadScene("FeedbackEnd");
        }
    }
}