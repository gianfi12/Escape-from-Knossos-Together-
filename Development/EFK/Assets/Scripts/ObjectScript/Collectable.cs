using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Collectable : InteractableObject
{
    private int id;
    [SerializeField] private bool hasParent;

    public int ID
    {
        get => id;
        set => id = value;
    }

    public override void Interact(GameObject player)
    {
        PlayerControllerMap playerControllerMap = player.GetComponent<PlayerControllerMap>();
        ItemSlot slot = playerControllerMap.GetFirstFreeSlot();
        slot.AddObject(this);
        gameObject.SetActive(false);
        _hasBeenActivated = true;
        if (hasParent)
        {
            GetComponentInParent<Locker>().TakenCard();
        }
        else
        {
            FindObjectOfType<AudioManager>().Play("RuneTaken");
        }
    }
}
