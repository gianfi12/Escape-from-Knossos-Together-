using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultConsoleScript : MonoBehaviour
{
    [SerializeField] public Text text;

    private int result;

    public int Result => result;

    public void updateValue(int addingValue)
    {
        text.text = (result+addingValue).ToString();
        result += addingValue;
    }

    public void reset(int startingValue)
    {
        text.text = startingValue.ToString();
        result = startingValue;
    }
}
