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
    [SerializeField] private List<ItemSlot> slots;
    private RoomAbstract myRoom;
    private List<GameObject> diaryTextList;
    private List<GameObject> diaryImageList;

    
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

    public void SetMyRoom(RoomAbstract room)
    {
        myRoom = room;
        diaryTextList = new List<GameObject>();
        diaryImageList = new List<GameObject>();
        diaryTextList = myRoom.DiaryTextList;
        diaryImageList = myRoom.DiaryImageList;
        BuildCurrentDiary();
    }
    public void BuildCurrentDiary()
    {
        if (diaryTextList != null)
        {
            foreach (var component in diaryTextList)
            {
                //assegno il testo
                SetText(component.GetComponent<Text>());
            }
        }
        if (diaryImageList != null)
        {
            foreach (var component in diaryImageList)
            {
                //assegno l'immagine
            }
        }
    }

    private void SetText(Text newText)
    {
        diaryText.text = newText.text;
    }
    
    public ItemSlot GetFirstFreeSlot()
    {
        return slots.Find(s => s.GetIsFree());
    }
}
