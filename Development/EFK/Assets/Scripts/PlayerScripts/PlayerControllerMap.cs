using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControllerMap : MonoBehaviour
{
    [SerializeField] private float _speed = 4f;
    private Vector3 _movement;
    private bool _hasChange = false;

    private GameManager gameManager;
    [SerializeField] private Text diaryText;
    private RoomAbstract roomManager;
    private List<GameObject> diaryComponents;
    
    public void SetLocation(Vector3 position)
    {
        transform.localPosition = position;
    }

    public void Move(Vector3 movement)
    {
        _movement = movement;
        _hasChange = true;
    }

    private void FixedUpdate()
    {
        if (_hasChange)
        {
            transform.position += _movement.normalized * Time.fixedDeltaTime * _speed;
            _hasChange = false;
        }
    }

    public Vector3 Movement => _movement.normalized;

    public void SetGameManager(GameManager manager)
    {
        gameManager = manager;
    }

    public void BuildCurrentDiary()
    {
        if (diaryComponents != null)
        {
            foreach (var component in diaryComponents)
            {
                if (component.name.Contains("Text"))
                {
                    //assegno il testo
                    SetText(component.GetComponent<Text>());
                }
                else if (component.name.Contains("Image"))
                {
                    //assegno l'immagine
                }
            }
        }
    }

    private void SetText(Text newText)
    {
        diaryText.text = newText.text;
    }

    public void SetRoomManager(RoomAbstract manager)
    {
        roomManager = manager;
        diaryComponents = new List<GameObject>();
        diaryComponents = roomManager.GetDiaryComponents();
        BuildCurrentDiary();
    }
}
