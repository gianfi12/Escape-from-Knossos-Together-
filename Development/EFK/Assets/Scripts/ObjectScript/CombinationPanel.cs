using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombinationPanel : InteractableObject
{
    private GameObject panel;
    private Canvas canvasToReturn;
    private ItemSlot[] slots = new ItemSlot[5];

    public GameObject Panel => panel;

    public ItemSlot[] Slots => slots;

    void Start()
    {
        panel = transform.parent.GetComponentInChildren<Canvas>().GetComponentInChildren<Image>().gameObject;
        GameObject slotPanel = panel.GetComponentInChildren<GridLayoutGroup>().gameObject;
        for (int i = 0; i < slotPanel.transform.childCount; i++)
        {
            slots[i] = slotPanel.transform.GetChild(i).GetComponent<ItemSlot>();
        }
        panel.gameObject.SetActive(false);
    }

    public override void Interact(GameObject player)
    {
        if (!_hasBeenActivated)
        {
            _hasBeenActivated = true;
            canvasToReturn = panel.transform.parent.GetComponent<Canvas>();
            Canvas playerCanvas = player.GetComponentInChildren<Canvas>();
            panel.transform.SetParent(playerCanvas.transform);
            foreach (var slot in slots)
            {
                slot.SlotImage.Canvas = playerCanvas;
            }
            panel.gameObject.SetActive(true);
        }
        else
        {
            _hasBeenActivated = false;
            panel.transform.SetParent(canvasToReturn.transform);
            foreach (var slot in slots)
            {
                slot.SlotImage.Canvas = canvasToReturn;
            }
            panel.gameObject.SetActive(false);
        }
    }
    
}
