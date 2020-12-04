using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver:MonoBehaviour
{
    public void ReloadStartingScene()
    {
        PhotonNetwork.LoadLevel("Main");
    }
}