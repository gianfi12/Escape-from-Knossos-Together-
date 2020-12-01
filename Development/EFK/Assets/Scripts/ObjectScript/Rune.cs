using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rune : InteractableObject
{
    public override void Interact(GameObject player)
    {
        PlayerControllerMap playerControllerMap = player.GetComponent<PlayerControllerMap>();
        ItemSlot slot = playerControllerMap.GetFirstFreeSlot();
        slot.AddObject(GetComponent<SpriteRenderer>().sprite);
        gameObject.SetActive(false);
    }
}
