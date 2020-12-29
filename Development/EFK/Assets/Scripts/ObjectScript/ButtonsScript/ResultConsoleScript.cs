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
        text.text = addingValue.ToString();
        result += addingValue;
    }

    public void reset(int startingValue)
    {
        result = startingValue;
    }
}
