using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialButton : InteractableObject
{
    private SpriteRenderer lightRenderer;
    private Material materialCopy;
    private Color buttonColor;
    
    void Start()
    {
        lightRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        materialCopy = new Material(lightRenderer.material);
        SetMaterialColor();
        lightRenderer.material = materialCopy;
    }
    public override void Interact(GameObject player)
    {
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        lightRenderer.material.SetColor("GlowColor", buttonColor);
        StartCoroutine("ResetLightWithDelay", 1f); 
    }

    IEnumerator ResetLightWithDelay(float delay) {
        yield return new WaitForSeconds(delay);
        lightRenderer.material.SetColor("GlowColor", new Color(0, 0, 0, 0.5f));
        gameObject.layer = LayerMask.NameToLayer("Interactable");
        SetMaterialColor();
    }

    private void SetMaterialColor()
    {
        buttonColor = new Color(UnityEngine.Random.Range(0,256),UnityEngine.Random.Range(0,256),UnityEngine.Random.Range(0,256), 1);
        //materialCopy.SetColor("GlowColor", new Color(0,0,0,0.5f));
        transform.GetChild(0).GetComponent<SpriteRenderer>().color = buttonColor;
    }
}
