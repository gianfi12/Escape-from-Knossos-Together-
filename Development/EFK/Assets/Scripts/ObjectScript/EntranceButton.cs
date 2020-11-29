using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntranceButton : InteractableObject {

    private EntrancePanel panel;
    private Material materialCopy;
    private Color buttonColor;
    [SerializeField] private int lightIntensity;

    // Start is called before the first frame update
    void Start()
    {
        panel = GetComponentInParent<EntrancePanel>();

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        materialCopy = new Material(spriteRenderer.material);
        buttonColor = spriteRenderer.color;
        materialCopy.SetColor("OutlineColor", Color.black);
        materialCopy.SetTexture("SampledTexture", spriteRenderer.sprite.texture);
        materialCopy.SetFloat("OutlineThickness", 1);
        spriteRenderer.material = materialCopy;
    }

    public override void Interact(GameObject player) {
        GetComponent<SpriteRenderer>().material.SetColor("OutlineColor", buttonColor);
        panel.ButtonPressed(transform.GetSiblingIndex());
    }

    public void ResetLight() {
        GetComponent<SpriteRenderer>().material.SetColor("OutlineColor", Color.black);
    }
}
