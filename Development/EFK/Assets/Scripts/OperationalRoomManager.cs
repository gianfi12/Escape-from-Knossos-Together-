using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
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
    private bool hasFinished;
    
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
            Transform newTransform = Instantiate(circlePrefab,_resultConsole.transform).transform;
            _spriteRenderers.Add(newTransform.GetComponent<SpriteRenderer>());
            newTransform.position -= new Vector3(4.25f, 2.3f+0.15f * i,1f);
        }
    }

    private List<List<int>> computeAllCombination(List<int> inputValues)
    {
        List<List<int>> combinationList = new List<List<int>>();
        Stack<List<int>> stack = new Stack<List<int>>();
        foreach (int inputValue in inputValues)
        {
            List<int> temp = new List<int>();
            temp.Add(inputValue);
            stack.Push(new List<int>(temp));
        }

        while (stack.Count!=0)
        {
            List<int> listOfInt = stack.Pop();
            combinationList.Add(new List<int>(listOfInt));
            if (listOfInt.Count == numberOfSteps) continue;
            for(int i=inputValues.IndexOf(listOfInt[listOfInt.Count-1]);i<inputValues.Count;i++)
            {
                List<int> temp = new List<int>(listOfInt);
                temp.Add(inputValues[i]);
                stack.Push(temp);
            }
        }

        return combinationList;
    }

    private Dictionary<int,int> aggregateAllCombination(List<List<int>> combinationList)
    {
        Dictionary<int,int> returnedAggregate = new Dictionary<int, int>();
        foreach (List<int> list in combinationList)
        {
            int sum = 0;
            foreach (int value in list)
            {
                sum += value;
            }
            returnedAggregate[sum]=returnedAggregate.ContainsKey(sum)?returnedAggregate[sum++]++:1;
        }

        return returnedAggregate;
    }

    private List<List<int>> computeAllCombinationWithAvalue(List<List<int>> combinationsList, int value)
    {
        List<List<int>> returnedList = new List<List<int>>();
        List<int> startList = new List<int>();
        startList.Add(value);
        returnedList.Add(new List<int>(startList));
        Stack<List<int>> stack = new Stack<List<int>>();
        stack.Push(new List<int>(startList));
        foreach (List<int> inputValue in combinationsList)
        {
            if(inputValue.Count<numberOfSteps) stack.Push(new List<int>(inputValue));
        }

        while (stack.Count!=0)
        {
            List<int> listOfInt = stack.Pop();
            List<int> temp = new List<int>(listOfInt);
            temp.Add(value);
            returnedList.Add(temp);
            temp = new List<int>(listOfInt);
            temp.Add(value);
            if (temp.Count == numberOfSteps) continue;
            stack.Push(temp);
        }

        return returnedList;
    }
    
    private void createValues()
    {
        int sum = 0;
        int value;
        // Dictionary<int, int> aggregatedResults;
        do
        {
            sum = 0;
            _combinationValues.Clear();
            for (int i = 0; i < numberOfSteps; i++)
            {
                while ((value = _rnd.Next(-9, 10)) == 0 || (value!=0 && checkNotInSelection(value,_combinationValues))) ;
                _combinationValues.Add(value);
                sum += value;
            }

            _finalValue = sum;

        } while (_finalValue >= 9 || _finalValue <= -9 || aggregateAllCombination(computeAllCombination(_combinationValues))[_finalValue]>1 || _finalValue==0);
        
        
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
        foreach (ButtonConsole buttonConsole in _buttonConsoles)
        {
            if(!buttonConsole.isSet)
            {
                List<List<int>> combinationsList = computeAllCombination(selectedValues);
                List<List<int>> tempCombinationList=new List<List<int>>();
                do
                {
                    while((value= _rnd.Next(-9, 10))==0 || (value!=0 && checkNotInSelection(value,selectedValues)));
                    
                    tempCombinationList = computeAllCombinationWithAvalue(new List<List<int>>(combinationsList),value);

                } while (aggregateAllCombination(tempCombinationList).ContainsKey(_finalValue));
                buttonConsole.updateValue(value);
                _buttonValues[index] = value;
                selectedValues.Add(value);
            }
            index++;
        }
        
        _startingValue = _rnd.Next(-9, 10);
        _finalValue += _startingValue;

        String solution = "OperationalRoom solution: ";
        foreach (int selectedValue in _combinationValues)
        {
            solution += " " + selectedValue + " ";
        }
        Debug.Log(solution);
        _resultConsole.updateValue(_startingValue);
    }

    private bool checkNotInSelection(int value, List<int> selectedValues)
    {
        int abs = Math.Abs(value);
        foreach (int selectedValue in selectedValues)
        {
            if (abs == Math.Abs(selectedValue) && value!=selectedValue)
                return true;
        }

        return false;
    }

    public void updateResultConsole(int value,PlayerControllerMap playerControllerMap)
    {
        if (hasFinished) return;
        _numberOfIteration++;
        _resultConsole.updateValue(value);
        if (_resultConsole.Result==_finalValue && numberOfSteps==_numberOfIteration)
        {
            doors.OpenDoors(true);
            confirmCounterForUser();
        }else if (_numberOfIteration == numberOfSteps)
        {
            playerControllerMap.IncrementTimer(-timePenalityInSeconds);
            _resultConsole.reset(_startingValue);
            foreach (ButtonConsole buttonConsole in _buttonConsoles)
            {
                buttonConsole.Button.resetPressedStatus();
            }
            StartCoroutine(resetCounterForUser());
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

    private IEnumerator resetCounterForUser()
    {
        updateCounterForUser(_numberOfIteration-1);
        yield return new WaitForSeconds(1);
        foreach (SpriteRenderer spriteRenderer in _spriteRenderers)
        {
            spriteRenderer.color = Color.white;
        }
        
        _numberOfIteration = 0;
    }

    private void confirmCounterForUser()
    {
        foreach (SpriteRenderer spriteRenderer in _spriteRenderers)
        {
            spriteRenderer.color = Color.green;
        }
    }
}
