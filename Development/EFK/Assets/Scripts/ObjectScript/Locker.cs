using System;
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
    private AudioSource[] lockerSounds;

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
        lockerSounds = GetComponentsInChildren<AudioSource>();
    }

    public override void Interact(GameObject player)
    {
        if (!_hasBeenActivated)
        {
            System.Random random = new System.Random();
            lockerSounds[random.Next(0,lockerSounds.Length)].Play();
            _hasBeenActivated = true;
            spriteRenderer.sprite = activeWithCard;
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            cables.layer = LayerMask.NameToLayer("Ignore Raycast");
            cables.transform.localPosition = new Vector3(-0.31f, 0.86f, 0.01f);
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
        cables.transform.localPosition = new Vector3(-0.31f, 0.86f, -0.01f);
    }

    public void TakenCard()
    {
        spriteRenderer.sprite = activeWithoutCard;
        FindObjectOfType<AudioManager>().Play("CardTaken");
    }
}
