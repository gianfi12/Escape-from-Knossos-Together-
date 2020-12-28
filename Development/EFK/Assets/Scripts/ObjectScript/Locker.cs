using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Locker : InteractableObject
{
    [SerializeField] private Collectable idCard;
    [SerializeField] private Sprite activeSprite;
    [SerializeField] private Sprite employeePhoto;
    private Sprite inactiveSprite;
    private SpriteRenderer spriteRenderer;

    public Collectable IDCard
    {
        get => idCard;
        set => idCard = value;
    }
    
    public Sprite EmployeePhoto => employeePhoto;
    public Sprite ActiveSprite
    {
        get => activeSprite;
        set => activeSprite = value;
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        inactiveSprite = GetComponent<SpriteRenderer>().sprite;
    }

    public override void Interact(GameObject player)
    {
        if (!_hasBeenActivated)
        {
            _hasBeenActivated = true;
            spriteRenderer.sprite = activeSprite;
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            if (!idCard.HasBeenActivated)
            {
                idCard.gameObject.SetActive(true);
                idCard.gameObject.layer = LayerMask.NameToLayer("Interactable");
            }
            StartCoroutine(CloseLocker(2.0f));
        }
    }

    IEnumerator CloseLocker(float closeTime)
    {
        yield return new WaitForSeconds(closeTime);
        _hasBeenActivated = false;
        spriteRenderer.sprite = inactiveSprite;
        idCard.gameObject.SetActive(false);
        idCard.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        gameObject.layer = LayerMask.NameToLayer("Interactable");
    }
}
