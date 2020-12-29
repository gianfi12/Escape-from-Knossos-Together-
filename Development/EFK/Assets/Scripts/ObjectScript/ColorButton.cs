using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorButton : InteractableObject {

    private ColorButtonPanel panel;
    private Material materialCopy;
    private Color buttonColor;
    [SerializeField] private int lightIntensity;

    // Start is called before the first frame update
    void Start()
    {
        panel = GetComponentInParent<ColorButtonPanel>();

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        materialCopy = new Material(spriteRenderer.material);
        materialCopy.SetColor("OutlineColor", Color.black);
        materialCopy.SetTexture("SampledTexture", spriteRenderer.sprite.texture);
        materialCopy.SetFloat("OutlineThickness", 1);
        spriteRenderer.material = materialCopy;
    }

    public void SetButtonColor(Color color) {
        buttonColor = color;
        GetComponent<SpriteRenderer>().color = color;
    }

    public override void Interact(GameObject player) {
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        GetComponent<SpriteRenderer>().material.SetColor("OutlineColor", buttonColor);
        panel.ButtonPressed(transform.GetSiblingIndex());
    }

    public void ResetLight() {
        StartCoroutine("ResetLightWithDelay", 0.3f);
   
    }

    IEnumerator ResetLightWithDelay(float delay) {
        yield return new WaitForSeconds(delay);
        GetComponent<SpriteRenderer>().material.SetColor("OutlineColor", Color.black);
        gameObject.layer = LayerMask.NameToLayer("Interactable");
    }
}
