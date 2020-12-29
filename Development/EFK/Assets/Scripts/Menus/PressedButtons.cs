using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PressedButtons : ActivatableObject
{
    [SerializeField] private Color emptyColor;
    private int pressed = 0;


    public void UpdatePressedColors(Color color) {
        transform.GetChild(pressed).GetComponent<Image>().color = color;
        pressed ++;
    }

    public void ResetPressedColors() {
        foreach(Transform child in transform) {
            child.GetComponent<Image>().color = emptyColor;
        }
        pressed = 0;
    }

    public override void ActivateObject() {
        gameObject.SetActive(true);
        GetComponent<Animator>().SetTrigger("Popup");
    }

    public override void DeactivateObject() {
        GetComponent<Animator>().SetTrigger("Close");
    }

    public void SetActiveFalse() {
        gameObject.SetActive(false);
    }
}
