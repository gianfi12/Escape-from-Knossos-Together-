using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class OperationalRoomManager : MonoBehaviour
{
    [SerializeField] private int numberOfSteps;
    [SerializeField] private int timePenalityInSeconds;
    [SerializeField] private Doors doors;
    [SerializeField] private GameObject circlePrefab;
    [SerializeField] private RoomPrefab operationalRoomPrefab;
    [SerializeField] private Sprite plusSprite;
    [SerializeField] private Sprite minusSprite;
    [SerializeField] private List<Image> guiImage;

    private List<ButtonConsole> _buttonConsoles;
    private ResultConsoleScript _resultConsole;
    private int _startingValue;
    private int _finalValue;
    private int _numberOfIteration;
    private List<int> _combinationValues;
    private int[] _buttonValues;
    private Random _rnd;
    private List<SpriteRenderer> _spriteRenderers;
    
    private void Awake()
    {
        _buttonConsoles = new List<ButtonConsole>();
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject gameobject = transform.GetChild(i).gameObject;
            if (gameobject.name.Contains("ButtonConsole"))
            {
                _buttonConsoles.Add(gameobject.GetComponent<ButtonConsole>());
            }else if (gameobject.name.Contains("ResultConsole"))
            {
                _resultConsole = gameobject.GetComponent<ResultConsoleScript>();
            }
        }
    }

    private void Start()
    {
        _rnd = new Random(GetComponent<ObjectsContainer>().Seed);
        _combinationValues = new List<int>();
        _combinationValues = new List<int>();
        _buttonValues = new int[_buttonConsoles.Count];
        _spriteRenderers = new List<SpriteRenderer>();    
        createValues();
        placeValues();
        placeCounterForUser();
        GameObject temp = new GameObject("OpText");
        Text text = temp.AddComponent<Text>();
        text.text = "Final Result: " + _finalValue;
        operationalRoomPrefab.SetDiaryText(text);
        for (int i = 0; i < _buttonValues.Length; i++)
        {
            guiImage[i].sprite = _buttonValues[i] < 0 ? minusSprite : plusSprite ; 
            operationalRoomPrefab.AddDiaryImage(guiImage[i]);
        }
    }

    private void placeCounterForUser()
    {
        for (int i = 0; i < numberOfSteps; i++)
        {
            Transform newTransform = Instantiate(circlePrefab, transform).transform;
            _spriteRenderers.Add(newTransform.GetComponent<SpriteRenderer>());
            newTransform.position -= new Vector3(0f, 0.15f * i,0f);
        }
    }
    
    private void placeValues()
    {
        List<int> selectedValues = new List<int>();
        selectedValues.AddRange(_combinationValues);
        for (int i = 0; i < numberOfSteps; i++)
        {
            int buttonIndex;
            while (_buttonConsoles[(buttonIndex = _rnd.Next(0, _buttonConsoles.Count))].isSet) ;
            _buttonConsoles[buttonIndex].updateValue(_combinationValues[i]);
            _buttonValues[buttonIndex] = _combinationValues[i];
        }

        int index = 0;
        int value;
        foreach (ButtonConsole buttonConsole in _buttonConsoles)
        {
            if(!buttonConsole.isSet)
            {
                while((value = _rnd.Next(-9, 10))==0 || (value!=0 && selectedValues.Contains(value)));
                buttonConsole.updateValue(value);
                _buttonValues[index] = value;
                selectedValues.Add(value);
            }
            index++;
        }
        _resultConsole.updateValue(_startingValue);
    }
    private void createValues()
    {
        _startingValue = _rnd.Next(-9, 10);

        int sum = 0;
        int value;
        for (int i = 0; i < numberOfSteps; i++)
        {
            while((value = _rnd.Next(-9, 10))==0 && _combinationValues.Contains(value));
            _combinationValues.Add(value);
            sum += value;
        }

        _finalValue = sum + _startingValue;
    }
    
    public void updateResultConsole(int value,PlayerControllerMap playerControllerMap)
    {
        _numberOfIteration++;
        _resultConsole.updateValue(value);
        if (_resultConsole.Result==_finalValue)
        {
            doors.OpenDoors();
            resetCounterForUser();
        }else if (_numberOfIteration == numberOfSteps)
        {
            playerControllerMap.IncrementTimer(-timePenalityInSeconds);
            _resultConsole.reset(_startingValue);
            resetCounterForUser();
        }
        else
        {
            updateCounterForUser(_numberOfIteration-1);
        }
    }

    private void updateCounterForUser(int value)
    {
        _spriteRenderers[value].color = Color.red;
    }

    private void resetCounterForUser()
    {
        foreach (SpriteRenderer spriteRenderer in _spriteRenderers)
        {
            spriteRenderer.color = Color.white;
        }
        
        _numberOfIteration = 0;
    }
}
