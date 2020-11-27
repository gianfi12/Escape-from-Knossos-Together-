using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rune : InteractableObject
{
    private Sprite image;
    

    private void Awake()
    {
        image = GetComponent<SpriteRenderer>().sprite;
    }

    public override void Interact(GameObject player)
    {
        PlayerControllerMap playerControllerMap = player.GetComponent<PlayerControllerMap>();
        ItemSlot slot = playerControllerMap.GetFirstFreeSlot();
        slot.AddObject(image);
        gameObject.SetActive(false);
    }
}
