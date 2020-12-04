using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Locker : InteractableObject
{
    [SerializeField] private Collectable idCard;
    [SerializeField] private Sprite openSprite;
    private Sprite closedSprite;
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer employeePhoto;
    
    public Collectable IDCard
    {
        get => idCard;
        set => idCard = value;
    }
    public SpriteRenderer EmployeePhoto => employeePhoto;
    
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        closedSprite = GetComponent<SpriteRenderer>().sprite;
        employeePhoto = transform.Find("EmployeePhoto").GetComponent<SpriteRenderer>();
        employeePhoto.gameObject.SetActive(false);
    }

    public override void Interact(GameObject player)
    {
        if (!_hasBeenActivated)
        {
            _hasBeenActivated = true;
            spriteRenderer.sprite = openSprite;
            employeePhoto.gameObject.SetActive(true);
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            if (!idCard.HasBeenActivated) idCard.gameObject.SetActive(true);
            StartCoroutine(CloseLocker(2.0f));
        }
    }

    IEnumerator CloseLocker(float closeTime)
    {
        yield return new WaitForSeconds(closeTime);
        _hasBeenActivated = false;
        spriteRenderer.sprite = closedSprite;
        employeePhoto.gameObject.SetActive(false);
        idCard.gameObject.SetActive(false);
        gameObject.layer = LayerMask.NameToLayer("Interactable");
    }
}
