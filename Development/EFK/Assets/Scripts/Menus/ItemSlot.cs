using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IDropHandler
{
    private bool isFree = true;
    [SerializeField] private DraggableUI slotImage;

    public DraggableUI SlotImage => slotImage;

    public bool GetIsFree()
    {
        return isFree;
    }

    public void SetIsFree()
    {
        isFree = true;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            DraggableUI draggableObject = eventData.pointerDrag.GetComponent<DraggableUI>();
            if (isFree)
            {
                draggableObject.GetMySlot().SetIsFree();
                slotImage.AddImage(draggableObject.GetImage().sprite);
                draggableObject.GetImage().enabled = false;
                isFree = false;
            }
            else
            {
                Sprite tempSprite = draggableObject.GetImage().sprite;
                draggableObject.AddImage(slotImage.GetImage().sprite);
                slotImage.AddImage(tempSprite);
            }
        }
    }

    public void AddObject(Sprite sprite)
    {
        slotImage.AddImage(sprite);
        isFree = false;
    }
}
