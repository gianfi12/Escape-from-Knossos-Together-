using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorButton : InteractableObject {

    private ColorButtonPanel panel;
    private SpriteRenderer lightRenderer;
    private Material materialCopy;
    private Color buttonColor;


    // Start is called before the first frame update
    void Start()
    {
        panel = GetComponentInParent<ColorButtonPanel>();

        lightRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        materialCopy = new Material(lightRenderer.material);
        materialCopy.SetColor("GlowColor", new Color(0,0,0,0.5f));
        lightRenderer.material = materialCopy;
    }

    public void SetButtonColor(Color color) {
        buttonColor = color;
        transform.GetChild(0).GetComponent<SpriteRenderer>().color = color;
    }

    public override void Interact(GameObject player) {
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        lightRenderer.material.SetColor("GlowColor", buttonColor * 1.2f);
        panel.ButtonPressed(transform.GetSiblingIndex());
    }

    public void ResetLight() {
        StartCoroutine("ResetLightWithDelay", 0.3f); 
    }

    IEnumerator ResetLightWithDelay(float delay) {
        yield return new WaitForSeconds(delay);
        lightRenderer.material.SetColor("GlowColor", new Color(0, 0, 0, 0.5f));
        gameObject.layer = LayerMask.NameToLayer("Interactable");
    }
}
