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

    public void RemoveImage()
    {
        isFree = true;
        Image image =  slotImage.Image.GetComponent<Image>();
        image.sprite = null;
        image.enabled = false;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            DraggableUI draggableObject = eventData.pointerDrag.GetComponent<DraggableUI>();
            if (isFree)
            {
                draggableObject.GetMySlot().SetIsFree();
                slotImage.AddImage(draggableObject.MyCollectable);
                draggableObject.Image.enabled = false;
                isFree = false;
            }
            else
            {
                Collectable tempCollectable = draggableObject.MyCollectable;
                draggableObject.AddImage(slotImage.MyCollectable);
                slotImage.AddImage(tempCollectable);
            }
        }
    }

    public void AddObject(Collectable collectable)
    {
        slotImage.AddImage(collectable);
        isFree = false;
    }
}
