using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IDropHandler
{
    private bool isFree = true;
    [SerializeField] private DraggableUI inventoryImage;

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
        if (eventData.pointerDrag != null && isFree)
        {
            DraggableUI draggableObject = eventData.pointerDrag.GetComponent<DraggableUI>();
            draggableObject.SetIsOnValidPosition();
            inventoryImage.AddImage(draggableObject.GetImage().sprite,this);
            draggableObject.GetImage().enabled = false;
            draggableObject.SetMySlot(this);
            isFree = false;
        }
    }

    public void AddObject(Sprite sprite)
    {
        inventoryImage.AddImage(sprite,this);
        isFree = false;
    }
}
