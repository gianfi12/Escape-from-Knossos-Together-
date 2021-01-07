using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonConsole : MonoBehaviour
{
    [SerializeField] public Text text;
    private Button _button;
    private int value;
    public bool isSet;

    public Button Button => _button;
    
    private OperationalRoomManager _operationalRoomManager;

    private void Awake()
    {
        _operationalRoomManager = transform.parent.GetComponent<OperationalRoomManager>();
        _button = GetComponentInChildren<Button>();
    }
    
    public void updateValue(int newText)
    {
        isSet = true;
        text.text = Math.Abs(newText).ToString();
        value = newText;
    }

    public void updateResult(PlayerControllerMap playerControllerMap)
    {
        _operationalRoomManager.updateResultConsole(value,playerControllerMap);
    }
}
