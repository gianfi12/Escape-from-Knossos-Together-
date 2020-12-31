﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Locker : InteractableObject
{
    [SerializeField] private Collectable idCard;
    [SerializeField] private GameObject cables;
    [SerializeField] private Sprite activeWithCard;
    [SerializeField] private Sprite activeWithoutCard;
    private Sprite inactiveWithCard;
    [SerializeField] private Sprite inactiveWithoutCard;
    [SerializeField] private Sprite employeePhoto;
    private SpriteRenderer spriteRenderer;

    public Collectable IDCard
    {
        get => idCard;
        set => idCard = value;
    }
    
    public Sprite EmployeePhoto
    {
        get => employeePhoto;
        set => employeePhoto = value;
    }
    
    public Sprite ActiveWithCard
    {
        get => activeWithCard;
        set => activeWithCard = value;
    }
    
    public Sprite ActiveWithoutCard
    {
        get => activeWithoutCard;
        set => activeWithoutCard = value;
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        inactiveWithCard = GetComponent<SpriteRenderer>().sprite;
    }

    public override void Interact(GameObject player)
    {
        if (!_hasBeenActivated)
        {
            FindObjectOfType<AudioManager>().Play("MonitorOn");
            _hasBeenActivated = true;
            spriteRenderer.sprite = activeWithCard;
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            cables.layer = LayerMask.NameToLayer("Ignore Raycast");
            if (!idCard.HasBeenActivated)
            {
                idCard.gameObject.SetActive(true);
                idCard.gameObject.layer = LayerMask.NameToLayer("Interactable");
            }
            else
            {
                spriteRenderer.sprite = activeWithoutCard;
            }
            StartCoroutine(CloseLocker(2.0f));
        }
    }

    IEnumerator CloseLocker(float closeTime)
    {
        yield return new WaitForSeconds(closeTime);
        _hasBeenActivated = false;
        if (idCard.HasBeenActivated) spriteRenderer.sprite = inactiveWithoutCard;
        else spriteRenderer.sprite = inactiveWithCard;
        idCard.gameObject.SetActive(false);
        idCard.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        gameObject.layer = LayerMask.NameToLayer("Interactable");
        cables.layer = LayerMask.NameToLayer("Interactable");
        FindObjectOfType<AudioManager>().Play("MonitorOff");
    }

    public void TakenCard()
    {
        spriteRenderer.sprite = activeWithoutCard;
    }
}
