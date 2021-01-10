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
        lightRenderer.material = materialCopy;
        SetMaterialColor();
    }
    public override void Interact(GameObject player)
    {
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        materialCopy.SetColor("GlowColor", buttonColor * 1.2f);
        StartCoroutine("ResetLightWithDelay", 1f); 
    }

    IEnumerator ResetLightWithDelay(float delay) {
        yield return new WaitForSeconds(delay);
        materialCopy.SetColor("GlowColor", new Color(0, 0, 0, 0.5f));
        gameObject.layer = LayerMask.NameToLayer("Interactable");
        SetMaterialColor();
    }

    private void SetMaterialColor()
    {
        buttonColor = new Color(UnityEngine.Random.Range(0.0f,1.0f), UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f), 1);
        Debug.Log(buttonColor);
        lightRenderer.color = buttonColor;
    }
}
