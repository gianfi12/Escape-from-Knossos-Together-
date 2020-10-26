using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameInput : MonoBehaviour
{
    [SerializeField] private InputField _nameInputField = null;

    private const string PlayerprefsNameKey = "PlayerName";
    
    // Start is called before the first frame update
    void Start()
    {
        SetUpInputField();
    }

    private void SetUpInputField()
    {
        if (!PlayerPrefs.HasKey(PlayerprefsNameKey)) return;

        string defaultName = PlayerPrefs.GetString(PlayerprefsNameKey);
        _nameInputField.text = defaultName;
        
        //SetPlayerName(defaultName);
    }

    /*public void SetPlayerName(string playerName)
    {
        _continueButton.interactable = !string.IsNullOrEmpty(playerName);
        print(!string.IsNullOrEmpty(playerName));
    }*/

    public void SavePlayerName()
    {
        string playerName = _nameInputField.text;
        PhotonNetwork.NickName = playerName;
        PlayerPrefs.SetString(PlayerprefsNameKey, playerName);
    }
}
