using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableUI : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private Canvas canvas;

    public Canvas Canvas
    {
        get => canvas;
        set => canvas = value;
    }

    private RectTransform rectTransform;
    private Vector3 originalPosition;
    private CanvasGroup canvasGroup;
    private Image image;
    private ItemSlot mySlot;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = transform.position;
        canvasGroup = GetComponent<CanvasGroup>();
        image = GetComponent<Image>();
        mySlot = transform.parent.GetComponent<ItemSlot>();
    }

    public Image GetImage()
    {
        return image;
    }

    public ItemSlot GetMySlot()
    {
        return mySlot;
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        transform.SetParent(canvas.transform);
        canvasGroup.alpha = .6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(mySlot.transform);
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        transform.localPosition = Vector3.zero;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void AddImage(Sprite sprite)
    {
        image.sprite = sprite;
        image.enabled = true;
    }
}
