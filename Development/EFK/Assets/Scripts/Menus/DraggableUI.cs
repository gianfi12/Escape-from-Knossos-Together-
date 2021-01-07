using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableUI : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private Canvas canvas;
    private Collectable myCollectable;
    private RectTransform rectTransform;
    private Vector3 originalPosition;
    private CanvasGroup canvasGroup;
    private Image image;

    public Image Image
    {
        get => image;
        set => image = value;
    }

    private ItemSlot mySlot;
    
    public Canvas Canvas
    {
        get => canvas;
        set => canvas = value;
    }
    
    public Collectable MyCollectable
    {
        get => myCollectable;
        set => myCollectable = value;
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = transform.position;
        canvasGroup = GetComponent<CanvasGroup>();
        image = GetComponent<Image>();
        mySlot = transform.parent.GetComponent<ItemSlot>();
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
        FindObjectOfType<AudioManager>().Play("DragUI");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(mySlot.transform);
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        transform.localPosition = Vector3.zero;
        if (image.enabled) FindObjectOfType<AudioManager>().Play("SnapUI");
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void AddImage(Collectable collectable)
    {
        myCollectable = collectable;
        image.sprite = collectable.GetComponent<SpriteRenderer>().sprite;
        image.enabled = true;
    }
}
