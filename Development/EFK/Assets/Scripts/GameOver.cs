using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver:MonoBehaviour
{
    public void ReloadStartingScene()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.LoadLevel("MainMenu");
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}